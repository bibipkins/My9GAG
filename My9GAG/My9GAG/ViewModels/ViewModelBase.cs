using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace My9GAG.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Implementation

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(oldValue, newValue))
                return false;

            oldValue = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
