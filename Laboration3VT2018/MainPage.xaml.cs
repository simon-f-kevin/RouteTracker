using Laboration3VT2018.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Laboration3VT2018
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int index;

        public MainPage()
        {
            this.InitializeComponent();
            App.listOfRoutes = new List<Route>();
            AddDataToRoutesListView();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AddDataToRoutesListView();
        }

        private void RoutesListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            var items = (sender as ListView).Items;
            var route = items[index] as Route;
            var id = route.ID;
            var data = new RouteParameters { ID = id };
            this.Frame.Navigate(typeof(RouteView), data);
            
        }


        private void NewRouteButton_Click(object sender, RoutedEventArgs e)
        {
            //open new route
            this.Frame.Navigate(typeof(NewRouteView));
        }

        private void Route_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //open saved route
            var route = sender as Route;
            
            this.Frame.Navigate(typeof(RouteView));
        }

        private async void AddDataToRoutesListView()
        {
            
            try
            {
                StorageFile sampleFile = await App.localFolder.GetFileAsync("dataStorage.txt");
                string json = File.ReadAllText(sampleFile.Path);

                App.listOfRoutes = JsonConvert.DeserializeObject<List<Route>>(json);

                if (App.listOfRoutes != null) RoutesListView.ItemsSource = App.listOfRoutes;

            }
            catch (Exception ex)
            {
                var strings = ex.Message;
                var errorList = new List<Route>();
                errorList.Add(new Route { RouteName = strings });
                RoutesListView.ItemsSource = errorList;
            }


        }

        private void RoutesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            index = RoutesListView.SelectedIndex;
        }

    }
}
