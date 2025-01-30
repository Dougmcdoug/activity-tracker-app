using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RunningTrackingApp.Commands
{
    /// <summary>
    /// Generic implementation of RelayCommand.
    /// This allows for a RelayCommand with an input parameter of type T, including enforced type safety of the parameter.
    /// As with a standard RelayCommand, this allows us to bind to commands from xaml, as well as optionally specifying the
    /// criteria for when a command can be executed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        
        // Generic constructor
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Non-generic constructor for parameterless commands
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute != null ? new Action<T>(o => execute()) : throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute != null ? new Predicate<T>(o => canExecute()) : null;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);
        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }


    // Non-generic RelayCommand class that wraps the generic implementation so it can be called without a parameter type
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute, Func<bool> canExecute = null) : base(execute, canExecute) { }
    }
}