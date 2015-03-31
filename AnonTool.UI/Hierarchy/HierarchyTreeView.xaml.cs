using KAnonymisation.Core.Hierarchy;
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

namespace AnonTool.UI.Hierarchy
{
    /// <summary>
    /// Interaction logic for HierarchyTreeView.xaml
    /// </summary>
    public partial class HierarchyTreeView : UserControl
    {
        public HierarchyTreeView()
        {
            InitializeComponent();
        }

        private void hierarchyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var dataContext = (AnonymisationHierarchy) this.DataContext;

            dataContext.SelectedTreeNode = (Node)hierarchyTreeView.SelectedItem;           
        }
    }
}
