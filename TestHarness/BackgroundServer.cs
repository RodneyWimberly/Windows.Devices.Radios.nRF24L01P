using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace nRF24L01P.TestHarness
{
    public sealed class BackgroundServer
    {
        private BackgroundTaskDeferral _deferral;
        private readonly IReadAndWriteProcess _readAndWriteProcess;

        public BackgroundServer(IReadAndWriteProcess readAndWriteProcess)
        {
            _readAndWriteProcess = readAndWriteProcess;
        }

        public void RunAsync(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            Task.Factory.StartNew(this.Initialize, TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public void Initialize()
        {
            _readAndWriteProcess.Initialize();
        }
    }
}
