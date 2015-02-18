using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnonTool.UI.DataImport
{
    /// <summary>
    /// Interaction logic for DataImportDialog.xaml
    /// </summary>
    public partial class DataImportDialog : Window
    {
        public DataImportDialog()
        {
            InitializeComponent();
        }

        private void Dialog_Done(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Dialog_Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
