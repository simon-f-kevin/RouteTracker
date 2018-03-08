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
        private int mainViewId;
        private string RouteLengthString;
        private string RouteName;
        private string StartPointCoords;
        private string EndPointCoords;
        private Route thisRoute;
        private MapHelper mapHelper;

        public RouteView()
        {
            this.InitializeComponent();
            mapHelper = new MapHelper();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var p = e.Parameter as RouteParameters;
            int id = p.ID;
            while(thisRoute == null)
            {
                thisRoute = App.listOfRoutes.Find(t => t.ID == id);
            }
            

            PopulatePageWithRouteData();
            ShowRouteOnMapAsync();


        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
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
    }
}
