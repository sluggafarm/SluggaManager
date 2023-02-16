using System;

namespace SluggaManager
{
    public class SluggaStateMessage: ViewModelBase
    {

        private SluggaResultMessage? _message;
        public SluggaResultMessage? message
        {
            get { return _message; }
            set
            {
                if (_message == value) return;
                _message = value;
                RaisePropertyChanged("message");
            }
        }
        public object error { get; set; }
        public object[] body { get; set; }
        public bool ok { get; set; }
        public int code { get; set; }


        private bool _IsAvailable = true;
        public bool IsAvailable
        {
            get { return _IsAvailable; }
            set
            {
                if (_IsAvailable == value) return;
                _IsAvailable = value;
                RaisePropertyChanged("IsAvailable");
            }
        }
    }

    public class SluggaResultMessage : ViewModelBase
    {

        private Slug _slug;
        public Slug slug
        {
            get { return _slug; }
            set
            {
                if (_slug == value) return;
                _slug = value;
                RaisePropertyChanged("slug");
            }
        }
        public Wallet wallet { get; set; }
        public string now { get; set; }
        public LifecycleEvent[] messages { get; set; }
    }

    public class Slug : ViewModelBase
    {
        public int id { get; set; }
        public string name { get; set; }
        public string token_id { get; set; }
        public int owner { get; set; }
        public object prophet_id { get; set; } = "";

        private string _rep = "";
        public string rep
        {
            get { return _rep; }
            set
            {
                if (_rep == value) return;
                _rep = value;
                RaisePropertyChanged("rep");
            }
        }

        private string _xp = "";
        public string xp
        {
            get { return _xp; }
            set
            {
                if (_xp == value) return;
                _xp = value;
                RaisePropertyChanged("xp");
            }
        }

        private string _action_in_progress = "";
        public string action_in_progress
        {
            get { return _action_in_progress; }
            set
            {
                if (_action_in_progress == value) return;
                _action_in_progress = value;
                RaisePropertyChanged("action_in_progress");
            }
        }
        public string lock_in_progress_from { get; set; } = "";

        private string _lock_in_progress_to = "";
        public string lock_in_progress_to
        {
            get { return _lock_in_progress_to; }
            set
            {
                if (_lock_in_progress_to == value) return;
                _lock_in_progress_to = value;
                RaisePropertyChanged("lock_in_progress_to");
            }
        }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public Lock[] locks { get; set; } = new Lock[0];

    }

    public class Lock
    {
        public int id { get; set; }
        public string slug_id { get; set; } = "";
        public string action { get; set; } = "";
        public string locked_from { get; set; } = "";
        public string locked_to { get; set; } = "";
        public string count { get; set; } = "0";   
    }

    public class Wallet
    {
        public int id { get; set; }
        public string address { get; set; } = string.Empty;
        public string shard { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class LifecycleEvent
    {
        public int id { get; set; }
        public string slug_id { get; set; } = "";
        public object prophet_id { get; set; } = "";
        public string wallet { get; set; } = "";
        public object owner { get; set; } = "";
        public string type { get; set; } = "";
        public string action { get; set; } = "";
        public string old_xp { get; set; } = "";
        public string xp { get; set; } = "";
        public object old_rep { get; set; } = "";
        public object new_rep { get; set; } = "";
        public object old_shards { get; set; } = "";
        public object shards { get; set; } = "";
        public string seen { get; set; } = ""; 
        public string created_at { get; set; } = "";
    }
}
