using System;
using System.Windows.Input;
using Client.Core;
using Client.MVVM.Model;
using Client.Services;

namespace Client.MVVM.ViewModel;

public class MainViewModel: Core.ViewModel
{
    private INavigationService _navigation;
   
    
    public INavigationService Navigation
    {
        get => _navigation;
        set
        {
            _navigation = value;
            OnPropertyChanged();
        }
    }
    
    public RelayCommand NavigateToHomeCommand { get; set; }
    public RelayCommand NavigateToSettingsViewCommand { get; set; }
    
    public RelayCommand NavigateToRatingViewCommand { get; set; }
    public RelayCommand NavigateToCreateGameViewCommand { get; set; }
    public RelayCommand NavigateToJoinGameViewCommand { get; set; }


    public ICommand LogOut { get; set; }

    private void Logout()
    {
        Globals.LogginInUser.access_token = "";
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
    }

    public MainViewModel(INavigationService navService)
    {
        Navigation = navService;
        Navigation.NavigateTo<HomeViewModel>(); // За замовчуванням стає HomeView
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        NavigateToSettingsViewCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>();}, canExecute:o => true );
        NavigateToRatingViewCommand = new RelayCommand(o => { Navigation.NavigateTo<RatingViewModel>();}, canExecute:o => true );
        NavigateToCreateGameViewCommand = new RelayCommand(o => { Navigation.NavigateTo<CreateGameViewModel>();}, canExecute:o => true );
        NavigateToJoinGameViewCommand = new RelayCommand(o => { Navigation.NavigateTo<JoinGameViewModel>();}, canExecute:o => true );
        LogOut = new RelayCommand(o => { Logout();}, canExecute:o => true );

    }
}