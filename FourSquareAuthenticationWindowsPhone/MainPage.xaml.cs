using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using OneTapCheckin.Resources;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;
using GeofenceUniversalApp;
using OneTapCheckinBackgroundTask;
using RestSharp;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;

namespace OneTapCheckin
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string ClientId = "PT5MAKB0JYPZ0MNBETTAUOLRYNAE0HNM0JCUXQJQNWYXJBEO";
        private string ClientSecret = "NZP1MHSW5W2C1YBXARQRM3GIAMCN2V2254C3DMKLJPQRDYYQ";
        private string AccessToken { get; set; }
        Response SelectedVenues;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += FourSquareAutoCheckin_Loaded;
        }

        private void FourSquareAutoCheckin_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Installed"))
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("AccessToken"))
                {
                    AccessToken = (string)IsolatedStorageSettings.ApplicationSettings["AccessToken"];

                    if (IsolatedStorageSettings.ApplicationSettings.Contains("AutoCheckins"))
                    {
                        //var SelectedVenue = (Venue)nearbyPlaces.SelectedItem;
                        try
                        {
                            IsolatedStorageSettings.ApplicationSettings.TryGetValue<Response>("AutoCheckins", out SelectedVenues);
                            SelectedVenues.venues.ToList<Venue>();
                        }
                        catch
                        {
                            SelectedVenues = new Response();
                            SelectedVenues.venues = new List<Venue>();
                        }
                        //nearbyPlaces.ItemsSource = SelectedVenues.venues;
                    }
                    else
                    {
                        IsolatedStorageSettings.ApplicationSettings.Add("AutoCheckins", new Response());
                        IsolatedStorageSettings.ApplicationSettings.Save();
                        SelectedVenues = new Response();
                        SelectedVenues.venues = new List<Venue>();
                    }
                }
                else
                {
                    NavigationService.Navigate(new Uri("/FourSquareLoginPage.xaml", UriKind.RelativeOrAbsolute));
                }

            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings["Installed"] = 1;
                IsolatedStorageSettings.ApplicationSettings.Add("AutoCheckins", new Response());
                IsolatedStorageSettings.ApplicationSettings.Save();
                NavigationService.Navigate(new Uri("/FourSquareLoginPage.xaml", UriKind.RelativeOrAbsolute));
            }

            output.Text = LocationTask.IsTaskRegistered().ToString();


        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string guid = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("guid", out guid))
            {
                if (NavigationService.CanGoBack)
                    NavigationService.RemoveBackEntry();
            }
           
        }

        /*
        private void nearbyPlaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (nearbyPlaces.SelectedItem == null)
                return;


            if (IsolatedStorageSettings.ApplicationSettings.Contains("AutoCheckins"))
            {
                var SelectedVenue = (Venue)nearbyPlaces.SelectedItem;
                //Query.Text += "," + SelectedVenue.id; 
                Response SelectedVenues;
                try
                {
                    IsolatedStorageSettings.ApplicationSettings.TryGetValue<Response>("AutoCheckins", out SelectedVenues);
                    SelectedVenues.venues.ToList<Venue>();
                }
                catch
                {
                    SelectedVenues = new Response();
                    SelectedVenues.venues = new List<Venue>();
                }
                SelectedVenues.venues.Remove((Venue)SelectedVenue);
                IsolatedStorageSettings.ApplicationSettings.Remove("AutoCheckins");
                IsolatedStorageSettings.ApplicationSettings.Add("AutoCheckins", SelectedVenues);
                //MessageBox.Show("Err1or");
                IsolatedStorageSettings.ApplicationSettings.Save();
                //MessageBox.Show("Erro2r"); 
                Random random = new Random();
                NavigationService.Navigate(new Uri("/MainPage.xaml?guid=" + random.Next(0, 9999).ToString(), UriKind.Relative));
            }
            else
            {
            }
            nearbyPlaces.SelectedItem = null;
        }
        */

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Terminate();
            base.OnBackKeyPress(e);
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FourSquareVenueSearch.xaml", UriKind.RelativeOrAbsolute));
        }

        private void DeSelectButon_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FourSquareSelectedVenues.xaml", UriKind.RelativeOrAbsolute));
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {


            foreach (Venue _Venue in SelectedVenues.venues)
            {

                try
                {
                    GeofenceService.CreateGeofence(_Venue.id.ToString(), double.Parse(_Venue.location.lat.ToString()), double.Parse(_Venue.location.lng.ToString()), double.Parse("150"));

                    //MessageBox.Show(string.Format("Geofence {0} created!", _Venue.id.ToString()));
                    //ProvisionGeofences();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
        
            }

            LocationTask.Register();
            output.Text = LocationTask.IsTaskRegistered().ToString();

        }

        private void DeRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Venue _Venue in SelectedVenues.venues)
            {

                try
                {
                    GeofenceService.RemoveGeofence(_Venue.id.ToString());

                    //MessageBox.Show(string.Format("Geofence {0} Removed!", _Venue.id.ToString()));
                    //ProvisionGeofences();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }


            LocationTask.Unregister();
            output.Text = LocationTask.IsTaskRegistered().ToString();
        }

        private async Task<Geoposition> getCoordinates()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 5;

            try
            {
                return await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                //exception
                return null;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {


            String fsClient = ClientId;
            String fssecret = ClientSecret;
            Geoposition pos = await getCoordinates();

            var client = new RestClient("https://api.foursquare.com/");
            var request = new RestRequest("v2/checkins/add", Method.POST);

            String Venue = "";
            Double temp = 401441296.9999999999;
            foreach(Venue _venue in SelectedVenues.venues){
                double lat1 = pos.Coordinate.Point.Position.Latitude;
                double long1 = pos.Coordinate.Point.Position.Longitude; 
                double lat2 = _venue.location.lat; 
                double long2 = _venue.location.lng;
                double R = 6371;
                double dLat = (lat2 - lat1) * Math.PI / 180;
                double dLon = (long2 - long1) * Math.PI / 180;
                lat1 = lat1 * Math.PI / 180;
                lat2 = lat2 * Math.PI / 180;
                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                double d = R * c;

                if(d < temp){
                    temp = d;
                    Venue = _venue.id.ToString();
                }
            }



            client.Authenticator = new HttpBasicAuthenticator(fsClient, ClientSecret);
            request.AddParameter("client_id", fsClient, ParameterType.GetOrPost);
            request.AddParameter("client_secret", fssecret, ParameterType.GetOrPost);
            request.AddParameter("v", "20140806", ParameterType.GetOrPost);
            request.AddParameter("m", "swarm", ParameterType.GetOrPost);
            request.AddParameter("oauth_token", AccessToken, ParameterType.GetOrPost);
            request.AddParameter("venueId", Venue, ParameterType.GetOrPost);
            

            client.ExecuteAsync(request, response =>
            {
                output.Text = response.Content;
                output.SelectAll();
                MessageBox.Show(response.Content);
            });
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}