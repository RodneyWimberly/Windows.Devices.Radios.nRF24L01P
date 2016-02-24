using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace nRF24L01P.TestHarness
{
    public interface IReadAndWriteProcess : IDisposable
    {
        void Initialize();
        byte[] GetBytes();
        void WriteBytes([ReadOnlyArray] byte[] bytes);
    }
}
