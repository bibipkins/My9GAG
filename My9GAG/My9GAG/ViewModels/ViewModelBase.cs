using My9GAG.Logic.Client;
using My9GAG.Logic.PageNavigator;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace My9GAG.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); }
        }
        public bool IsMessageVisible
        {
            get { return _isMessageVisible; }
            set { SetProperty(ref _isMessageVisible, value); }
        }
        public string WorkIndicationText
        {
            get { return _workIndicationText; }
            set { SetProperty(ref _workIndicationText, value); }
        }
        public bool IsWorkIndicationVisible
        {
            get { return _isWorkIndicationVisible; }
            set
            {
                if (SetProperty(ref _isWorkIndicationVisible, value))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UpdateCommands();
                    });
                }
            }
        }

        #endregion

        #region Implementation

        protected virtual void UpdateCommands()
        {

        }
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
        protected Task ShowMessage(string message, int timespan = ViewModelConstants.MESSAGE_DELAY)
        {
            return Task.Run(async () =>
            {
                Device.BeginInvokeOnMainThread(() => IsMessageVisible = true);
                MessageText = message;

                await Task.Delay(timespan);

                Device.BeginInvokeOnMainThread(() => IsMessageVisible = false);
                MessageText = "";
            });
        }
        protected void StartWorkIndication(string message)
        {            
            WorkIndicationText = message;
            IsWorkIndicationVisible = true;
        }
        protected void StopWorkIndication()
        {
            IsWorkIndicationVisible = false;
            WorkIndicationText = "";
        }

        #endregion

        #region Fields

        private string _messageText;
        private bool _isMessageVisible;
        private string _workIndicationText;
        private bool _isWorkIndicationVisible;

        #endregion
    }
}
