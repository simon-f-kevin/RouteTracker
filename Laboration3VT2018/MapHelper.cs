using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;

namespace Laboration3VT2018
{
    public class MapHelper
    {
        public MapHelper()
        {

        }

        public async Task<MapRouteView> GetRouteViewForMap(BasicGeoposition startPos, BasicGeoposition endPos)
        {
            // Get the route between the points.
            MapRouteFinderResult routeResult =
                  await MapRouteFinder.GetDrivingRouteAsync(
                  new Geopoint(startPos),
                  new Geopoint(endPos),
                  MapRouteOptimization.Time,
                  MapRouteRestrictions.None);

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Colors.Yellow;
                viewOfRoute.OutlineColor = Colors.Black;


                return viewOfRoute;
                //// Add the new MapRouteView to the Routes collection
                //// of the MapControl.
                //MapWithRoute.Routes.Add(viewOfRoute);

                //// Fit the MapControl to the route.
                //await MapWithRoute.TrySetViewBoundsAsync(
                //      routeResult.Route.BoundingBox,
                //      null,
                //      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
            }
            return null;
        }

        public MapElementsLayer AddIconToMap(BasicGeoposition inputPosition, string name)
        {
            var MyLandmarks = new List<MapElement>();

            Geopoint newPoint = new Geopoint(inputPosition);

            var Icon = new MapIcon
            {
                Location = newPoint,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0,
                Title = name
            };

            MyLandmarks.Add(Icon);

            var LandmarksLayer = new MapElementsLayer
            {
                ZIndex = 1,
                MapElements = MyLandmarks
            };

            return LandmarksLayer;


        }

        public async Task<MapRouteFinderResult> GetRouteToPoint(BasicGeoposition start, BasicGeoposition end)
        {
            return await MapRouteFinder.GetDrivingRouteAsync(
                                                    new Geopoint(start),
                                                    new Geopoint(end),
                                                    MapRouteOptimization.Time,
                                                    MapRouteRestrictions.None);
        }
    }
}
