using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using AnonTool.Core.Shell;

namespace DataAnonTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;

            var shellVm = new ShellViewModel();
            shellView.DataContext = shellVm;
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
