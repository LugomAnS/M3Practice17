using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace AdoNet.Infrastructure
{
    internal class INPC : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChange([CallerMemberName] string property = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        public bool Set<T>(ref T field, T value, [CallerMemberName] string property = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChange(property);
            return true;
        }
    }
}
