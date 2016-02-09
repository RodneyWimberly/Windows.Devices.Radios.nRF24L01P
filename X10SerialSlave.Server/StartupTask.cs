using Windows.ApplicationModel.Background;

namespace X10SerialSlave.Server
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            new WebServer(new Rf24Server()).RunAsync(taskInstance);
            //new WebServer(new PSC05Server()).RunAsync(taskInstance);
            //new WebServer(CM11AServer.GetInstance()).RunAsync(taskInstance);
        }
    }
}
