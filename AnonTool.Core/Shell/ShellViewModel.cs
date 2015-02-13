using AnonTool.Core.MenuBar;
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
        private MenuBarViewModel _menuBarVm = new MenuBarViewModel();
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

    }
}
