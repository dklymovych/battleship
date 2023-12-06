using System;
using Client.Core;
using Client.Services;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Client.MVVM.Model;

namespace Client.MVVM.ViewModel;

public class RegisterViewModel : Core.ViewModel
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
    private string _confrimpassword;
    public string ConfrimPassword
    {
        get { return _confrimpassword; }
        set
        {
            _confrimpassword = value;
            OnPropertyChanged("ConfrimPassword");
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
    public RelayCommand NavigateToSettingsViewCommand { get; set; }
    public RelayCommand NavigateToLoginViewCommand { get; set; }
    public RelayCommand NavigateToRegisterViewCommand { get; set; }
    private async void Submit()
    {
        using (HttpClient httpClient = new HttpClient())
        {

            string username = Username;

            string password = Password;
            string confirmedPassword = ConfrimPassword;
           
            if (password != confirmedPassword)
            {
                Globals.ShowDialog("Passwords do not match!");
                return;
            }

            string apiUrl = Globals.Url;  // This needs to be in file config

            var postData = new { username = username, password = password }; // This needs to be in file config

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage responsePost = await httpClient.PostAsync($"{apiUrl}/api/Auth/Register", content);

            if (responsePost.IsSuccessStatusCode)
            {
                Username = "";
                Password = "";
                ConfrimPassword = "";
                Navigation.NavigateTo<LoginViewModel>();
            }
            else
            {
                Globals.ShowDialog("This user is already exist!");
            }
        }
    }
    public ICommand SubmitCommand { get; set; }
    public RegisterViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        NavigateToLoginViewCommand =
            new RelayCommand(o => { Navigation.NavigateTo<LoginViewModel>(); }, canExecute: o => true);
        SubmitCommand = new RelayCommand(o => { Submit(); }, canExecute:o => true); 
    }
}