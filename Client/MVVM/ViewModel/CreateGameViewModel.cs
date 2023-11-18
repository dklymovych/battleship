using Client.Core;
using Client.Services;

namespace Client.MVVM.ViewModel;

public class CreateGameViewModel : Core.ViewModel
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



    public CreateGameViewModel(INavigationService navService)
    {
        Navigation = navService;
        // NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        // NavigateToSettingsViewCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>();}, canExecute:o => true );
        // NavigateToRatingViewCommand = new RelayCommand(o => { Navigation.NavigateTo<RatingViewModel>();}, canExecute:o => true );
        // NavigateToCreateGameViewCommand = new RelayCommand(o => { Navigation.NavigateTo<CreateGameViewModel>();}, canExecute:o => true );


    }
}