using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GifMaker.Application
{
    class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public Command(Action action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action.Invoke();
        }

        private Action action;
    }
}
