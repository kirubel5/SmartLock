using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SmartLock.Services
{
    //This interface is used to save images in the phone
    public interface IFileService
    {
        Task SaveImage(string name, MemoryStream stream);
        string[] LoadPicture();
    }

}
