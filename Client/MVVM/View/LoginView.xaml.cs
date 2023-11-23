using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Client.MVVM.View;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        using (HttpClient httpClient = new HttpClient())
        {

            string username = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            string apiUrl = "http://localhost:5199";

            var postData = new { username = username, password = password };

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage responsePost = await httpClient.PostAsync($"{apiUrl}/api/Auth/Login", content);

            if (responsePost.IsSuccessStatusCode)
            {
                // Get the content of the response
                string responseContent = await responsePost.Content.ReadAsStringAsync();
                MessageBox.Show("POST Request Response: \n" + responseContent);
            }
            else
            {
                MessageBox.Show($"POST Request failed with status code {(int) responsePost.StatusCode}");
            }
        }
    }
}