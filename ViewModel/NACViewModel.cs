using DisplayAnOfflineMapOnDemand.Utils;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DisplayAnOfflineMapOnDemand.ViewModel
{
    internal class NACViewModel : ViewModelBase
    {

        private readonly VideoCapture capture;
        private readonly Dispatcher dispatcher;
        private readonly BackgroundWorker bkgWorker;
        private ImageSource? _frameImage;
        public ImageSource? FrameImage
        {
            get => _frameImage;
            set
            {   
                Debug.WriteLine("Setting FrameImage.");
                _frameImage = value;
                OnPropertyChanged(nameof(FrameImage));
            }
        }
        
        public RelayCommand WindowClose => new RelayCommand(execute => BtnCloseWindow(), canExecute => { return true; });
        public RelayCommand WindowSize => new RelayCommand(execute => BtnSizeWindow(), canExecute => { return true; });
        public RelayCommand WindowHide => new RelayCommand(execute => BtnHideWindow(), canExecute => { return true; });
        public RelayCommand WindowDrag => new RelayCommand(execute => BtnDragWindow(), canExecute => { return true; });
        public RelayCommand CameraOn => new RelayCommand(execute => Camera(), canExecute => { return true; });
        public NACViewModel()
        {   

            dispatcher = Application.Current.Dispatcher; // Initialize the dispatcher
            capture = new VideoCapture();
            bkgWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            bkgWorker.DoWork += Worker_DoWork;
            Debug.WriteLine("NACViewModel Constructor Called.");
            FrameImage = new BitmapImage(new Uri(@"\Assets\offlineImage.png", UriKind.RelativeOrAbsolute));
        }
        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            bkgWorker.CancelAsync();

            capture.Dispose();
            //cascadeClassifier.Dispose();
        }

        

        public void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                using (var frameMat = capture.RetrieveMat())
                {
                    // Must create and use WriteableBitmap in the same thread(UI Thread).
                    if (!frameMat.Empty())
                    {
                        dispatcher.Invoke(() => // Use the instance of Dispatcher
                        {
                            FrameImage = frameMat.ToWriteableBitmap();
                        });
                    }
                }

                Thread.Sleep(1000 / 90);
            }
        }
        public void BtnDragWindow()
        {
            Application.Current.MainWindow.DragMove();
        }

        private void BtnCloseWindow()
        {   
            Debug.WriteLine("Closing Window.");
            bkgWorker.CancelAsync();
            capture.Dispose();
            Application.Current.MainWindow.Close();
            //Application.Current.Shutdown();
        }

        public void BtnSizeWindow()
        {
            Debug.WriteLine("Toggling Window Size.");
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        public void BtnHideWindow()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        public void Camera()
        {
            if (!capture.IsOpened())
            {
                try
                {

                    Task<Mat> task = Task.Run(() =>
                    {
                        Mat frame = new Mat();
                        capture.Open("http://192.168.0.17:9300/stream.mjpg");
                        bkgWorker.RunWorkerAsync();

                        return frame;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error connecting to video stream: " +
                        $"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                bkgWorker.CancelAsync();
                Task.Delay(1000).ContinueWith(_ =>
                {
                    Debug.WriteLine("Releasing Capture.");
                    capture.Release();
                    FrameImage = new BitmapImage(new Uri(@"/Assets/offlineImage.png", UriKind.Relative));
                });

            }

        }
    }
}
