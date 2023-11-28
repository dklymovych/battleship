using System.Windows.Input;
using Client.Core;
using Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Client.Core;
using Client.MVVM.Model;
using Client.MVVM.View;
using Client.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client.MVVM.ViewModel;

public class WaitingPageViewModel : Core.ViewModel
{
    System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();

    private string _gameCode;

    public string GameCode
    {
        get => _gameCode;
        set
        {
            _gameCode = value;
            OnPropertyChanged("GameCode");
        }
    }
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

    private async void Cancel()
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",Globals.LogginInUser.access_token);
            string apiUrl = "http://localhost:5199";  // This needs to be in file config
            HttpResponseMessage responsePost = await httpClient.DeleteAsync($"{apiUrl}/api/Game/WaitForGame/{Globals.GameCode}");

            if (responsePost.IsSuccessStatusCode)
            {
                Globals.GameCode = "";
                GameCode = "";
                Navigation.NavigateTo<CreateGameViewModel>(); 
            }
            else
            {
                if ((int)responsePost.StatusCode == 400 )
                {
                    MessageBox.Show("Bad request");
                }
                else
                {
                    var responseContent = await responsePost.Content.ReadAsStringAsync();
                    MessageBox.Show($"{responsePost.StatusCode} {(int)responsePost.StatusCode}");
                }
            }
           
        }
    }

    private void Func()
    {
        Timer.Interval = new TimeSpan(0, 0, 5);
        Timer.Tick += Waiting; // set it up here
        Timer.Start();
        
    }
    private async void Waiting(object? sender, EventArgs eventArgs)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",Globals.LogginInUser.access_token);
            string apiUrl = "http://localhost:5199";  // This needs to be in file config
            HttpResponseMessage responsePost = await httpClient.GetAsync($"{apiUrl}/api/Game/WaitForGame/{Globals.GameCode}");

            if ((int)responsePost.StatusCode == 200)
            {
                Timer.Stop();
                GameCode = "";
                //MessageBox.Show("Good request");
                Navigation.NavigateTo<RatingViewModel>(); 
            }
            else if ((int)responsePost.StatusCode == 204)
            {
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
                    var responseContent = await responsePost.Content.ReadAsStringAsync();
                    MessageBox.Show($"{responsePost.StatusCode} {(int)responsePost.StatusCode}");
                }
            }
           
        }
    }

    
    public WaitingPageViewModel(INavigationService navService)
    {
        Navigation = navService;
        GameCode = Globals.GameCode;
        Func();
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        CancelCommand = new RelayCommand(o => { Cancel(); }, canExecute: o => true);
    }
}