using Laboration3VT2018.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Laboration3VT2018
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewRouteView : Page
    {
        Geoposition currentPos;
        Geoposition startPosition;
        Geoposition endPosition;

        public NewRouteView()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Specify a known location.
            //GetCurrentPosition();

            RouteMapControl.Center = new Geopoint(new BasicGeoposition { Latitude = 57.72103500, Longitude = 12.93981900 });
            RouteMapControl.ZoomLevel = 12;
            RouteMapControl.LandmarksVisible = true;


            // Set the map location.
            //RouteMapControl.Center = currentPos.Coordinate.Point;
            //RouteMapControl.ZoomLevel = 12;
            //RouteMapControl.LandmarksVisible = true;
        }

        private async void GetCurrentPosition()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 10 };

                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    currentPos = pos;
                    break;

                case GeolocationAccessStatus.Denied:
                    //coordinatesBlock.Text = "Location request denied.";
                    break;

                case GeolocationAccessStatus.Unspecified:
                    //coordinatesBlock.Text = "Unspecified error.";
                    break;
            }
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile sampleFile;
            Route newRoute = new Route()
            {
                RouteName = RouteNameBox.Text,

            };
            App.listOfRoutes.Add(newRoute);
            string json = JsonConvert.SerializeObject(App.listOfRoutes, Formatting.Indented);

            sampleFile  = await App.localFolder.CreateFileAsync("dataStorage.txt", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(sampleFile, json);

            Frame.Navigate(typeof(MainPage));
        }

    }
}
