
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using WsaPartner.Contracts.ViewModels;
using WsaPartner.Helpers;

namespace WsaPartner.ViewModels
{
    public class InstallViewModel : ObservableRecipient, INavigationAware
    {
        private readonly SharpAdbClient.AdbClient _adbClient;
        private readonly SharpAdbClient.AdbServer _adbServer;
        private DeviceData _device;
        private bool _isInstalling;
        private string _appPath;
        private Page _installPage;

        private PackageManager _packageManager;
        public InstallViewModel(SharpAdbClient.AdbClient adbClient, SharpAdbClient.AdbServer adbServer)
        {
            _adbClient = adbClient;
            _adbServer = adbServer;
        }

        public void SetWindow(Page installPage)
        {
            _installPage = installPage;
        }

        public void OnNavigatedFrom()
        {
            ADBHelper.Monitor.DeviceChanged -= OnDeviceChanged;
        }


        public bool IsInstalling
        {
            get { return _isInstalling; }

            set { SetProperty(ref _isInstalling, value); }
        }

        public string AppPath
        {
            get { return _appPath; }

            set { SetProperty(ref _appPath, value); }
        }

        private ICommand _selectAppCommand;

        public ICommand SelectAppCommand
        {
            get
            {
                if (_selectAppCommand == null)
                {
                    _selectAppCommand = new RelayCommand(
                        async () =>
                        {
                            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

                            var picker = new Windows.Storage.Pickers.FileOpenPicker();

                            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;

                            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;

                            picker.FileTypeFilter.Add(".apk");

                            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

                            if (file != null)
                            {
                                // Application now has read/write access to the picked file
                                this.AppPath = file.Path;
                            }
                            else
                            {
                                this.AppPath = "Operation cancelled.";
                            }
                        });
                }

                return _selectAppCommand;
            }
        }

        private ICommand _installAppCommand;

        public ICommand InstallAppCommand
        {
            get
            {
                if (_installAppCommand == null)
                {
                    _installAppCommand = new RelayCommand(
                        async () =>
                        {
                            _packageManager.InstallPackage(AppPath, true);
                        });
                }

                return _installAppCommand;
            }
        }


        public async void OnNavigatedTo(object parameter)
        {
            await Task.Run(() =>
            {
                try
                {
                    _adbServer.StartServer($@"{AppDomain.CurrentDomain.BaseDirectory}\AdbTools\adb.exe", restartServerIfNewer: false);
                    _adbClient.Connect(new DnsEndPoint("127.0.0.1", 58526));
                    ADBHelper.Monitor.DeviceChanged += OnDeviceChanged;
                    if (CheckDevice() && _device != null)
                    {
                        _packageManager = new PackageManager(_adbClient, this._device);
                        //CheckAPK();
                    }
                }
                catch (Exception ex)
                {

                }

            });
        }

        private void OnDeviceChanged(object sender, SharpAdbClient.DeviceDataEventArgs e)
        {
            if (!IsInstalling)
            {
                //DispatcherQueue.TryEnqueue(() =>
                //{
                //    _adbClient.Connect(new DnsEndPoint("127.0.0.1", 58526));
                //    if (CheckDevice() && _device != null)
                //    {
                //        var manager = new PackageManager(_adbClient, this._device);
                //        //CheckAPK();
                //    }
                //});
            }
        }

        private bool CheckDevice()
        {
            IList<DeviceData> devices = _adbClient.GetDevices();

            if (devices.Count <= 0) { return false; }

            foreach (DeviceData device in devices)
            {
                if (device.Model.Contains("Subsystem_for_Android_TM_"))
                {
                    this._device = device ?? this._device;
                    return true;
                }
            }
            return false;
        }
    }
}
