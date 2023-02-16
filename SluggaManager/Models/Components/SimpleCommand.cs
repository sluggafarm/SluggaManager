using System;
using System.Windows.Input;

namespace SluggaManager
{
    public class SimpleCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        readonly Action<object?> _action;
        public SimpleCommand(Action<object?> action)
        {
            _action = action;   

        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }
        public void Execute(object? parameter)
        {
            if (_action != null)
            {
                _action(parameter);
            }
        }
    }
}
