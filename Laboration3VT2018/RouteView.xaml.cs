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
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    public sealed partial class RouteView : Page
    {
        private Route thisRoute;
        private MapHelper mapHelper;
        private List<Route> routeList;

        public RouteView()
        {
            this.InitializeComponent();
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
            //Application.Current.Resuming += new EventHandler<Object>(App_Resuming);

            routeList = new List<Route>();
            mapHelper = new MapHelper();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var p = e.Parameter as RouteParameters;
            int id = p.ID;
            routeList = p.RouteList;
            while(thisRoute == null)
            {
                thisRoute = routeList.Find(t => t.ID == id);
            }
            

            PopulatePageWithRouteData();
            ShowRouteOnMapAsync();


        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), routeList);
        }

        private void RouteMapControl_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            
            
        }

        private void PopulatePageWithRouteData()
        {
            var len = thisRoute.StartPosition.LengthToEndPoint;
            RouteLenBlock.Text = len.ToString() + " m";
            pageTitle.Text = thisRoute.RouteName;
            StartPointBlock.Text = thisRoute.StartPosition.Point.Latitude.ToString() + " : " + thisRoute.StartPosition.Point.Longitude.ToString();
            EndPointBlock.Text = thisRoute.EndPosition.Point.Latitude.ToString() + " : " + thisRoute.EndPosition.Point.Longitude.ToString();
        }

        private async void ShowRouteOnMapAsync()
        {
            RouteMapControl.ZoomLevel = 13;
            Geopoint startGeopoint = new Geopoint(thisRoute.StartPosition.Point);
            RouteMapControl.Center = startGeopoint;
            var startIcon = mapHelper.AddIconToMap(thisRoute.StartPosition.Point, "Start");
            RouteMapControl.Layers.Add(startIcon);
            var stopIcon = mapHelper.AddIconToMap(thisRoute.EndPosition.Point, "End");
            RouteMapControl.Layers.Add(stopIcon);


            // Get the route between start and end points.
            MapRouteFinderResult routeResult =
                  await MapRouteFinder.GetDrivingRouteAsync(
                  new Geopoint(thisRoute.StartPosition.Point),
                  new Geopoint(thisRoute.EndPosition.Point),
                  MapRouteOptimization.Time,
                  MapRouteRestrictions.None);

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Colors.Yellow;
                viewOfRoute.OutlineColor = Colors.Black;

                RouteMapControl.Routes.Add(viewOfRoute);

                // Fit the MapControl to the route.
                //await RouteMapControl.TrySetViewBoundsAsync(
                //      routeResult.Route.BoundingBox,
                //      null,
                //      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
            }

            if(thisRoute.MarkedPositions != null)
            {
                int index = 0;
                foreach (var pos in thisRoute.MarkedPositions)
                {
                    index++;
                    var icon = mapHelper.AddIconToMap(pos.Point, "Position " + index.ToString());
                    RouteMapControl.Layers.Add(icon);
                }
            }
            
        }

        private async void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            //save data here
            string data = this.BaseUri.LocalPath + Environment.NewLine + thisRoute.ID.ToString();
            StorageFile sampleFile = await App.localFolder.CreateFileAsync("suspensiondata.txt", CreationCollisionOption.OpenIfExists);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await FileIO.WriteTextAsync(sampleFile, json);

        }

        private void App_Resuming(Object sender, Object e)
        {
            // TODO: Refresh network data, perform UI updates, and reacquire resources like cameras, I/O devices, etc.
        }

        private void RouteMapControl_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var geoPoint = args.Location;
            var clickedLat = geoPoint.Position.Latitude;
            clickedLat = Math.Round(clickedLat, 2);
            var clickedLong = geoPoint.Position.Longitude;
            clickedLong = Math.Round(clickedLong, 2);
            if (thisRoute.StartPosition.Point.Latitude.ToString().Contains(clickedLat.ToString()) && thisRoute.StartPosition.Point.Longitude.ToString().Contains(clickedLong.ToString()))
            {
                VisitSpeedBlock.Text = thisRoute.StartPosition.VisitSpeed.ToString();
                VisitTimeBlock.Text = thisRoute.StartPosition.VisitedTime.ToString();
            }
            if (thisRoute.EndPosition.Point.Latitude.ToString().Contains(clickedLat.ToString()) && thisRoute.EndPosition.Point.Longitude.ToString().Contains(clickedLong.ToString()))
            {
                VisitSpeedBlock.Text = thisRoute.EndPosition.VisitSpeed.ToString();
                VisitTimeBlock.Text = thisRoute.EndPosition.VisitedTime.ToString();
            }
            if (thisRoute.MarkedPositions != null)
            {
                foreach (var pos in thisRoute.MarkedPositions)
                {
                    if (pos.Point.Latitude.ToString().Contains(clickedLat.ToString()) && pos.Point.Longitude.ToString().Contains(clickedLong.ToString()))
                    {
                        VisitSpeedBlock.Text = pos.VisitSpeed.ToString();
                        VisitTimeBlock.Text = pos.VisitedTime.ToString();
                    }
                }
            }
        }
    }
}
