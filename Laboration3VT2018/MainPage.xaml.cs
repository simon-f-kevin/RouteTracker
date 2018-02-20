using Laboration3VT2018.Models;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Laboration3VT2018
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Route> listOfRoutes;

        public MainPage()
        {
            this.InitializeComponent();
            listOfRoutes = new List<Route>();
            AddDataToRoutesListView();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AddDataToRoutesListView();
        }


        private void NewRouteButton_Click(object sender, RoutedEventArgs e)
        {
            //open new route
            this.Frame.Navigate(typeof(NewRouteView));
        }

        private async void Route_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //open saved route
            this.Frame.Navigate(typeof(RouteView));
        }

        private async void AddDataToRoutesListView()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //try
            //{
            //    StorageFile sampleFile = await localFolder.GetFileAsync("dataStorage.xml");
            //    XmlSerializer serializer = new XmlSerializer(typeof(List<Route>));
            //    using (XmlReader reader = XmlReader.Create(sampleFile.Path))
            //    {
            //        listOfRoutes = (List<Route>)serializer.Deserialize(reader);
            //    }

            //    if(listOfRoutes != null) RoutesListView.ItemsSource = listOfRoutes;

            //}
            //catch (Exception)
            //{
            //    // File not found
            //}

            listOfRoutes.Add(new Route
            {
                RouteName = "Test Route",
                StartPosition = new Position(),
                EndPosition = new Position(),
                MarkedPositions = null,
            });

            RoutesListView.ItemsSource = listOfRoutes;
        }
    }
}
