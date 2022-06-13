using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Bluetooth;

using Android.Content;
using Android.Views;
using Android.Widget;

using System.Linq;

using SmartLock.Droid;
using Xamarin.Essentials;
using System.Collections.Generic;
using Java.Util;
using SmartLock.Services;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.IO;
using Java.IO;
using System.Threading;
using System.Text;
using File = Java.IO.File;
using Environment = Android.OS.Environment;

[assembly: Dependency(typeof(ConnectBT))]
[assembly: Dependency(typeof(Toaster))]
[assembly: Dependency(typeof(FileService))]

namespace SmartLock.Droid
{
    [Activity(Label = "SmartLock", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class ConnectBT : IConnectBT
    {
        BluetoothAdapter _adapter = BluetoothAdapter.DefaultAdapter;
        BluetoothDevice _device;
        BluetoothSocket _socket;

        public async Task<int> MakeConnection(string name)
        {
            if (_adapter == null)
            {
                return 1;
            }

            if (!_adapter.IsEnabled)
            {
                return 2;
            }

            _device = (from bd in _adapter.BondedDevices
                       where bd.Address == "00:21:08:35:07:18"
                       select bd).FirstOrDefault();

            if (_device == null)
            {
                return 3;
            }

             _socket = _device.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            //_socket = _device.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString("0000112f-0000-1000-8000-00805f9b34fb"));
          
            try
            {
                if (!_socket.IsConnected)
                {
                    await _socket.ConnectAsync();                   
                }              
            }
            catch (Exception e)
            {
                string a = e.ToString();
                throw;
            }
            
            return 0;
        }

        public async Task<int> ReceiveInstraction()
        {
            try
            {
                Stream stream = _socket.InputStream;
                byte[] c = new byte[1];

                int n = 0;

                await this.SendData(3);
                await stream.ReadAsync(c);

                n = (int)c[0];  

                return n;               
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> ReceiveData()
        {
            try
            {
                Stream stream = _socket.InputStream;
                byte[] c = new byte[1024];

                string data = "";
                if (stream.CanRead)
                {
                    await this.SendData(3);

                    await stream.ReadAsync(c, 0, c.Length);

                    foreach (var item in c)
                    {
                        data += Convert.ToChar(item);
                    }
                }

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendData(byte data)
        {
            try
            {
                Stream stream = _socket.OutputStream;
                 stream.WriteByte(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendData(string id, string name)
        {
            try
            {
                Stream stream = _socket.OutputStream;
                byte[] bytes = Encoding.ASCII.GetBytes(id + " " + name);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendData(string data)
        {
            try
            {
                Stream stream = _socket.OutputStream;
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class Toaster : IToast
    {
        public void MakeToast(string message)
        {
            Toast.MakeText(Platform.AppContext, message, ToastLength.Long).Show();
        }
    }

    public class FileService : IFileService
    {
        public async Task SaveImage(string fileName, MemoryStream stream)
        {
            try
            {
                File myDir = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures),
                    "SmartLockCameraDownloaded");

                if (!myDir.Exists())
                    myDir.Mkdirs();

                File file = new File(myDir, fileName);

                if (file.Exists())
                    file.Delete();

                FileOutputStream outs = new FileOutputStream(file);
                await outs.WriteAsync(stream.ToArray());

                await outs.FlushAsync();
                outs.Close();

               
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string[] LoadPicture()
        {
            string[] fileNames = null;
            var documentsPath = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures),
                "SmartLockCamera");

            string a = documentsPath.Path;

            if (documentsPath.Exists())
            {
                fileNames = documentsPath.List();
            }

            for (int i = 0; i < fileNames.Length; i++)
            {
                fileNames[i] = a + "/" + fileNames[i];
            }

            return fileNames;
        }
    }
}