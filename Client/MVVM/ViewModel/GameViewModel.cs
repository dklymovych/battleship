using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Core;
using Client.MVVM.Model;
using Client.Services;
using Newtonsoft.Json.Linq;

namespace Client.MVVM.ViewModel;

public class GameViewModel : Core.ViewModel
{
    private INavigationService _navigation;
    private DispatcherTimer timer;
    private TimeSpan time;
    public List<BattlefieldSquare> MySquares { get; set; } = new List<BattlefieldSquare>();
    public List<BattlefieldSquare> EnemySquares { get; set; } = new List<BattlefieldSquare>();
    System.Windows.Threading.DispatcherTimer TimerForMove = new System.Windows.Threading.DispatcherTimer();
    
    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }
    private string _myUsername;
    public string MyUsername
    {
        get => _myUsername;
        set
        {
            _myUsername = value;
            OnPropertyChanged("MyUsername");
        }
    }
    private string _enemyUsername;
    public string EnemyUsername
    {
        get => _enemyUsername;
        set
        {
            _enemyUsername = value;
            OnPropertyChanged("EnemyUsername");
        }
    }
    private string _timeToShow;
    public string TimeToShow
    {
        get => _timeToShow;
        set
        {
            _timeToShow = value;
            OnPropertyChanged("TimeToShow");
        }
    }

    private string _myMove;
    public string MyMove
    {
        get => _myMove;
        set
        {
            _myMove = value;
            OnPropertyChanged("MyMove");
        }
    }
    private bool _isAllowedToMove;
    public bool IsAllowedToMove
    {
        get => _isAllowedToMove;
        set
        {
            _isAllowedToMove = value;
            OnPropertyChanged("IsAllowedToMove");
        }
    }
    public RelayCommand NavigateToHomeCommand { get; set; }
    public ICommand CleanerCommand { get; set; }
    private void InitializeSquares()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                BattlefieldSquare mySquare = new BattlefieldSquare(j, i, 0);
                MySquares.Add(mySquare);
                BattlefieldSquare enemySquare = new BattlefieldSquare(j, i, 0);
                EnemySquares.Add(enemySquare);
            }
        }
    }

    private void ConvertToMySquares()
    {
        for (int i = 0; i < Globals.Battlefield.Count; i++)
        {
            MySquares[i].Value = Globals.Battlefield[i];
        }
    }
    private void ConvertToEnemySquares()
    {
        for (int i = 0; i < Globals.Battlefield.Count; i++)
        {
            EnemySquares[i].Value = Globals.Battlefield[i];
        }
    }

    private void TimeInit()
    {
        if (timer == null) 
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
        }
        else 
        {
            timer.Stop();
            timer.Tick -= Timer_Tick; // Detach the previous event handler
        }

        timer.Tick += Timer_Tick; // Attach the new event handler
        time = TimeSpan.Zero;
        timer.Start();
    }
    private void Timer_Tick(object sender, EventArgs e)
    {
        time = time.Add(TimeSpan.FromSeconds(1));
        TimeToShow =  $"Time: {time.ToString(@"mm\:ss")}";
    }
    public ICommand ClearBoardCommand { get; private set; }

    private void ClearBoard()
    {
        foreach (var square in EnemySquares)
        {
            square.Value = 0;
        }
    }
    public void SquareClick(BattlefieldSquare square)
    {
        if (square.Value == 0)
        {
            if (Globals.MyMove)
            {
                MakeMove(square.X, square.Y);
            }
        }
    }
    private async void MakeMove(int x_, int y_)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",Globals.LogginInUser.access_token);
            string apiUrl = Globals.Url; // This needs to be in file config
            var postData = new { x = x_, y = y_ }; 
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage responsePost = await httpClient.PostAsync($"{apiUrl}/api/Game/MakeMove/{Globals.GameCode}", content);
            if (responsePost.IsSuccessStatusCode)
            {
                var responseContent = await responsePost.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseContent);
                var isAllowed = json.Property("isMoveAllowed").ToObject<bool>();
                List<int> field = json["battlefield"].ToObject<List<int>>();
                var WinnerName = json.Property("winnerName").ToObject<string?>();
                Globals.Battlefield = field;
                Globals.MyMove = isAllowed;
                MyMove = Globals.MyMove ? "Your Move" : "Enemy Move";
                ConvertToEnemySquares();
                if (WinnerName != null)
                {
                    // MessageBox.Show($"Congratulation to {WinnerName}");
                }
                if (!isAllowed)
                {
                    MakeTimer();
                }
            }
            else
            {
                if ((int)responsePost.StatusCode == 400 )
                {
                    Globals.ShowDialog("BAD REQUEST");
                }
                else if ((int)responsePost.StatusCode == 401)
                {
                    Globals.LogginInUser.Logout();
                    Navigation.NavigateTo<HomeViewModel>();
                }
                else
                {
                    Globals.ShowDialog($"{responsePost.StatusCode} {(int)responsePost.StatusCode}");
                }
            }
        }
        
    }
    private async void Surrender()
    {
        timer.Stop();
        TimerForMove.Stop();
        Globals.MyUsername = "";
        Globals.EnemyUsername = "";
        Globals.MyMove = false;
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",Globals.LogginInUser.access_token);
            string apiUrl = Globals.Url;  // This needs to be in file config
            HttpResponseMessage responsePost = await httpClient.DeleteAsync($"{apiUrl}/api/Game/WaitForMove/{Globals.GameCode}");
            if (responsePost.IsSuccessStatusCode)
            {
              //skip
            }
            else
            {
                if ((int)responsePost.StatusCode == 400 )
                {
                    Globals.ShowDialog("BAD REQUEST");
                }
                else if ((int)responsePost.StatusCode == 401)
                {
                    Globals.LogginInUser.Logout();
                    Navigation.NavigateTo<HomeViewModel>();
                }
                else
                {
                    Globals.ShowDialog($"{responsePost.StatusCode} {(int)responsePost.StatusCode}");
                }
            }
        }
        Globals.GameCode = ""; // need to be cleaned after all data have been send to server
        Navigation.NavigateTo<RatingViewModel>(); // Recommended to move this navigation to command, to make method more usable
    }
    private void MakeTimer()
    {
        TimerForMove.Stop(); // Stop any existing timer
        TimerForMove.Interval = new TimeSpan(0, 0, 1);
        
        // Clear existing event handlers
        TimerForMove.Tick -= Waiting; // Remove the handler if it's already there
        TimerForMove.Tick += Waiting; // Add the handler

        TimerForMove.Start();
    }

    private async void Waiting(object? sender, EventArgs eventArgs)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",Globals.LogginInUser.access_token);
            string apiUrl = Globals.Url;
            HttpResponseMessage responsePost = await httpClient.GetAsync($"{apiUrl}/api/Game/WaitForMove/{Globals.GameCode}");
            if ((int)responsePost.StatusCode == 200)
            {
                var responseContent = await responsePost.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseContent);
                var isAllowed = json.Property("isMoveAllowed").ToObject<bool>();
                var WinnerName = json.Property("winnerName").ToObject<string?>();
                IsAllowedToMove = isAllowed;
                List<int> field = json["battlefield"].ToObject<List<int>>();
                Globals.Battlefield = field;
                ConvertToMySquares();
                if (WinnerName != null)
                {
                    Globals.Winner = WinnerName;
                    TimerForMove.Stop(); // Stop the timer immediately
                    Globals.GameCode = "";
                    Navigation.NavigateTo<RatingViewModel>();
                    return;
                }
                if (IsAllowedToMove)
                {
                    TimerForMove.Stop();
                    Globals.MyMove = isAllowed;
                    MyMove = Globals.MyMove ? "Your Move" : "Enemy Move";
                }
            }
            else if ((int)responsePost.StatusCode == 204)
            {
                // skip
            }
            else
            {
                if ((int)responsePost.StatusCode == 400 )
                {
                    Globals.ShowDialog("BAD REQUEST");
                }
                else if ((int)responsePost.StatusCode == 401)
                {
                    Globals.LogginInUser.Logout();
                    Navigation.NavigateTo<HomeViewModel>();
                }
                else
                {
                    var responseContent = await responsePost.Content.ReadAsStringAsync();
                    Globals.ShowDialog($"{responsePost.StatusCode} {(int)responsePost.StatusCode}");
                }
            }
           
        }
    }
   

    private void RunGameView()
    {
        Globals.Winner = "";
        TimeInit();
        if (!Globals.MyMove)
        {
            MakeTimer();
        }
        IsAllowedToMove = Globals.MyMove;
        MyUsername = Globals.MyUsername;
        EnemyUsername = Globals.EnemyUsername;
        MyMove = Globals.MyMove ? "Your Move" : "Enemy Move";
        ConvertToMySquares();
        ClearBoard();
    }

    public ICommand commands { get; set; }
    public ICommand SquareClickCommand { get; set; }
    public GameViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        navigation.Navigating += OnNavigatingGame;
        InitializeSquares();
        commands = new RelayCommand(o => { ConvertToMySquares(); }, canExecute: o => true);
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        CleanerCommand = new RelayCommand(o => { Surrender();}, canExecute: o => true);
        SquareClickCommand =  new RelayCommand(o => { SquareClick(o as BattlefieldSquare);}, canExecute: o => true);
    }
    private void OnNavigatingGame(Core.ViewModel viewModel)
    {
        if (viewModel is GameViewModel)
        {
            RunGameView();
        }
    }
}