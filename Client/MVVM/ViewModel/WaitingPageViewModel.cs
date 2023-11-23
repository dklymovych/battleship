using System.Windows.Input;
using Client.Core;
using Client.Services;

namespace Client.MVVM.ViewModel;

public class WaitingPageViewModel : Core.ViewModel
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
    public RelayCommand CancelCommand { get; private set; }
    public RelayCommand NavigateToHomeCommand { get; set; }
    // public RelayCommand NavigateToSettingsViewCommand { get; set; }

    public WaitingPageViewModel(INavigationService navService)
    {
        Navigation = navService;
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        CancelCommand = new RelayCommand(o => { Navigation.NavigateTo<RatingViewModel>(); }, canExecute: o => true);
        // NavigateToSettingsViewCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>();}, canExecute:o => true );
    }
}