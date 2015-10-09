using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using System.Windows.Input;

namespace OneTapCheckin
{
    public partial class FourSquareVenueSearch : PhoneApplicationPage
    {
        private string ClientId = "PT5MAKB0JYPZ0MNBETTAUOLRYNAE0HNM0JCUXQJQNWYXJBEO";
        private string ClientSecret = "NZP1MHSW5W2C1YBXARQRM3GIAMCN2V2254C3DMKLJPQRDYYQ";
        private string RedirectedUri = "http://c.erkiner.com/projects/autocheckin";
        private string RedirectedUriHost = "c.erkiner.com";
        private string AccessToken { get; set; }
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Geoposition pos;

        public FourSquareVenueSearch()
        {
            InitializeComponent();
            this.Loaded += FourSquareVenueSearch_Loaded;
        }

        void FourSquareVenueSearch_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("AccessToken"))
            {
                AccessToken = (string)IsolatedStorageSettings.ApplicationSettings["AccessToken"];
                getNearbyPlaces();
            }
            else
            {
                NavigationService.Navigate(new Uri("/FourSquareLoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private async Task<Geoposition> getCoordinates()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 5;

            try
            {
                return await geolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                //exception
                return null;
            }
        }

        private async void getNearbyPlaces()
        {
            String fsClient = ClientId;
            String fssecret = ClientSecret;
            if (pos == null)
            {
                pos = await getCoordinates();
            }
            //setCurrentLocation(pos);
            try
            {
                String datatopost = "?client_id=" + fsClient + "&client_secret=" + fssecret + "&v=20130815&m=swarm&intent=checkin&ll=" + pos.Coordinate.Latitude.ToString().Replace(",", ".") + "," + pos.Coordinate.Longitude.ToString().Replace(",", ".") + "&query=" + Query.Text.ToString() + "&oauth_token=" + AccessToken;
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
                                nearbyPlaces.ItemsSource = nearbyVenues.response.venues;
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
                MessageBox.Show("Error"); 
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                getNearbyPlaces();
            }
        }

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
                var dubb = 0;
                foreach (Venue _Venue in SelectedVenues.venues)
                {
                    //MessageBox.Show(_Venue.ToString());
                    if (_Venue.id == SelectedVenue.id)
                    {
                        dubb = 1;
                        //SelectedVenues.venues.Remove((Venue)SelectedVenue);
                    }
                }
                if (dubb != 1)
                {
                    SelectedVenues.venues.Add((Venue)SelectedVenue);
                }
                
                IsolatedStorageSettings.ApplicationSettings.Remove("AutoCheckins");
                IsolatedStorageSettings.ApplicationSettings.Add("AutoCheckins", SelectedVenues);
                //MessageBox.Show("Err1or");
                IsolatedStorageSettings.ApplicationSettings.Save();
                //MessageBox.Show("Erro2r"); 
                NavigationService.Navigate(new Uri("/FourSquareSelectedVenues.xaml", UriKind.Relative));
                
            }
            else
            { 
            }

            /*var AutoCheckins = (string)IsolatedStorageSettings.ApplicationSettings["AutoCheckins"];
            AutoCheckins += "," + SelectedVenue.id;
            IsolatedStorageSettings.ApplicationSettings["AutoCheckins"] = AutoCheckins.Trim().TrimStart(',').TrimEnd(',');

            IsolatedStorageSettings.ApplicationSettings[SelectedVenue.id] = SelectedVenue.name + "," + SelectedVenue.location.lat + "," + SelectedVenue.location.lng;


            IsolatedStorageSettings.ApplicationSettings.Save();
            NavigationService.Navigate(new Uri("/FourSquareSelectedVenues.xaml", UriKind.Relative));
            */
            // Reset selected item to null
            nearbyPlaces.SelectedItem = null;
        }

    }
}