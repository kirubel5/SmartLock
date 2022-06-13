using System;
using System.Collections.Generic;
using System.Text;

namespace SmartLock.Services
{
    public enum Instruction
    {
        ImageTooMessy = 10,
        FingerprintsDontMatch = 13,
        CancelDeleteOrder = 130,
        FingerprintRegisteredSuccessfully = 14,
        LoadDataCommand = 20,
        CheckPin = 32,
        PinNotRegistered = 33,
        PinCorrect = 34,
        PinIncorrect = 35,
        FingerprintSensorNotFound = 4,
        RegisterFingerPrintCommand = 50,
        DeleteFingerprintCommand = 51,
        ClearFingerprintDatabaseCommand = 52,
        ClearFeedCommand = 53,
        CommunicationError = 7,
        ImagingError = 8,
        UnknownError = 9

    }
}
