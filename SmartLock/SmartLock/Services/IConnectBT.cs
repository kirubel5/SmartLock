using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartLock.Services
{
    //This is an interface defining Methods used to connect, send and recieve data Via bluetooth
    public interface IConnectBT
    {
        Task<int> MakeConnection(string name);
        Task SendData(byte data);
        Task SendData(string id, string name);
        Task SendData(string data);
        Task<int> ReceiveInstraction();
        Task<string> ReceiveData();
    }
}
