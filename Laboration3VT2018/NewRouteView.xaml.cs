using Laboration3VT2018.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
        Position startPosition;
        Position endPosition;
        List<Position> markedPositions;
        MapHelper mapHelper;

        ContentDialogResult result;

        public NewRouteView()
        {
            this.InitializeComponent();
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
            //Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            mapHelper = new MapHelper();
            // Specify a known location.
            while(currentPos == null) await Task.Run(() => GetCurrentPosition());

            RouteMapControl.Center = currentPos.Coordinate.Point;
            RouteMapControl.ZoomLevel = 12;
            RouteMapControl.LandmarksVisible = true;

        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), MainPage.RouteList);
        }

        private async void RouteMapControl_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            var geoPoint = args.Location;

            MapRouteFinderResult routeToEndPoint;

            if (startPosition == null)
            {
                startPosition = new Position();
                startPosition.Point = geoPoint.Position;

                if (currentPos.Coordinate.Point == geoPoint)
                {
                    startPosition.IsVisited = true;
                    startPosition.VisitedTime = DateTime.Now.Date;
                    startPosition.VisitSpeed = (float)currentPos.Coordinate.Speed;
                }
                var icon = mapHelper.AddIconToMap(startPosition.Point, "Start");
                RouteMapControl.Layers.Add(icon);
                RouteMapControl.Center = new Geopoint(startPosition.Point);
                return;
            }

            if (endPosition == null && startPosition != null)
            {
                endPosition = new Position();
                endPosition.Point = geoPoint.Position;
                routeToEndPoint = await mapHelper.GetRouteToPoint(new BasicGeoposition
                {
                    Longitude = geoPoint.Position.Longitude,
                    Latitude = geoPoint.Position.Latitude
                }, new BasicGeoposition
                {
                    Longitude = endPosition.Point.Longitude,
                    Latitude = endPosition.Point.Latitude
                });
                endPosition.LengthToEndPoint = routeToEndPoint.Route.LengthInMeters;
                endPosition.IsMonitoredPosition = true;

                //set startpoint route to endpoint
                var routeFromStart = await mapHelper.GetRouteToPoint(new BasicGeoposition
                {
                    Longitude = startPosition.Point.Longitude,
                    Latitude = startPosition.Point.Latitude
                }, new BasicGeoposition
                {
                    Longitude = endPosition.Point.Longitude,
                    Latitude = endPosition.Point.Latitude
                });
                startPosition.LengthToEndPoint = routeFromStart.Route.LengthInMeters;
                var icon = mapHelper.AddIconToMap(endPosition.Point, "End");
                RouteMapControl.Layers.Add(icon);

                RouteMapControl.Center = new Geopoint(endPosition.Point);

                return;
            }

            if (startPosition != null && endPosition != null)
            {
                markedPositions = new List<Position>();
                routeToEndPoint = await mapHelper.GetRouteToPoint(new BasicGeoposition
                {
                    Longitude = geoPoint.Position.Longitude,
                    Latitude = geoPoint.Position.Latitude
                }, new BasicGeoposition
                {
                    Longitude = endPosition.Point.Longitude,
                    Latitude = endPosition.Point.Latitude
                });
                var newPos = new Position()
                {
                    Point = geoPoint.Position,
                    IsMonitoredPosition = false,
                    IsVisited = false,
                    LengthToEndPoint = routeToEndPoint.Route.LengthInMeters
                };

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    DisplayMonitoredPositionDialog();
                });


                if (result == ContentDialogResult.Primary)
                {
                    newPos.IsMonitoredPosition = true;
                }

                markedPositions.Add(newPos);
                var icon = mapHelper.AddIconToMap(newPos.Point, "");

                RouteMapControl.Layers.Add(icon);
                RouteMapControl.Center = new Geopoint(newPos.Point);
            }


        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
            List<Route> tempListOfRoutes = new List<Route>();
            StorageFile sampleFile = await App.localFolder.CreateFileAsync("dataStorage.txt", CreationCollisionOption.OpenIfExists);

            Route newRoute = new Route()
            {
                ID = MainPage.RouteList.Count + 1,
                RouteName = RouteNameBox.Text,
                StartPosition = startPosition,
                EndPosition = endPosition,
                MarkedPositions = markedPositions,
               
            };

            string oldJson = await FileIO.ReadTextAsync(sampleFile);
            //deserialiaze existing data and append new route to old route-list
            
            if (oldJson.Equals(string.Empty))
            {
                tempListOfRoutes.Add(newRoute);
                App.listOfRoutes = tempListOfRoutes;
            }
            else
            {
                tempListOfRoutes = JsonConvert.DeserializeObject<List<Route>>(oldJson);
                tempListOfRoutes.Add(newRoute);
                App.listOfRoutes = tempListOfRoutes;
            }
            
            
            //serialize the data again before withing it to the file
            string json = JsonConvert.SerializeObject(App.listOfRoutes, Formatting.Indented);

            await FileIO.WriteTextAsync(sampleFile, json);

            Frame.Navigate(typeof(MainPage), App.listOfRoutes);
        }

        private async void GetCurrentPosition()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            try
            {
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:

                        Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 10 };

                        //Geoposition pos = await geolocator.GetGeopositionAsync();
                        var nativeObject = new LocationComponent.GetLocation();
                        var position = await nativeObject.ReturnPosition();
                        currentPos = position;
                        break;

                    case GeolocationAccessStatus.Denied:
                        //coordinatesBlock.Text = "Location request denied.";
                        break;

                    case GeolocationAccessStatus.Unspecified:
                        //coordinatesBlock.Text = "Unspecified error.";
                        break;
                }
            }
            catch (Exception)
            {
                //handle exceptions here
            }
            
        }

        private async void DisplayMonitoredPositionDialog()
        {
            ContentDialog monitoredPositionDialog = new ContentDialog
            {
                Title = "Make this point monitored?",
                Content = "You will be notified when reaching this position.",
                CloseButtonText = "No",
                PrimaryButtonText = "Yes"
            };

             result = await monitoredPositionDialog.ShowAsync();
        }

        private async void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            //save data here
            string data = this.BaseUri.LocalPath;
            StorageFile sampleFile = await App.localFolder.CreateFileAsync("suspensiondata.txt", CreationCollisionOption.OpenIfExists);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await FileIO.WriteTextAsync(sampleFile, json);
        }

        //private async void App_Resuming(Object sender, Object e)
        //{
        //    // TODO: Refresh network data, perform UI updates, and reacquire resources like cameras, I/O devices, etc.
        //    StorageFile sampleFile = await App.localFolder.GetFileAsync("suspensiondata.txt");
        //    string json = File.ReadAllText(sampleFile.Path);
        //    Route tempRoute = JsonConvert.DeserializeObject<Route>(json);
        //    RouteNameBox.Text = tempRoute.RouteName;
        //    startPosition = tempRoute.StartPosition;
        //    endPosition = tempRoute.EndPosition;
        //    markedPositions = tempRoute.MarkedPositions;
        //}


    }
}
