using System;
using System.Collections.Generic;
using Client.Core;
using Client.Services;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System;
using Client.MVVM.Model;
using Client.MVVM.View;
using Newtonsoft.Json.Linq;

namespace Client.MVVM.ViewModel;

public class LoginViewModel : Core.ViewModel
{
    private string _username;
    public string Username
    {
        get { return _username; }
        set
        {
            _username = value;
            OnPropertyChanged("Username");
        }
    }

    private string _password;
    public string Password
    {
        get { return _password; }
        set
        {
            _password = value;
            OnPropertyChanged("Password");
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
    public RelayCommand NavigateToHomeCommand { get; set; }
    public RelayCommand NavigateToRatingViewCommand { get; set; }
    public ICommand SubmitCommand { get; set; }


    
    private async void Submit()
    {
        using (HttpClient httpClient = new HttpClient())
        {

            string username = Username;
            string password = Password;
            if (username == "" || password == "")
            {
                //MessageBox.Show("Fields are empty");
                Globals.ShowDialog("Fields are empty");
                return;
            }
           

            string apiUrl = Globals.Url;  // This needs to be in file config

            var postData = new { username = username, password = password };  // This needs to be in file config

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage responsePost = await httpClient.PostAsync($"{apiUrl}/api/Auth/Login", content);

            if (responsePost.IsSuccessStatusCode)
            {
                //To get json
                var responseContent = await responsePost.Content.ReadAsStringAsync();
                var token = JObject.Parse(responseContent)["accessToken"]!.ToString();
                User user = new User(token);
                Globals.LogginInUser = user;
                Username = "";
                Password = "";
                Navigation.NavigateTo<RatingViewModel>();
            }
            else
            {
                if ((int)responsePost.StatusCode == 400 )
                {
                    Globals.ShowDialog("Bad request");
                }
                if ((int)responsePost.StatusCode == 401)
                {
                    Globals.ShowDialog("Username or password are incorrect!");
                }
            }
           
        }
    }

    public RelayCommand NavigateToRegisterViewCommand { get; set; }

    public LoginViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        NavigateToRatingViewCommand = new RelayCommand(o => { Navigation.NavigateTo<RatingViewModel>();}, canExecute:o => true );
        NavigateToRegisterViewCommand = new RelayCommand(o => { Navigation.NavigateTo<RegisterViewModel>();}, canExecute:o => true );
        SubmitCommand = new RelayCommand(o => { Submit(); }, canExecute:o => true); 
    }
}