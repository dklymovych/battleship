using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Configuration;

namespace battleship_my
{
    public partial class MainWindow : Window
    {
        Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private string username = "admin";
        private string password = "root";

        private bool loggedIn = false;

        public MainWindow()
        {
            InitializeComponent();

            var jwtSetting = AppConfig.GetSection("JWTSetting") as JWTSetting;
            if (jwtSetting == null)
            {
                AppConfig.Sections.Add("JWTSetting", new JWTSetting());
            }
            else
            {
                if (jwtSetting.JWT != String.Empty)
                {
                    loggedIn = true;
                    MessageBox.Show("Fetched local JWT token");
                }
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredUsername = usernameTextBox.Text;
            string enteredPassword = passwordTextBox.Text;

            if (loggedIn)
            {
                MessageBox.Show("Already logged in");
            }

            else if (enteredUsername == username && enteredPassword == password)
            {
                var jwtSetting = AppConfig.GetSection("JWTSetting") as JWTSetting;
                if (jwtSetting != null)
                {
                    jwtSetting.JWT = "1234";
                    AppConfig.Save();
                    loggedIn = true;
                    MessageBox.Show("Login successful");
                }
            }
            else
            {
                MessageBox.Show("Wrong username/password");
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!loggedIn)
            {
                MessageBox.Show("Not logged in");
            }
            else
            {
                var jwtSetting = AppConfig.GetSection("JWTSetting") as JWTSetting;
                if (jwtSetting != null)
                {
                    jwtSetting.JWT = string.Empty;
                    AppConfig.Save();
                    loggedIn = false;
                    MessageBox.Show("Logged out successfully");
                }
            }
        }
    }
}
