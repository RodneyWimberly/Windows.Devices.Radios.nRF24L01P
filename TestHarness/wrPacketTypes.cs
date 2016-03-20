namespace nRF24L01P.TestHarness
{
    enum wrPacketTypes : byte
    {
        NotSet,
        X10StandardRequest,
        X10ExtendedRequest,
        X10ExtendedResponse,
        X10EventNotification,
        IrEventNotification,
        RfEventNotification,
        EnvironmentSensorRequest,
        EnvironmentSensorResponse,
        SetClockRequest,
        ReadClockRequest,
        ReadClockResponse
    }
}
