using System;
using Client.Core;
using Client.MVVM.ViewModel;

namespace Client.Services;


public interface INavigationService
{
    public event Action<ViewModel> Navigating;
    ViewModel CurrentView { get; }
    void NavigateTo<T>() where T : ViewModel;
}
public class NavigationService : ObservableObject, INavigationService
{
    private readonly Func<Type, ViewModel> _viewModelFactory;
    private ViewModel _currentView;
    public event Action<ViewModel> Navigating;
    public ViewModel CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public NavigationService(Func<Type, ViewModel> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModel
    {
        ViewModel viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
        Navigating?.Invoke(viewModel);
        CurrentView = viewModel;
    }
}


