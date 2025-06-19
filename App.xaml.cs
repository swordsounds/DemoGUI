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
            ArcGISRuntimeEnvironment.ApiKey = "AAPTxy8BH1VEsoebNVZXo8HurHI84bxf92iHEtus-iAVKYwnOINb0WgUe2LJCJ93o2bG7BEgy8fEPJYT3-nvNDRkHw_Wzd5_Fk6cnyfCtV2urz9sCeSPjbXIzUl8k2WWoUpzzBYSZIROGYA1wmx8n56Qibln1ix1y6tQo30GArZLDfe8Krer5Wii95knAe65wFXEDZv4AEAhM-oLhAbE1UMB-W2MfW7eUAPG3MTLpnNAwQw.AT1_B4pdHfJs";
            // Set license key
            string licenseKey = "runtimelite,1000,rud2388348839,none,TRB3LNBHPFK7003AD123";
            ArcGISRuntimeEnvironment.SetLicense(licenseKey);

            // Call a function to set up the AuthenticationManager for OAuth.
            UserAuth.ArcGISLoginPrompt.SetChallengeHandler();

        }
    }
}

