using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileCheck.ViewModels.Base
{
    public class BaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, field)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
