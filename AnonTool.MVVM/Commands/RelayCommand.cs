using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AnonTool.MVVM.Commands
{
    /// <summary>
    ///  RelayCommand Class used for wiring commands View -> ViewModel
    ///  Predicate used to assess can Execute
    ///  Class orignally from: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090051 Last Accessed: 29th April
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region private fields
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        #endregion

        #region Constructors
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region ICommand Members
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) ? true : _canExecute(parameter);
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }


        #endregion
    }
}
