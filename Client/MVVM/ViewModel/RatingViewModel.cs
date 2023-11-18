using Client.Core;
using Client.Services;

namespace Client.MVVM.ViewModel;

public class RatingViewModel : Core.ViewModel
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
    // public RelayCommand NavigateToSettingsViewCommand { get; set; }
    public RelayCommand NavigateToHomeCommand { get; set; }
    public RatingViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        // NavigateToLoginViewCommand =
        //     new RelayCommand(o => { Navigation.NavigateTo<LoginViewModel>(); }, canExecute: o => true);
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        //NavigateToSettingsViewCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>();}, canExecute:o => true );
    }
}