using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace SluggaManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SetupDiagnostics();
            base.OnStartup(e);
        }
        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
        }
        private void SetupDiagnostics()
        {
            Trace.AutoFlush = true;
#if DEBUG
            Trace.Listeners.Add(new TextWriterTraceListener(@"c:\temp\slugga-farmer.log"));
#endif
            Trace.TraceInformation("Starting the Slugga Farm Management System");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;   
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.TraceError($"Unhandled Exception: {e.ExceptionObject}");
        }
    }
}
