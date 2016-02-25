using System;

namespace nRF24L01P.TestHarness
{
    public interface IReadAndWriteProcess : IDisposable
    {
        void Initialize();
    }
}
