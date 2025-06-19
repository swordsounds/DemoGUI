using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Offline;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DisplayAnOfflineMapOnDemand.ViewModel
{
    class MapViewModel : INotifyPropertyChanged
    {
        public MapViewModel()
        {
            _ = SetupMap();
        }
     
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Map? _map;
        public Map? Map
        {
            get { return _map; }
            set
            {
                _map = value;
                OnPropertyChanged();
            }
        }

        private GraphicsOverlayCollection? _graphicsOverlays;
        public GraphicsOverlayCollection? GraphicsOverlays
        {
            get { return _graphicsOverlays; }
            set
            {
                _graphicsOverlays = value;
                OnPropertyChanged();
            }
        }

        private async Task SetupMap()
        {
            try
            {
                // Create a portal pointing to ArcGIS Online.
                ArcGISPortal portal = await ArcGISPortal.CreateAsync();

                // Create a portal item for a specific web map id.
                string webMapId = "59ffaf68fe7a4f8fafafa2fb7a27c2dc";


                PortalItem mapItem = await PortalItem.CreateAsync(portal, webMapId);

                // Create the map from the item.
                Map map = new Map(mapItem);

                // Set the view model "Map" property.
                Map = map;

                // For ArcGIS Pro users only :C
                //try
                //{
                //    var mobileMapPackage = await MobileMapPackage.OpenAsync(offlineMapFilePath);
                //    await mobileMapPackage.LoadAsync();
                //    this.Map = mobileMapPackage.Maps.First();
                //}
                //catch (Exception ex)
                //{
                //    Debug.WriteLine($"Error loading mobile map package: {ex.Message}");
                //    this.Map = map;
                //}

                // Define area of interest (envelope) to take offline.
                EnvelopeBuilder envelopeBldr = new EnvelopeBuilder(SpatialReferences.Wgs84)
                {
                    XMin = -52.7365,
                    XMax = -52.7303,
                    YMin = 47.5695,
                    YMax = 47.5750
                };

                Envelope offlineArea = envelopeBldr.ToGeometry();

                // Create a graphic to display the area to take offline.
                SimpleLineSymbol lineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Red, 2);
                SimpleFillSymbol fillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, Color.Transparent, lineSymbol);
                Graphic offlineAreaGraphic = new Graphic(offlineArea, fillSymbol);

                // Create a graphics overlay and add the graphic.
                GraphicsOverlay areaOverlay = new GraphicsOverlay();
                areaOverlay.Graphics.Add(offlineAreaGraphic);
                
                
                MapPoint point = new MapPoint(
                    x: -52.7365,
                    y: 47.5695,
                    spatialReference: SpatialReferences.Wgs84
                    );
                SimpleMarkerSymbol symbol = new SimpleMarkerSymbol(
                    style: SimpleMarkerSymbolStyle.Circle,
                    color: Color.Blue,
                    size: 10
                    );
                Graphic pointGraphic = new Graphic(
                    geometry: point,
                    symbol:symbol
                    );
                
                areaOverlay.Graphics.Add(pointGraphic);
                // Add the overlay to a new graphics overlay collection.
                GraphicsOverlayCollection overlays = new GraphicsOverlayCollection
                {
                    areaOverlay
                };

                // Set the view model's "GraphicsOverlays" property (will be consumed by the map view).
                GraphicsOverlays = overlays;

                // Create an offline map task using the current map.
                OfflineMapTask offlineMapTask = await OfflineMapTask.CreateAsync(map);

                // Create a default set of parameters for generating the offline map from the area of interest.
                GenerateOfflineMapParameters parameters = await offlineMapTask.CreateDefaultGenerateOfflineMapParametersAsync(offlineArea);
                parameters.UpdateMode = GenerateOfflineMapUpdateMode.NoUpdates;

                // Build a folder path in the "bin/Debug/net" folder.
                string OfflineMap1;
                string directoryFolder = Directory.GetCurrentDirectory();
                string fullPath = Path.Combine(directoryFolder, nameof(OfflineMap1));
                string downloadDirectory = Path.Combine(fullPath);
                Directory.CreateDirectory(downloadDirectory);

                DirectoryInfo di = new DirectoryInfo(fullPath);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }

                GenerateOfflineMapJob generateJob = offlineMapTask.GenerateOfflineMap(parameters, downloadDirectory);
               

                generateJob.ProgressChanged += GenerateJob_ProgressChanged;

                //Debug.WriteLine(directoryFolder);

                generateJob.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting up map: {ex.Message}");
            }
        }
        private async void GenerateJob_ProgressChanged(object? sender, EventArgs e)
        {

            try
            {
                var generateJob = sender as GenerateOfflineMapJob;
                if(generateJob == null) { return; }

                // If the job succeeds, show the offline map in the map view.
                if (generateJob.Status == Esri.ArcGISRuntime.Tasks.JobStatus.Succeeded)
                {
                    var result = await generateJob.GetResultAsync();
                    Map = result.OfflineMap;
                    Debug.WriteLine("Generate offline map: Complete");
                }
                // If the job fails, notify the user.
                else if (generateJob.Status == Esri.ArcGISRuntime.Tasks.JobStatus.Failed)
                {
                    MessageBox.Show($"Unable to generate a map for that area: {generateJob?.Error?.Message}");
                }
                else
                {
                    int percentComplete = generateJob.Progress;
                    Debug.WriteLine($"Generate offline map: {percentComplete}%");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating offline map: {ex.Message}");
            }

        }

    }
}

