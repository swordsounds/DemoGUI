using System.Windows;
using System.Windows.Input;
using DisplayAnOfflineMapOnDemand.ViewModel;
namespace DisplayAnOfflineMapOnDemand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            //MainWindowViewModel vm = new MainWindowViewModel();
            //this.DataContext = vm;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.DragMove();
        }
    }
}

