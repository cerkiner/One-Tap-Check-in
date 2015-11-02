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
using System.IO;
using System.Text;

namespace OneTapCheckin
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string ClientId = "PT5MAKB0JYPZ0MNBETTAUOLRYNAE0HNM0JCUXQJQNWYXJBEO";
        private string ClientSecret = "NZP1MHSW5W2C1YBXARQRM3GIAMCN2V2254C3DMKLJPQRDYYQ";
        private string AccessToken { get; set; }
        Response SelectedVenues;
        Response Venues;
        Geoposition pos = null;

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

            //output.Text = LocationTask.IsTaskRegistered().ToString();


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

        private async Task getCoordinates()
        {

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 5;
            geolocator.MovementThreshold = 5;
            geolocator.ReportInterval = 500;

            try
            {
                output.Text = "Locating";
                pos = await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(1), timeout: TimeSpan.FromSeconds(5)); // crashes sometimes???? get an idea !!!
            }
            catch (Exception ex)
            {
                //exception
                output.Text = "Error Locating";
                MessageBox.Show("Error "+ex);
            }
        }

        private async Task getNearbyPlaces()
        {
            String fsClient = ClientId;
            String fssecret = ClientSecret;
            if (pos == null)
            {
                await getCoordinates();
            }
            //setCurrentLocation(pos);
            try
            {
                output.Text = "Getting Venues";
                String datatopost = "?client_id=" + fsClient + "&client_secret=" + fssecret + "&v=20130815&m=swarm&intent=checkin&ll=" + pos.Coordinate.Point.Position.Latitude.ToString().Replace(",", ".") + "," + pos.Coordinate.Point.Position.Longitude.ToString().Replace(",", ".") + "&query=&oauth_token=" + AccessToken;
                Uri address = new Uri("https://api.foursquare.com/v2/venues/search" + datatopost);

                //Query.Text = address.ToString();
                // Create the web request

                //MessageBox.Show(address.ToString()); 
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                // Set type to POST
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                // Create the data we want to send
                request.Accept = "application/json";
                // Get response

                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //To obtain response body
                        using (Stream streamResponse = response.GetResponseStream())
                        {
                            using (StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8))
                            {
                                RootObject nearbyVenues = JsonConvert.DeserializeObject<RootObject>(streamRead.ReadToEnd().ToString());

                                //foreach (Venue _venue in nearbyVenues.response.venues) // to add offset to nearby places because i wanted to promote whitelist if they are in certain distance.
                                //{
                                //    double lat1 = pos.Coordinate.Point.Position.Latitude;
                                //    double long1 = pos.Coordinate.Point.Position.Longitude;
                                //    double offset = 25; // meter
                                //    double R = 6378137; // earth's something in METERS
                                //    double dLat = offset / R;
                                //    double dLon = offset / (R * Math.Cos(Math.PI * lat1 / 180));
                                //    _venue.location.lat = lat1 + dLat * (180 / Math.PI);
                                //    _venue.location.lng = long1 + dLon * (180 / Math.PI);
                                //}

                                Venues.venues.AddRange(nearbyVenues.response.venues);
                            }
                        }
                    }
                }
                /*

                   var client = new RestClient();
                   client.BaseUrl = new Uri( "https://api.foursquare.com/");

                   var request = new RestRequest();
                   request.Resource = "v2/venues/search";

                   client.Authenticator = new HttpBasicAuthenticator(fsClient, ClientSecret);
                   request.AddParameter("client_id", fsClient, ParameterType.UrlSegment);
                   request.AddParameter("client_secret", fssecret, ParameterType.UrlSegment);
                   request.AddParameter("ll", pos.Coordinate.Latitude+","+pos.Coordinate.Longitude, ParameterType.UrlSegment);
                   request.AddParameter("v","20140806" , ParameterType.UrlSegment);
                   request.AddParameter("m","swarm" , ParameterType.UrlSegment);
                   request.AddParameter("oauth_token", AccessToken, ParameterType.UrlSegment);

                   var asyncHandle = client.ExecuteAsync<RootObject>(request, response =>
                   {
                       nearbyPlaces.ItemsSource = response.Data.response.venues;
                       //Console.WriteLine(response.Data.Name);
                   });
                   //RestResponse<RootObject> response2 = client.ExecuteAsync<RootObject>(request);
                   //RestResponse response = client.Execute(request);
                   */
            }
            catch (Exception ex)
            {
                output.Text = "Error Getting Venues";
                MessageBox.Show("Error " + ex);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {


            String fsClient = ClientId;
            String fssecret = ClientSecret;

            Venues = new Response();
            Venues.venues = new List<Venue>();

            var client = new RestClient("https://api.foursquare.com/");
            var request = new RestRequest("v2/checkins/add", Method.POST);



            await getCoordinates();

            try
            {

                output.Text = "Checking For Selected Venues";
                String Venue = "";
                Double temp = 401441296.9999999999;
                foreach (Venue _venue in SelectedVenues.venues)
                {
                    double lat1 = pos.Coordinate.Point.Position.Latitude;
                    double long1 = pos.Coordinate.Point.Position.Longitude;
                    double lat2 = _venue.location.lat;
                    double long2 = _venue.location.lng;
                    double R = 6378137; // earth's something in meters
                    double dLat = (lat2 - lat1) * Math.PI / 180;
                    double dLon = (long2 - long1) * Math.PI / 180;
                    lat1 = lat1 * Math.PI / 180;
                    lat2 = lat2 * Math.PI / 180;
                    double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
                    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                    double d = R * c;

                    if (d < temp)
                    {
                        temp = d;
                        Venue = _venue.id.ToString(); // trying to get the closest place id among whitelist and offseted nearby places
                        output.Text = "Checked in to " + _venue.name.ToString() + " (" + temp.ToString() + ")";
                        //MessageBox.Show(_venue.name.ToString() + " (" + temp.ToString() + ")");
                    }
                }

                if (temp > 25) { // if none of the selected venues in the range of 25 meters
                    await getNearbyPlaces();
                    output.Text = "Checking For Nearby Venues";
                    foreach (Venue _venue in Venues.venues)
                    {
                        double lat1 = pos.Coordinate.Point.Position.Latitude;
                        double long1 = pos.Coordinate.Point.Position.Longitude;
                        double lat2 = _venue.location.lat;
                        double long2 = _venue.location.lng;
                        double R = 6378137; // earth's something in kilometers
                        double dLat = (lat2 - lat1) * Math.PI / 180;
                        double dLon = (long2 - long1) * Math.PI / 180;
                        lat1 = lat1 * Math.PI / 180;
                        lat2 = lat2 * Math.PI / 180;
                        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
                        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                        double d = R * c;

                        if (d < temp)
                        {
                            temp = d;
                            Venue = _venue.id.ToString(); // trying to get the closest place id among whitelist and offseted nearby places
                            output.Text = "Checked in to " + _venue.name.ToString() + " (" + temp.ToString() + ")";
                        }
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
                    Venues.venues.Clear();
                    //MessageBox.Show(response.Content);
                });

            }
            catch (Exception ex)
            {
                output.Text = "Error Checking In";
                MessageBox.Show("Error " + ex);
            }
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