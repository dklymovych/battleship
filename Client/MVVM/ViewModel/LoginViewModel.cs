using Client.Core;
using Client.Services;
using System.Runtime.CompilerServices;

namespace Client.MVVM.ViewModel;

public class LoginViewModel : Core.ViewModel
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
    public RelayCommand NavigateToRatingViewCommand { get; set; }

    public LoginViewModel(INavigationService navigation)
    {
        
        Navigation = navigation;
        //NavigateToLoginViewCommand = new RelayCommand(o =>{Navigation.NavigateTo<LoginViewModel>();}, canExecute:o => true)
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        NavigateToRatingViewCommand = new RelayCommand(o => { Navigation.NavigateTo<RatingViewModel>();}, canExecute:o => true );

        //NavigateToSettingsViewCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>();}, canExecute:o => true );
    }
}