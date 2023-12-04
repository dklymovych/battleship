using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Core;
using Client.MVVM.Model;
using Client.Services;

namespace Client.MVVM.ViewModel;

public class GameViewModel : Core.ViewModel
{
    private INavigationService _navigation;
    private DispatcherTimer timer;
    private TimeSpan time;
    public List<Square> MySquares { get; set; } = new List<Square>();
    public List<Square> EnemySquares { get; set; } = new List<Square>();
    
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
    public RelayCommand NavigateToHomeCommand { get; set; }
    public ICommand CleanerCommand { get; set; }
    private void InitializeSquares()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Square temp = new Square(j, i);
                MySquares.Add(temp);
                EnemySquares.Add(temp);
            }
        }
    }

    private void TimeInit()
    {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
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
        foreach (var square in MySquares)
        {
            square.isShip = false;
            square.isCloseToShip = false;
            square.ChangeColor();
        }
    }

    private void Cleaner()
    {
        timer.Stop();
        Globals.MyUsername = "";
        Globals.EnemyUsername = "";
        Globals.GameCode = ""; // need to be cleaned after all data have been send to server
        Globals.MyMove = false;
        Navigation.NavigateTo<RatingViewModel>(); // Recommended to move this navigation to command, to make method more usable
    }

   

    private void RunView()
    {
        TimeInit();
        MyUsername = Globals.MyUsername;
        EnemyUsername = Globals.EnemyUsername;
        MyMove = Globals.MyMove ? "Your Move" : "Enemy Move";
    }
    public GameViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        navigation.Navigating += OnNavigating;
        InitializeSquares();
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        CleanerCommand = new RelayCommand(o => { Cleaner();}, canExecute: o => true);
    }
    private void OnNavigating(Core.ViewModel viewModel)
    {
        if (viewModel is GameViewModel)
        {
            RunView();
        }
    }
}