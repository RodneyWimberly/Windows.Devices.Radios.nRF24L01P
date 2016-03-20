using Windows.ApplicationModel.Background;

namespace nRF24L01P.TestHarness
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            new BackgroundServer(new ReadAndWriteToSensorServerModule()).RunAsync(taskInstance);
        }
    }
}
