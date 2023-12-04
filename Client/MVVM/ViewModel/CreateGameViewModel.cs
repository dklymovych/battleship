using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Client.Core;
using Client.MVVM.Model;
using Client.MVVM.View;
using Client.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client.MVVM.ViewModel;

public class CreateGameViewModel : Core.ViewModel
{
    private bool _isPrivate;
    public bool IsPrivate
    {
        get => _isPrivate;
        set
        {
            _isPrivate = value;
            OnPropertyChanged();
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
    
    private Dictionary<string, Ship> LoadShipsFromJson()
    {
        var json = File.ReadAllText("C:/Users/newme/RiderProjects/battleship/Client/Resources/ships.json");
        return JsonConvert.DeserializeObject<Dictionary<string, Ship>>(json);
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

    private bool OnOk()
    {
        if (_shipLengthsAvailable.Any(kv => kv.Value > 0))
        {
            MessageBox.Show("Please place all ships in board!", "Alert", MessageBoxButton.OK);
            return false;
        }
        return true;
    }
    
    public RelayCommand NavigateToSettingsViewCommand { get; set; }
    public RelayCommand NavigateToGameViewCommand { get; set; }
    public RelayCommand NavigateToRatingViewCommand { get; set; }
    public RelayCommand NavigateToCreateGameViewCommand { get; set; }


    public ICommand TogglePrivacyCommand { get; private set; }
    private void TogglePrivacy()
    {
        IsPrivate = !IsPrivate;
    }
    public ICommand CreateCommand { get; private set; }

    private async void CreateGame()
    {
        if( OnOk())
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",Globals.LogginInUser.access_token);
                string apiUrl = "http://localhost:5199";  // This needs to be in file config

                var postData = new { isPublic = !IsPrivate, shipsPosition = ships }; 

                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage responsePost = await httpClient.PostAsync($"{apiUrl}/api/Game/CreateRoom", content);

                if (responsePost.IsSuccessStatusCode)
                {
                    var responseContent = await responsePost.Content.ReadAsStringAsync();
                    var gameCode = JObject.Parse(responseContent)["gameCode"]!.ToString();
                    Globals.GameCode = gameCode;
                    ClearBoard();
                    Navigation.NavigateTo<WaitingPageViewModel>();
                }
                else
                {
                    if ((int)responsePost.StatusCode == 400 )
                    {
                        MessageBox.Show("BAD REQUEST", "Alert");
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
    }

    public CreateGameViewModel(INavigationService navService)
    {
        Navigation = navService;
        IsPrivate = true; // Default value
        InitializeSquares();
        ClearBoardCommand = new RelayCommand(o => ClearBoard(), canExecute:o => true);
        SquareClickCommand = new RelayCommand(o=>{SquareClick(o as Square);}, canExecute:o=> true);
        OkCommand = new RelayCommand(o => OnOk(), o => true);
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        NavigateToGameViewCommand = new RelayCommand(o => { Navigation.NavigateTo<GameViewModel>();}, canExecute:o => true );
        TogglePrivacyCommand = new RelayCommand(o => TogglePrivacy(), o => true);
        CreateCommand =  new RelayCommand(o => CreateGame(), o => true);
    }
}