using AnonTool.Core.Hierarchy;
using AnonTool.MVVM.Commands;
using AnonTool.MVVM.Updates;
using AnonTool.UI.Hierarchy;
using KAnonymisation.Core.ColumnInfo;
using KAnonymisation.Core.Hierarchy;
using KAnonymisation.Core.IdentifierTypes;
using KAnonymisation.Core.Interfaces;
using KAnonymisation.Hierarchy;
using KAnonymisation.SetBased;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace AnonTool.Core.Preprocessing
{
    public class PreprocessingColumnsVm : UpdateBase
    {
        private bool _loadPluginAnonymisations = false;
        private PreProcessingViewModel _parentVm;
        private PreprocessColumnVm _selectedColumn;
        private HierarchyDefintionShellVm _hierarchyDefintionShellVm;
        private Dictionary<string, HierarchyDefintionShellVm> _hierarchyDefintionDict = new Dictionary<string, HierarchyDefintionShellVm>();
        private AnonymisationHierarchy _anonymisationHierarchy;

        private ObservableCollection<IdentifierType> _availableAttributeTypes = new ObservableCollection<IdentifierType>() 
            { IdentifierType.NonSensitive, IdentifierType.Sensitive, IdentifierType.Quasi, IdentifierType.Explicit };
        private ObservableCollection<IKAnonymisation> _availableKAnonymisations;
        private ObservableCollection<PreprocessColumnVm> _columns = new ObservableCollection<PreprocessColumnVm>();
        private ICommand _defineHierarchyCommand;
        
        
        public ICommand DefineHierarchyCommand
        {
            get { return _defineHierarchyCommand ?? (_defineHierarchyCommand = new RelayCommand(o => DefineHierarchy(), o => true)); }
        }
        public PreprocessColumnVm SelectedColumn
        {
            get { return _selectedColumn; }
            set
            {
                if(_selectedColumn != value)
                {
                    _selectedColumn = value;
                    if (_selectedColumn.SelectedAnonymisation == null && AvailableKAnonymisations.Count > 0)
                        _selectedColumn.SelectedAnonymisation = AvailableKAnonymisations.First();
                        
                    RaisePropertyChanged(() => SelectedColumn);
                }
            }
        }
        public ObservableCollection<PreprocessColumnVm> Columns
        {
            get { return _columns; }
            set
            {
                if(_columns != value)
                {
                    _columns = value;
                    RaisePropertyChanged(() => Columns);
                }
            }
        }
        public ObservableCollection<IdentifierType> AvailableAttributeTypes
        {
            get { return _availableAttributeTypes; }
        }
        public ObservableCollection<IKAnonymisation> AvailableKAnonymisations
        {
            get { return _availableKAnonymisations; }
            set
            {
                if(_availableKAnonymisations != value)
                {
                    _availableKAnonymisations = value;
                    RaisePropertyChanged(() => AvailableKAnonymisations);
                }
            }
        }

        //Constructor
        public PreprocessingColumnsVm(PreProcessingViewModel parentVm, bool loadPluginAnons)
        {
            _parentVm = parentVm;
            _loadPluginAnonymisations = loadPluginAnons;
            LoadAvailableKAnonymisations();
        }

        private void LoadAvailableKAnonymisations()
        {
            //Defaults hard coded to load
            IKAnonymisation defaultSetBasedAnon = new SetBasedAnonymisation();
            IKAnonymisation defaultHierarchyBasedAnon = new HierarchyBasedAnonymisation();
            AvailableKAnonymisations = new ObservableCollection<IKAnonymisation>() { defaultSetBasedAnon, defaultHierarchyBasedAnon };

            if (!_loadPluginAnonymisations)
                return;

            try
            {
                //Dynamically load anonymisation plugins
                List<IKAnonymisation> pluginAnons = DynamicallyLoadAnonymisationPlugins();
                foreach (var iKAnon in pluginAnons)
                    AvailableKAnonymisations.Add(iKAnon);
            }
            catch(Exception ex)
            {
                var msgBox = MessageBox.Show(ex.Message, "Error Dynamically Loading Anonymisation Plugins");
            }

        }
        private List<IKAnonymisation> DynamicallyLoadAnonymisationPlugins()
        {
            var result = new List<IKAnonymisation>();

            var curDir = Directory.GetCurrentDirectory();
            var baseDir = Application.StartupPath;
            var pPath = "\\Plugins";
            var fullPath = string.Format("{0}{1}", baseDir, pPath);

            string[] plugins = Directory.GetFiles(fullPath, "*.DLL");

            foreach(var plugin in plugins)
            {
                var assembly = Assembly.LoadFile(plugin);
                if(assembly != null)
                {
                    var type = typeof(IKAnonymisation);
                    var types = assembly.GetTypes();
                    foreach(var t in  types)
                    {
                        if (type.IsAssignableFrom(t))
                            result.Add((IKAnonymisation)Activator.CreateInstance(t));
                    }
                }
            }
            return result;
        }


        private void DefineHierarchy()
        {
            if (SelectedColumn == null)
                return;

            var uniqueVals = GetUniqueValues(_parentVm.InputDataTable, SelectedColumn);

            if(_hierarchyDefintionDict.ContainsKey(SelectedColumn.Header))
            {
                _hierarchyDefintionShellVm = _hierarchyDefintionDict[SelectedColumn.Header];
            }else
            {
                _hierarchyDefintionShellVm = new HierarchyDefintionShellVm(SelectedColumn.Header, uniqueVals);
                _hierarchyDefintionDict.Add(SelectedColumn.Header, _hierarchyDefintionShellVm);
            }
            
            var hierarchyDefintionDialog = new HierarchyDefintionDialog();


            hierarchyDefintionDialog.DataContext = _hierarchyDefintionShellVm;
            hierarchyDefintionDialog.ShowDialog();

            SelectedColumn.AnonymisationHierarchy = _hierarchyDefintionShellVm.ExtractHierarchy();
        }
        private ObservableCollection<string> GetUniqueValues(System.Data.DataTable dataTable, PreprocessColumnVm SelectedColumn)
        {
            var uniqueVals = new ObservableCollection<string>();
            var vls = new ObservableCollection<string>();

            foreach (DataRow row in dataTable.Rows) 
            { 
                var str = row[SelectedColumn.Header].ToString();

                if (!uniqueVals.Contains(str))
                    uniqueVals.Add(row[SelectedColumn.Header].ToString());
            }
            return uniqueVals;
        }
        public List<ColumnModel> TranslateToColumnModels()
        {
            var columnModel = new List<ColumnModel>();

            foreach(var col in Columns)
            {
                var colMod = new ColumnModel()
                {
                     AttributeType = col.AttributeType,
                     DataType = col.DataType,
                     Header = col.Header,
                     K = col.K,
                     AnonymisationHierarchy = col.AnonymisationHierarchy,
                     KAnonymisation = col.SelectedAnonymisation
                      
                };
                columnModel.Add(colMod);
            }

            return columnModel;
        }
    }
}
