using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;

namespace PhoneApp1
{
    public class StorageWrapper
    {
        private IsolatedStorageSettings _storage;

        public StorageWrapper(IsolatedStorageSettings storage)
        {
            _storage = storage;
        }

        public String LoadIfExists(string propertyName)
        {
            if (_storage.Contains(propertyName))
            {
                return _storage[propertyName] as string;
            }
            else
            {
                return String.Empty;
            }
        }

        public void AddOrReplace(string propertyName, object value)
        {
            if (_storage.Contains(propertyName)) { _storage.Remove(propertyName); }
            _storage.Add(propertyName, value);
        }

        public AuthSettings LoadAuthSettings()
        {
            var AuthSettings = new AuthSettings();
            AuthSettings.Host = LoadIfExists("auth_host");
            String portString = LoadIfExists("auth_port");
            int result;
            if (int.TryParse(portString, out result))
            {
                AuthSettings.Port = result;
            }
            else
            {
                AuthSettings.Port = 8080;
            }
            AuthSettings.Username = LoadIfExists("auth_username");
            AuthSettings.Password = LoadIfExists("auth_password");
            return AuthSettings;
        }

        public void SaveAuthSettings(AuthSettings authSettings)
        {
            AddOrReplace("auth_host", authSettings.Host);
            AddOrReplace("auth_port", authSettings.Port.ToString());
            AddOrReplace("auth_username", authSettings.Username);
            AddOrReplace("auth_password", authSettings.Password);
        }
    }
}
