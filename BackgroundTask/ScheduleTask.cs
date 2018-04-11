using Laboration3VT2018;
using Laboration3VT2018.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Newtonsoft.Json;
using Windows.UI.Notifications;

namespace ScheduleTask
{
    public sealed class ScheduleTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        Geoposition currentPos;
        List<Route> routes;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            StorageFile sampleFile = null;
            while (sampleFile == null)
            {
                sampleFile = await App.localFolder.GetFileAsync("dataStorage.txt");
            }

            string json = File.ReadAllText(sampleFile.Path);

            routes = JsonConvert.DeserializeObject<List<Route>>(json);
            GetCurrentPosAsync();
            CheckIfCurrentPosIsMonitored(currentPos);
            WriteSpeedAndTimeOfVisitToStorageAsync();
            ToastMessage();


            _deferral.Complete();
            
        }

        private void ToastMessage()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            Windows.Data.Xml.Dom.XmlDocument toastxml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            Windows.Data.Xml.Dom.XmlNodeList toastTextElements = toastxml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastxml.CreateTextNode("You are at a marked position"));
            ToastNotification toast = new ToastNotification(toastxml);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private async void GetCurrentPosAsync()
        {
          

            while (currentPos == null)
            {
                await Task.Run(() => GetCurrentPosition());
            }

        }

        private async void WriteSpeedAndTimeOfVisitToStorageAsync()
        {
            StorageFile sampleFile = null;
            while (sampleFile == null)
            {
                sampleFile = await App.localFolder.GetFileAsync("dataStorage.txt");
            }

            string json = JsonConvert.SerializeObject(routes, Formatting.Indented);

            await FileIO.WriteTextAsync(sampleFile, json);
        }

        private void CheckIfCurrentPosIsMonitored(Geoposition currentPos)
        {
            foreach(var route in routes)
            {
                if(route.MarkedPositions != null)
                {
                    foreach (var pos in route.MarkedPositions)
                    {
                        if (pos.IsMonitoredPosition)
                        {
                            if (pos.Point.Equals(currentPos.Coordinate.Point.Position))
                            {
                                pos.IsVisited = true;
                                pos.VisitedTime = DateTime.Now.Date;
                                pos.VisitSpeed = (float)currentPos.Coordinate.Speed;
                            }
                        }
                    }
                }
                if (route.EndPosition.Point.Equals(currentPos.Coordinate.Point.Position))
                {
                    route.EndPosition.IsVisited = true;
                    route.EndPosition.VisitedTime = DateTime.Now.Date;
                    route.EndPosition.VisitSpeed = (float)currentPos.Coordinate.Speed;
                }
            }
            //open app and open route

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

    }
}
