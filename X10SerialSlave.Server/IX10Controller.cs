using System.Runtime.InteropServices.WindowsRuntime;

namespace X10SerialSlave.Server
{
    public interface IX10Controller
    {
        byte[] GetBytes();
        void WriteBytes([ReadOnlyArray] byte[] bytes);

        void Initialize();
    }
}
