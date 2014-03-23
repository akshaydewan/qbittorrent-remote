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

namespace PhoneApp1
{
    public class AuthSettings
    {
        public String Host { get; set; }
        public int Port { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }

        public AuthSettings()
        {
            Host = "";
            Port = 0;
            Username = "";
            Password = "";
        }
    }
}
