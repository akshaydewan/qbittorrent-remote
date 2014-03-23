using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;

namespace PhoneApp1
{
    public partial class Settings : PhoneApplicationPage
    {

        public AuthSettings AuthSettings { get; set; }
        private StorageWrapper storage;

        public Settings()
        {
            InitializeComponent();
            storage = new StorageWrapper(IsolatedStorageSettings.ApplicationSettings);
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            AuthSettings = storage.LoadAuthSettings();
            TextBoxHost.Text = AuthSettings.Host;
            TextBoxPort.Text = AuthSettings.Port.ToString();
            TextBoxUsername.Text = AuthSettings.Username;
            PasswordBox.Password = AuthSettings.Password;
        }

        private void saveAuthSettings(AuthSettings authSettings, StorageWrapper storage)
        {
            storage.SaveAuthSettings(authSettings);
        }
        

        private void DoneButton_Click(object sender, EventArgs e)
        {
            AuthSettings.Host = TextBoxHost.Text;
            int port;
            if(int.TryParse(TextBoxPort.Text, out port))
            {
                AuthSettings.Port = port;
            }
            AuthSettings.Username = TextBoxUsername.Text;
            AuthSettings.Password = PasswordBox.Password;
            saveAuthSettings(AuthSettings, storage);
            NavigationService.GoBack();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }        
    }
}