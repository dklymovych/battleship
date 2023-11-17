using Client.Core;
using Client.Services;

namespace Client.MVVM.ViewModel;

public class HomeViewModel : Core.ViewModel
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
    public RelayCommand NavigateToSettingsViewCommand { get; set; }
    public RelayCommand NavigateToRegisterViewCommand { get; set; }
    public RelayCommand NavigateToLoginViewCommand { get; set; }

    public HomeViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        // NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        NavigateToSettingsViewCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>();}, canExecute:o => true );
        NavigateToRegisterViewCommand = new RelayCommand(o => { Navigation.NavigateTo<RegisterViewModel>();}, canExecute:o => true );
        NavigateToLoginViewCommand = new RelayCommand(o => { Navigation.NavigateTo<LoginViewModel>();}, canExecute:o => true );

    }
}