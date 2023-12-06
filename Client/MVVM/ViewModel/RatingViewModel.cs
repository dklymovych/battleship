using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Documents;
using Client.Core;
using Client.MVVM.Model;
using Client.Services;
using Newtonsoft.Json.Linq;

namespace Client.MVVM.ViewModel;

public class RatingViewModel : Core.ViewModel
{
    private INavigationService _navigation;
    private List<Dictionary<string, object>> _scoreboard;
    public List<Dictionary<string, object>>  Scoreboard
    {
        get => _scoreboard;
        set
        {
            _scoreboard = value;
            OnPropertyChanged("Scoreboard");
        } 
    }
    
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

    public void runRating()
    {
        if (!string.IsNullOrEmpty(Globals.Winner))
        {
            Globals.ShowDialog($"Game ended! Congratulation to {Globals.Winner}");
            Globals.Winner = ""; // Reset after showing
        }
        GetRating();
    }

    public async void GetRating()
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",Globals.LogginInUser.access_token);
            string apiUrl = Globals.Url; // This needs to be in file config
            HttpResponseMessage responsePost = await httpClient.GetAsync($"{apiUrl}/api/Game/Scoreboard");
            if ((int)responsePost.StatusCode == 200)
            {
                var responseContent = await responsePost.Content.ReadAsStringAsync();
                // MessageBox.Show(responseContent);
                // // Console.WriteLine("Received JSON: " + responseContent); // Log the raw JSON
                var json = JObject.Parse(responseContent);
                List<Dictionary<string, object>> scoreboard = json.Property("scoreboard").Value.ToObject<List<Dictionary<string, object>>>();
                Scoreboard = scoreboard;
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
    public RatingViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        navigation.Navigating += OnNavigatingRating;
        // NavigateToLoginViewCommand =
        //     new RelayCommand(o => { Navigation.NavigateTo<LoginViewModel>(); }, canExecute: o => true);
        NavigateToHomeCommand = new RelayCommand(o => { Navigation.NavigateTo<HomeViewModel>();}, canExecute:o => true );
        //NavigateToSettingsViewCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>();}, canExecute:o => true );
    }
    private void OnNavigatingRating(Core.ViewModel viewModel)
    {
        if (viewModel is RatingViewModel)
        {
            runRating();
        }
    }
}