using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// A class used for notify WPF to update the new properties values. For notify, just call OnPropertyChanged on inside the property
/// </summary>
public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
