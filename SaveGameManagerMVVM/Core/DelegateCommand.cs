using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveGameManagerMVVM.Core
{
    internal class DelegateCommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public DelegateCommand(Action<object> execute) : this(execute, null) { }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChange() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(object? paramter) => _canExecute?.Invoke(paramter) ?? true;

        public void Execute(object? parameter) => _execute?.Invoke(parameter);
    }
}
