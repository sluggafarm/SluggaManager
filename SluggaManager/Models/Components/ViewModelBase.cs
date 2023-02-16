using System.ComponentModel;

namespace SluggaManager
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            var action = PropertyChanged;
            if (action != null)
            {
                action(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
