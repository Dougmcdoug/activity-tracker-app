using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RunningTrackingApp.Commands
{
    public class RelayCommand2<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        
        // Generic constructor
        public RelayCommand2(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Non-generic constructor for parameterless commands
        public RelayCommand2(Action execute, Func<bool> canExecute = null)
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
    public class RelayCommand2 : RelayCommand2<object>
    {
        public RelayCommand2(Action execute, Func<bool> canExecute = null) : base(execute, canExecute) { }
    }
}

/*
namespace DashboardProject
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);

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
}*/