using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace nRF24L01P.TestHarness
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            new WebServer(new ReadAndWriteToRadio()).RunAsync(taskInstance);
            
        }
    }
}
