using Laboration3VT2018.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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

namespace Laboration3VT2018
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        static public List<Route> listOfRoutes;
        static public StorageFolder localFolder;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            listOfRoutes = new List<Route>();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            localFolder = ApplicationData.Current.LocalFolder;

            StorageFile sampleFile = null;
            while (sampleFile == null)
            {
                sampleFile = await localFolder.GetFileAsync("dataStorage.txt");
            }

            string json = File.ReadAllText(sampleFile.Path);

            listOfRoutes = JsonConvert.DeserializeObject<List<Route>>(json);



            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated || e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                // TODO: Populate the UI with the previously saved application data
                //open the app in same window user was in when closing app
                StorageFile file = await localFolder.GetFileAsync("suspensiondata.txt");
                string[] text = File.ReadAllText(file.Path).Split(new[] { "\\r\\n", "\r", "\n" }, StringSplitOptions.None);
                var firstLine = text[0];
                if (firstLine.Contains("RouteView"))
                {
                    rootFrame = new Frame();
                    string resultString = Regex.Match(text[1], @"\d+").Value;
                    rootFrame.Navigate(typeof(RouteView), new RouteParameters { ID = Int32.Parse(resultString), RouteList = listOfRoutes });
                    Window.Current.Content = rootFrame;
                }
                if (firstLine.Contains("NewRouteView"))
                {
                    rootFrame = new Frame();
                    rootFrame.Navigate(typeof(NewRouteView));
                    Window.Current.Content = rootFrame;
                }
                else
                {
                    rootFrame = new Frame();
                    rootFrame.Navigate(typeof(MainPage), listOfRoutes);
                    Window.Current.Content = rootFrame;
                }

            }
            else
            {
                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (rootFrame == null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();

                    rootFrame.NavigationFailed += OnNavigationFailed;

                    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                    {
                        //TODO: Load state from previously suspended application
                    }

                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;
                }

                if (e.PrelaunchActivated == false)
                {
                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        rootFrame.Navigate(typeof(MainPage), listOfRoutes);
                    }
                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            }
        }

            

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

    }
}
