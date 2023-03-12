using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AdoNet.Infrastructure
{
    internal class Command : ICommand
    {
        private Action<object> execute;
        private Predicate<object> canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }


        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
            => canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter)
            => execute?.Invoke(parameter);
    }
}
