using System;
using System.Windows.Input;

namespace YukarinetteSePlayer.Core
{
    public class RelayCommand : ICommand
    {
        private readonly Action _action;

        public RelayCommand(Action action)
        {
            _action = action;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _action != null;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _action;

        public RelayCommand(Action<T> action)
        {
            _action = action;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _action != null;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}