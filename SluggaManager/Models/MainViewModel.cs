using Newtonsoft.Json;
using SluggaManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace SluggaManager
{
    public class MainViewModel : ViewModelBase
    {
        SluggaService svc = new SluggaService();
        const string cache_path = $@"c:\temp\_slugga_cache\";
        public MainViewModel()
        {
            //Trace.TraceInformation("Main ViewModel ctor");
            Wallet = svc.Wallet;
            ManualStartCommand = new SimpleCommand(ManualStart_Action);
            RefreshStateCommand = new SimpleCommand(RefreshState_Action);

            AddScheduledActionCommand = new SimpleCommand(AddScheduledAction_Action);
            SluggaPetAction = new SimpleCommand(arg => Slugga_Action("Pet", arg));
            SluggaSleepAction = new SimpleCommand(arg => Slugga_Action("Sleep", arg));
            SluggaFeedAction = new SimpleCommand(arg => Slugga_Action("Feed", arg));
            SluggaRefreshAction = new SimpleCommand(arg => Slugga_GetState_Action(arg));

            callback = new TimerCallback(OnTimerTick);

            //ticker = new Timer(callback, this, 10000, 10000);

            InitializeSluggaList();
        }

        readonly TimerCallback callback;
        Timer ticker;

        private void FireAll(string action)
        {
            ServiceMessages.Add($"Fire All: {action}");
            Action<object?> todoSluggaAction = a => { };
            switch(action)
            {
                case "Pet":
                    todoSluggaAction = a => {
                        if (a != null) { App.Current.Dispatcher.Invoke(new Action<SluggaStateMessage>(r => { SluggaPetAction.Execute(r); }), a as SluggaStateMessage); }
                    };
                    break;
                case "Feed":
                    todoSluggaAction = a => {
                        if (a != null) { App.Current.Dispatcher.Invoke(new Action<SluggaStateMessage>(r => { SluggaFeedAction.Execute(r); }), a as SluggaStateMessage); }
                    };
                    break;
                case "Sleep":
                    todoSluggaAction = a => { 
                        if (a != null) { App.Current.Dispatcher.Invoke(new Action<SluggaStateMessage>(r => { SluggaSleepAction.Execute(r); }), a as SluggaStateMessage); } 
                    };
                    break;
            }

            foreach(var sl in this.SluggaStateList.Take(5))
            {
                if (sl != null && sl.message != null)
                {
                    ServiceMessages.Add($"{action} -> {sl.message.slug.token_id}");
                    var task = Task.Factory.StartNew(todoSluggaAction, sl);
                    task.Wait();
                    ServiceMessages.Add($"{action} -> {sl.message.slug.token_id}: done!");
                    Thread.Sleep(1250);
                }
            }
        }

        private void OnTimerTick (object? state)
        {

            App.Current.Dispatcher.Invoke(new Action(() => { ServiceMessages.Add($"Time Elapsed! @ {DateTime.Now:HH:mm:ss}"); }));

            App.Current.Dispatcher.Invoke(new Action(() => {
                if (ScheduledJobs.Any())
                {
                    ServiceMessages.Add($"Schedules present. evaluating...");
                    for (var q = ScheduledJobs.Count - 1; q >= 0; q--)
                    {
                        var job = ScheduledJobs[q];
                        if (job.TriggerTime <= DateTime.Now)
                        {
                            try
                            {
                                MessageBox.Show("Coming Soon!");
                                //FireAll(job.ActionName);

                                ServiceMessages.Add($"Job Triggered!");
                                ScheduledJobs.RemoveAt(q);

                            }
                            catch (Exception ex)
                            {
                                ServiceMessages.Add($"Error: {ex.Message}");
                            }
                        }
                    }
                }
            }));

            
        }

        private void AddScheduledAction_Action(object? state)
        {
            string[] actions = new[] { "Pet", "Feed", "Slee" };  
            var schedule_date = DateTime.Parse($"{DateTime.Now:yyyy-MM-dd} {JobActionSelectedDateText}");
            var sched = new QueuedJob
            {
                TriggerTime = schedule_date,
                ActionName = actions[JobActionSelectedIndex]
            };
            App.Current.Dispatcher.Invoke(new Action<QueuedJob>(j => { 
                this.ScheduledJobs.Add(j);
            }), sched);
        }


        private void Slugga_Action (string actionName, object? state)
        {
            if (state == null) { return;  }
            SluggaStateMessage message = (SluggaStateMessage)state;
            App.Current.Dispatcher.Invoke(new Action<SluggaStateMessage>(o =>
            {
                o.IsAvailable = false;
            }), message);

            Trace.TraceInformation($"{actionName}, {message.message.slug.token_id}");

            try
            {
                string response_json = ""; 
                if (actionName.ToLower().Equals("pet"))
                {

                    ServiceMessages.Add($"Pet {message.message.slug.token_id} @ {DateTime.Now:t}");
                    response_json = svc.Pet(message.message.slug.token_id);
                    Trace.TraceInformation(response_json);
                    ServiceMessages.Add(response_json);

                }
                if (actionName.ToLower().Equals("feed"))
                {
                    ServiceMessages.Add($"Feed {message.message.slug.token_id} @ {DateTime.Now:t}");
                    response_json = svc.Feed(message.message.slug.token_id);
                    Trace.TraceInformation(response_json);
                    ServiceMessages.Add(response_json);
                }
                if (actionName.ToLower().Equals("sleep"))
                {
                    ServiceMessages.Add($"Sleep {message.message.slug.token_id} @ {DateTime.Now:t}");
                    response_json = svc.Sleep(message.message.slug.token_id);
                    Trace.TraceInformation(response_json);
                    ServiceMessages.Add(response_json);
                }

                App.Current.Dispatcher.Invoke(new Action<SluggaStateMessage>(o =>
                {
                    SluggaRefreshAction.Execute(o);
                }), message);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{nameof(Slugga_Action)}: {actionName}: {message.message.slug.token_id}: {ex.Message}");
            }


            App.Current.Dispatcher.Invoke(new Action<SluggaStateMessage>(o =>
            {
                o.IsAvailable = true;
            }), message);
        }
        
        private void Slugga_GetState_Action(object? state)
        {
            if (state == null) { return; }
            SluggaStateMessage message = (SluggaStateMessage)state;
            App.Current.Dispatcher.Invoke(new Action<SluggaStateMessage>(o =>
            {
                o.IsAvailable = false;
            }), message);

            //Trace.TraceInformation($"Get State!!! {message.message.slug.token_id}");

            try
            {
                var response_json = svc.GetState(message.message.slug.token_id);
                System.IO.File.WriteAllText($"{cache_path}{message.message.slug.token_id}.json", response_json);
                ServiceMessages.Add(response_json);
                
                var new_message = JsonConvert.DeserializeObject<SluggaStateMessage>(response_json);
                
                for(var t = 0; t < this.SluggaStateList.Count; t++)
                {
                    var target = this.SluggaStateList[t];
                    if (target.message.slug.token_id == new_message.message.slug.token_id)
                    {
                        this.SluggaStateList[t] = new_message;

                    }
                }

                //this.RefreshState_Action(response_json);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{nameof(Slugga_GetState_Action)}: {ex.Message}");
            }

            App.Current.Dispatcher.Invoke(new Action<SluggaStateMessage>(o =>
            {
                //var satisfied_sleep_quota = o.message.slug.locks.Any(g => g.action == "sleep" && g.count == "1");

                //if (satisfied_sleep_quota)
                //{
                //    o.IsAvailable = false;
                //}
                //else
                //{
                //    o.IsAvailable = true;
                //}



            }), message);
        }

        private void ManualStart_Action(object? state)
        {
            Trace.TraceInformation(nameof(ManualStart_Action));
            InitializeSluggaList();
        }

        private void RefreshState_Action(object? state)
        {
            SluggaStateList.Clear();

            InitializeSluggaList();
        }

        private void InitializeSluggaList()
        {
            var list = new List<SluggaStateMessage>(); 
            var shards = "";
            foreach(var item in System.IO.Directory.GetFiles(cache_path))
            {
                try
                {
                    var msg = JsonConvert.DeserializeObject<SluggaStateMessage>(System.IO.File.ReadAllText(item));

                    if (msg != null)
                    {
                        if (!String.IsNullOrWhiteSpace(msg.message.wallet.shard))
                        {
                            shards = msg.message.wallet.shard;
                        }
                        list.Add(msg);
                    }
                    else
                    {
                        var tokenId = item.Replace(".json", "").Replace(cache_path, "");
                        list.Add(new SluggaStateMessage { message = new SluggaResultMessage { slug = new Slug { token_id = tokenId, xp = "ERROR" } } });
                    }
                }
                catch(Exception ex)
                {
                    var tokenId = item.Replace(".json", "").Replace(cache_path, "");
                    list.Add(new SluggaStateMessage { message = new SluggaResultMessage { slug = new Slug { token_id = tokenId, xp = "ERROR" } } });
                    Trace.TraceError($"{ex}");
                }
            }

            foreach(var item in list.OrderBy(q => Convert.ToInt32(q.message.slug.token_id)))
            {
                SluggaStateList.Add(item);
            }

            this.ShardCount = shards;
        }



        private string _JobActionSelectedDateText = "4:00 AM";
        public string JobActionSelectedDateText
        {
            get { return _JobActionSelectedDateText; }
            set
            {
                if (_JobActionSelectedDateText == value) return;
                _JobActionSelectedDateText = value;
                RaisePropertyChanged("JobActionSelectedDateText");
            }
        }

        private int _JobActionSelectedIndex = 0;
        public int JobActionSelectedIndex
        {
            get { return _JobActionSelectedIndex; }
            set
            {
                if (_JobActionSelectedIndex == value) return;
                _JobActionSelectedIndex = value;
                RaisePropertyChanged("JobActionSelectedIndex");
            }
        }



        private string _Title = "Slugga Manager v1";
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }


        private ObservableCollection<QueuedJob> _ScheduledJobs = new ObservableCollection<QueuedJob>();
        public ObservableCollection<QueuedJob> ScheduledJobs
        {
            get { return _ScheduledJobs; }
            set
            {
                if (_ScheduledJobs == value) return;
                _ScheduledJobs = value;
                RaisePropertyChanged("ScheduledJobs");
            }
        }

        private ObservableCollection<string> _ServiceMessages = new ObservableCollection<string>();
        public ObservableCollection<string> ServiceMessages
        {
            get { return _ServiceMessages; }
            set
            {
                if (_ServiceMessages == value) return;
                _ServiceMessages = value;
                RaisePropertyChanged("ServiceMessages");
            }
        }

        private static ObservableCollection<SluggaStateMessage> _SluggaStateList = new ObservableCollection<SluggaStateMessage>();
        public ObservableCollection<SluggaStateMessage> SluggaStateList
        {
            get { return _SluggaStateList; }
            set
            {
                if (_SluggaStateList == value) return;
                _SluggaStateList = value;
                RaisePropertyChanged("SluggaStateList");
            }
        }


        private string _Wallet = "";
        public string Wallet
        {
            get { return _Wallet; }
            set
            {
                if (_Wallet == value) return;
                _Wallet = value;
                RaisePropertyChanged("Wallet");
            }
        }


        private string _APIVersion = "v1";
        public string APIVersion
        {
            get { return _APIVersion; }
            set
            {
                if (_APIVersion == value) return;
                _APIVersion = value;
                RaisePropertyChanged("APIVersion");
            }
        }


        private string _ShardCount = "1500";
        public string ShardCount
        {
            get { return _ShardCount; }
            set
            {
                if (_ShardCount == value) return;
                _ShardCount = value;
                RaisePropertyChanged("ShardCount");
            }
        }

        public SimpleCommand SluggaSleepAction { get; set; }
        public SimpleCommand SluggaFeedAction { get; set; }
        public SimpleCommand SluggaPetAction { get; set; }
        public SimpleCommand SluggaRefreshAction { get; set; }

        public SimpleCommand ManualStartCommand { get; set; }
        public SimpleCommand RefreshStateCommand { get; set; }

        public SimpleCommand AddScheduledActionCommand { get; set; }
    }

}
