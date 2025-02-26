using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMExample
{
    public class RelayCommand : ICommand
    {

        public event EventHandler? CanExecuteChanged;

        private Action? action;
        private Action<object>? objectAction;
        public RelayCommand(Action action)
        {
            this.action = action;
        }

        public RelayCommand(Action<object> objectAction)
        {
            this.objectAction = objectAction;
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action?.Invoke();
            if (parameter != null)
                objectAction?.Invoke(parameter);
        }
    }

    public class ModelCommand : ICommand
    {
        #region Constructors

        public ModelCommand(Action<object> execute)
            : this(execute, null) { }

        public ModelCommand(Action<object> execute, Predicate<object>? canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute != null && parameter != null ? _canExecute(parameter) : true;
        }

        public void Execute(object? parameter)
        {
            if (_execute != null && parameter != null)
                _execute(parameter);
        }

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        #endregion

        private readonly Action<object> _execute;
        private readonly Predicate<object>? _canExecute = null;
    }
}
