using Laboration3VT2018.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Collections.Generic;
using Laboration3VT2018;
using System;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.Background;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Laboration3VT2018
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int index;
        public static List<Route> RouteList;

        public MainPage()
        {
            this.InitializeComponent();
            App.listOfRoutes = new List<Route>();
            AddDataToRoutesListView(App.listOfRoutes);
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RouteList = e.Parameter as List<Route>;
            AddDataToRoutesListView(RouteList);
            RegisterBackgroundTask();
        }

        

        private void RoutesListView_ItemClick(object sender, ItemClickEventArgs e)
        {

            var route = e.ClickedItem as Route;
            index = route.ID;

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

        private async void AddDataToRoutesListView(List<Route> list)
        {
            try
            { 
                if (App.listOfRoutes != null) RoutesListView.ItemsSource = list;

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
            var items = (sender as ListView).Items;
            if (index != 0) index = index - 1; //check if index is 0 to not go out of bound on array
            var route = items[index] as Route;
            var data = new RouteParameters { ID = route.ID, RouteList = RouteList };
            this.Frame.Navigate(typeof(RouteView), data);
        }

        private async void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            //save data here
        }

        private void App_Resuming(Object sender, Object e)
        {
            // TODO: Refresh network data, perform UI updates, and reacquire resources like cameras, I/O devices, etc.
        }

        private async void RegisterBackgroundTask()
        {
            var taskName = "BackgroundTask";
            bool taskRegistered = false;
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if(backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed || backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        taskRegistered = true;
                        break;
                    }
                }
                var builder = new BackgroundTaskBuilder();

                builder.Name = taskName;
                builder.TaskEntryPoint = "ScheduleTask.ScheduleTask";
                builder.SetTrigger(new TimeTrigger(30, false));
                var registration = builder.Register();
                registration.Completed += TaskRegistration_Completed;
            }
        }
        private void TaskRegistration_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            //completed task
        }
    }
}
