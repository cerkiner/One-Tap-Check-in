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

namespace OneTapCheckin
{
    public partial class FourSquareSelectedVenues : PhoneApplicationPage
    {
        private string AccessToken { get; set; }
        Response SelectedVenues;
        // Constructor
        public FourSquareSelectedVenues()
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
                        var SelectedVenue = (Venue)nearbyPlaces.SelectedItem;
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
                        nearbyPlaces.ItemsSource = SelectedVenues.venues;
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



        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string guid = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("guid", out guid))
            {
                //guid exists therefore it's a reload, so delete the last entry
                //from the navigation stack
                if (NavigationService.CanGoBack)
                    NavigationService.RemoveBackEntry();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            base.OnBackKeyPress(e);
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FourSquareVenueSearch.xaml", UriKind.RelativeOrAbsolute));
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
                SelectedVenues.venues.Remove((Venue)SelectedVenue);
                IsolatedStorageSettings.ApplicationSettings.Remove("AutoCheckins");
                IsolatedStorageSettings.ApplicationSettings.Add("AutoCheckins", SelectedVenues);
                //MessageBox.Show("Err1or");
                IsolatedStorageSettings.ApplicationSettings.Save();
                //MessageBox.Show("Erro2r"); 
                Random random = new Random();
                NavigationService.Navigate(new Uri("/FourSquareSelectedVenues.xaml?guid=" + random.Next(0, 9999).ToString(), UriKind.Relative));
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