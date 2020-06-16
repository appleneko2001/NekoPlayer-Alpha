using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Action<Object> mExecute;
    private readonly Predicate<Object> mCanExecute;

    /// <summary>
    /// Create a RelayCommand object, parameter canExecute will keep true.
    /// </summary>
    /// <param name="execute"></param>
    public RelayCommand(Action<Object> execute)
        : this(execute, null)
    {

    }

    public RelayCommand(Action<Object> execute, Predicate<Object> canExecute)
    {
        Exception e = null;
        mExecute = execute ?? throw new ArgumentNullException("Execute", e);
        mCanExecute = canExecute;
    }

    #region ICommand Members

    public bool CanExecute(object parameter)
    {
        return mCanExecute == null ? true : mCanExecute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object parameter)
    {
        mExecute(parameter);
    }

    #endregion
}