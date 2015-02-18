using AnonTool.Core.MenuBar;
using AnonTool.Core.Preprocessing;
using AnonTool.MVVM.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonTool.Core.Shell
{
    public class ShellViewModel : UpdateBase
    {
        private MenuBarViewModel _menuBarVm;
        private PreProcessingViewModel _preprocessingVm;

        public MenuBarViewModel MenuBarVm
        {
            get { return _menuBarVm; }
            set
            {
                if(_menuBarVm != value)
                {
                    _menuBarVm = value;
                    RaisePropertyChanged(() => MenuBarVm);
                }
            }
        }
        public PreProcessingViewModel PreprocessingVm
        {
            get { return _preprocessingVm; }
            set
            {
                if(_preprocessingVm != value)
                {
                    _preprocessingVm = value;
                    RaisePropertyChanged(() => PreprocessingVm);
                }
            }
        }

        //Constructor
        public ShellViewModel()
        {
            _menuBarVm = new MenuBarViewModel();
            _preprocessingVm = new PreProcessingViewModel();

            _menuBarVm.importedData += _menuBarVm_importedData;
        }

        void _menuBarVm_importedData(object sender, System.Data.DataTable importedDataTable)
        {
            _preprocessingVm.InputDataTable = importedDataTable;
        }
    }
}
