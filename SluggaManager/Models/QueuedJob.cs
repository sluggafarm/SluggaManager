using System;

namespace SluggaManager
{
    public sealed class QueuedJob : ViewModelBase
    {

        private DateTime _TriggerTime = DateTime.Now;
        public DateTime TriggerTime
        {
            get { return _TriggerTime; }
            set
            {
                if (_TriggerTime == value) return;
                _TriggerTime = value;
                RaisePropertyChanged("TriggerTime");
            }
        }


        private DateTime _Created = DateTime.Now;
        public DateTime Created
        {
            get { return _Created; }
            set
            {
                if (_Created == value) return;
                _Created = value;
                RaisePropertyChanged("Created");
            }
        }

        private string _ActionName;
        public string ActionName
        {
            get { return _ActionName; }
            set
            {
                if (_ActionName == value) return;
                _ActionName = value;
                RaisePropertyChanged("ActionName");
            }
        }
    }
}
