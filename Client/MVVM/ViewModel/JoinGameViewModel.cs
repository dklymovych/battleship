using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Client.Core;
using Client.MVVM.Model;
using Client.MVVM.View;
using Client.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client.MVVM.ViewModel;
public class Ship
{
    public List<List<Coordinate>> Cords { get; set; }
    public int Count => Cords?.Count ?? 0;
}

public class Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }
}
public class JoinGameViewModel : Core.ViewModel
{
    private string _gameCode;

    public string GameCode
    {
        get => _gameCode;
        set
        {
            _gameCode = value;
            OnPropertyChanged("GameCode");
        }
    }
    public List<Square> Squares { get; set; } = new List<Square>();


    public Dictionary<string, List<List<Dictionary<string, int>>>> ships =
        new Dictionary<string, List<List<Dictionary<string, int>>>>()
        {
            {"4", new List<List<Dictionary<string, int>>>()}, {"3", new List<List<Dictionary<string, int>>>()}, 
            {"2", new List<List<Dictionary<string, int>>>()}, {"1", new List<List<Dictionary<string, int>>>()}
        };
    
    private Dictionary<int, int> _shipLengthsAvailable = new Dictionary<int, int>
    {
        { 1, 4 },
        { 2, 3 },
        { 3, 2 },
        { 4, 1 }
    };
    
    private INavigationService _navigation;
    
    private Dictionary<string, List<List<Coordinate>>> LoadShipsFromJson()
    {
        var json = File.ReadAllText("C:/Users/newme/RiderProjects/battleship/Client/Resources/shipsGot.json");
        return JsonConvert.DeserializeObject<Dictionary<string, List<List<Coordinate>>>>(json);
    }
    
    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }
    private void InitializeSquares()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Square temp = new Square(j, i);
                Squares.Add(temp);
            }
        }
        
    }
    public void SquareClick(Square square)
    {
       
        // var ships = LoadShipsFromJson();
        // foreach (var ship in ships)
        // {
        //     foreach (var cordList in ship.Value)
        //     {
        //         foreach (var coordinate in cordList)
        //         {
        //             int temp = coordinate.X + coordinate.Y*10;
        //             Squares[temp].isShip = true;
        //             Squares[temp].isCloseToShip = true;
        //             Squares[temp].ChangeColor();
        //         }
        //     }
        // }
        var shipSelectionWindow = new ShipSelectionView(_shipLengthsAvailable);
        if (shipSelectionWindow.ShowDialog() == true)
        {
            int length = shipSelectionWindow.SelectedLength;
            string orientation = shipSelectionWindow.SelectedOrientation;
        
            if (_shipLengthsAvailable.ContainsKey(length) && _shipLengthsAvailable[length] > 0)
            {
                if (IsValidPlacement(square, length, orientation))
                {
                    PlaceShip(square, length, orientation);
                    _shipLengthsAvailable[length]--;
                }
            }
        }
    }

    private void PlaceShip(Square startSquare, int length, string orientation)
    {
        List<Dictionary<string, int>> temp = new List<Dictionary<string, int>>();
        if (!IsValidPlacement(startSquare, length, orientation))
        {
            // Handle invalid placement
            return;
        }
        if (length == 1)
        {
            MarkShipAndAdjacent(startSquare.X, startSquare.Y);
            Dictionary<string, int> temp2 = new Dictionary<string, int>();
            temp2.Add("X",startSquare.X);
            temp2.Add("Y",startSquare.Y);
            temp.Add(temp2);
        }
        else if (orientation == "Horizontal")
        {
            for (int i = 0; i < length; i++)
            {
                MarkShipAndAdjacent(startSquare.X + i, startSquare.Y);
                Dictionary<string, int> temp2 = new Dictionary<string, int>();
                temp2.Add("X",startSquare.X + i);
                temp2.Add("Y",startSquare.Y);
                temp.Add(temp2);
            }
        }
        else if (orientation == "Vertical")
        {
            for (int i = 0; i < length; i++)
            {
                MarkShipAndAdjacent(startSquare.X, startSquare.Y + i);
                Dictionary<string, int> temp2 = new Dictionary<string, int>();
                temp2.Add("X",startSquare.X);
                temp2.Add("Y",startSquare.Y + i);
                temp.Add(temp2);
            }
        }
        switch (length)
        {
            case 1: 
                ships["1"].Add(temp);
                break;
            case 2: 
                ships["2"].Add(temp);
                break;
            case 3: 
                ships["3"].Add(temp);
                break;
            case 4: 
                ships["4"].Add(temp);
                break;
            default:
                break;
            
        }
    }

    private void MarkShipAndAdjacent(int x, int y)
    {
        Square shipSquare = Squares.FirstOrDefault(s => s.X == x && s.Y == y);
        if (shipSquare != null)
        {
            shipSquare.isShip = true; // Mark as ship
            shipSquare.isCloseToShip = true; // Also mark as close to ship
            shipSquare.ChangeColor(); // Update color
        }

        // Mark adjacent squares including corners
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // Skip the ship square itself

                Square adjacentSquare = Squares.FirstOrDefault(s => s.X == x + i && s.Y == y + j);
                if (adjacentSquare != null)
                {
                    adjacentSquare.isCloseToShip = true; // Mark as close to ship
                    adjacentSquare.ChangeColor(); // Update color
                }
            }
        }
    }
    
    private bool IsValidPlacement(Square startSquare, int length, string orientation)
    {
        // Check if the ship goes out of bounds
        if (orientation == "Vertical" && startSquare.Y + length > 10)
        {
            return false;
        }
        else if (orientation == "Horizontal" && startSquare.X + length > 10)
        {
            return false;
        }

        // Check for overlapping with existing ships or being too close to other ships
        for (int i = 0; i < length; i++)
        {
            int x = orientation == "Horizontal" ? startSquare.X + i : startSquare.X;
            int y = orientation == "Vertical" ? startSquare.Y + i : startSquare.Y;

            if (!IsSquareValidForShip(x, y))
            {
                return false;
            }
        }

        return true; // Valid placement
    }

    private bool IsSquareValidForShip(int x, int y)
    {
        // Check the square itself and its adjacent squares
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Square adjacentSquare = Squares.FirstOrDefault(s => s.X == x + i && s.Y == y + j);
                if (adjacentSquare != null && adjacentSquare.isShip)
                {
                    return false; // Invalid if any adjacent square is a ship or too close to a ship
                }
            }
        }
        return true; // Square and its adjacent are valid for placing a ship
    }
    
    public ICommand SquareClickCommand { get; }
    public ICommand ClearBoardCommand { get; private set; }

    private void ClearBoard()
    {
        foreach (var square in Squares)
        {
            square.isShip = false;
            square.isCloseToShip = false;
            square.ChangeColor();
        }

        // Reset the ship lengths available to initial values
        _shipLengthsAvailable = new Dictionary<int, int>
        {
            { 1, 4 },
            { 2, 3 },
            { 3, 2 },
            { 4, 1 }
        };
        foreach (var ship in ships)
        {
            ship.Value.Clear();
        }
    }

    public RelayCommand NavigateToHomeCommand { get; set; }
    public ICommand OkCommand { get; private set; }

    private  bool OnOk()
    {
        if (_shipLengthsAvailable.Any(kv => kv.Value > 0))
        {
            MessageBox.Show("Please place all ships in board!", "Alert", MessageBoxButton.OK);
            return false;
        }

        return true;
    }

    public ICommand ConvertToJsonCommand { get; private set; }
    public ICommand JoinGameCommand { get; private set; }

    private async void JoinGame()
    {
        if (OnOk())
        {
            if (GameCode != "")
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",Globals.LogginInUser.access_token);
                    string apiUrl = "http://localhost:5199";  // This needs to be in file config

                    var postData = new { shipsPosition = ships }; 

                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage responsePost = await httpClient.PutAsync($"{apiUrl}/api/Game/JoinRoom/{GameCode}", content);

                    if (responsePost.IsSuccessStatusCode)
                    {
                        //Globals.GameCode = GameCode;
                        //MessageBox.Show($"Success");
                        Navigation.NavigateTo<RatingViewModel>(); //this need to be in future a battle form
                    }
                    else
                    {
                        if ((int)responsePost.StatusCode == 400 )
                        {
                            MessageBox.Show("BAD REQUEST", "Alert");
                        }
                        else  if ((int)responsePost.StatusCode == 404 )
                        {
                            MessageBox.Show("THERE IS NO SUCH ROOM", "Warning");
                        }
                        else if ((int)responsePost.StatusCode == 401)
                        {
                            Globals.LogginInUser.Logout();
                            Navigation.NavigateTo<HomeViewModel>();
                        }
                        else
                        {
                            MessageBox.Show($"{responsePost.StatusCode} {(int)responsePost.StatusCode}");
                        }
                       
                    }
           
                }
            }
            else
            {
                MessageBox.Show("Please enter game code!", "Alert", MessageBoxButton.OK);
            }
        }
    }
    
    public JoinGameViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        InitializeSquares();
        ClearBoardCommand = new RelayCommand(o => ClearBoard(), canExecute:o => true);
        SquareClickCommand = new RelayCommand(o=>{SquareClick(o as Square);}, canExecute:o=> true);
        OkCommand = new RelayCommand(o => OnOk(), o => true);
        JoinGameCommand = new RelayCommand(o => JoinGame(), o => true);

        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        // ConvertToJsonCommand =  new RelayCommand(o => SerializeJson(), o => true);
    }
}