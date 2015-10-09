using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO.IsolatedStorage;

namespace OneTapCheckin
{
    public partial class FourSquareLoginPage : PhoneApplicationPage
    {
        private string ClientId = "PT5MAKB0JYPZ0MNBETTAUOLRYNAE0HNM0JCUXQJQNWYXJBEO";
        private string ClientSecret = "NZP1MHSW5W2C1YBXARQRM3GIAMCN2V2254C3DMKLJPQRDYYQ";
        private string RedirectedUri = "http://c.erkiner.com/projects/autocheckin";
        private string RedirectedUriHost = "c.erkiner.com";
        private string AccessToken { get; set; }
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
      
        public FourSquareLoginPage()
        {
            InitializeComponent();

            this.Loaded += FourSquareLoginPage_Loaded;
        }

        void FourSquareLoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            var url = "https://foursquare.com/oauth2/authorize?display=touch&client_id=" + ClientId + "&client_secret=" + ClientSecret + "&response_type=token&redirect_uri=" + RedirectedUri;
            WebBrowserLogin.Navigate(new Uri(url, UriKind.RelativeOrAbsolute));
            WebBrowserLogin.NavigationFailed += WebBrowserLogin_NavigationFailed;
            WebBrowserLogin.Navigating += WebBrowserLogin_Navigating;
           
        }

        void WebBrowserLogin_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.IsAbsoluteUri)
            {
                if (e.Uri.Host.Equals(RedirectedUriHost))
                {
                  
                    string responseString = e.Uri.ToString();                 
                    string[] splitAccessToken = responseString.Split(new Char[] { '#', '=' });
                    string accessTokenString = splitAccessToken.GetValue(1).ToString();
                    string accessTokenValue = splitAccessToken.GetValue(2).ToString();
                    if (accessTokenString == "access_token")
                    {
                        AccessToken = accessTokenValue;

                        if (AccessToken != null)
                        {

                            localSettings.Values["AccessToken"] = AccessToken;
                           
                            IsolatedStorageSettings.ApplicationSettings["AccessToken"] = AccessToken;
                            IsolatedStorageSettings.ApplicationSettings.Save();
                        }
                        Random random = new Random();
                        NavigationService.Navigate(new Uri("/MainPage.xaml?guid=" + random.Next(0, 9999).ToString(), UriKind.Relative));
                    }
                    else
                        MessageBox.Show("Authentication Failed!");

                    WebBrowserLogin.Visibility = System.Windows.Visibility.Collapsed; 
                }
            }
           
        }

        void WebBrowserLogin_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            MessageBox.Show("Navigation Failed"); 
        }
    }
}