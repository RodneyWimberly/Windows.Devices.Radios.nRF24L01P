using System;

namespace nRF24L01P.TestHarness
{
    public enum HeaderTypes
    {
        /* These types will NOT receive network acknowledgements */
        notSet = 0,
        rePowerLineEvent = 1,
        reSerialDataEvent = 2,
        reInfaredEvent = 3,
        reRadioFrequencyEvent = 4,

        /* These types will receive network acknowledgements */
        hidGetKey = 66,
        hidWaitForKey = 67,
        hidLcdClear = 68,
        hidLcdSetCursor = 69,
        hidLcdWrite = 70,
        hidLcdWriteAt = 71,
        hidLcdCustomCharacter = 72,
        hidLcdCustomCharacterAt = 73,
        hidLcdCommand = 74,
        reX10Standard = 75,
        reX10ModuleStatus = 76,
        reEnvironmentSensor = 77,
        reSetClock = 78,
        reReadClock = 79,
    }

    internal struct X10Data
    {
        private char houseCode;
        private byte unitCode;
        private byte commandCode;
        private byte extendedData;
        private byte extendedCommand;
        private bool success;
    }

    internal struct X10ModuleStatus
    {
        private char houseCode;
        private byte unitCode;
        private byte type;
        private string name;
        private bool stateIsKnown;
        private bool stateIsOn;
        private byte dimPercentage;
        private bool success;
    }

    internal struct EnvironmentData
    {
        private float humidity;
        private float temperatureC;
        private float temperatureF;
        private float heatIndexC;
        private float heatIndexH;
        private bool success;
    }

    internal struct ClockData
    {
        private DateTime time;
        private bool success;
    }
}
