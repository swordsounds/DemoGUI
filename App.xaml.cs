using DisplayAnOfflineMapOnDemand.ViewModel;
using Esri.ArcGISRuntime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DisplayAnOfflineMapOnDemand
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                
                DataContext = new MainViewModel() // Set the DataContext to the MainWindowViewModel
            };
            MainWindow.Show();

            base.OnStartup(e);

            // !!MUST OMIT THESE TO NOT LEAK API KEY!!
            // Set the access token for ArcGIS Maps SDK for .NET.
            ArcGISRuntimeEnvironment.ApiKey = ;
            // Set license key
            string licenseKey = ;
            ArcGISRuntimeEnvironment.SetLicense(licenseKey);

            // Call a function to set up the AuthenticationManager for OAuth.
            UserAuth.ArcGISLoginPrompt.SetChallengeHandler();

        }
    }
}

