using Laboration3VT2018.Models;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.ViewManagement;

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
            listOfRoutes.Add(new Route
            {
                RouteName = "Test Route",
                StartPosition = new Position(),
                EndPosition = new Position(),
                MarkedPositions = null,
            });

            RoutesListView.ItemsSource = listOfRoutes;
        }


        private void NewRouteButton_Click(object sender, RoutedEventArgs e)
        {
            //open new route
        }

        private async void Route_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //open saved route
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(RouteView), null);
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }
    }
}
