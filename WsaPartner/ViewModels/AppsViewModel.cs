
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using WsaPartner.Contracts.Services;
using WsaPartner.Contracts.ViewModels;
using WsaPartner.Core.Models;
using WsaPartner.Helpers;

namespace WsaPartner.ViewModels
{
    public class AppsViewModel : ObservableRecipient, INavigationAware
    {
        private readonly SharpAdbClient.AdbClient _adbClient;
        private readonly SharpAdbClient.AdbServer _adbServer;
        private DeviceData _device;
        private bool _isInstalling;
        private string _appPath;
        private Page _installPage;
        private readonly IADBDeviceService _adbDeviceService;

        private Dictionary<string, string> _packages;

        private List<WsaApp> _apps;

        private PackageManager _packageManager;
        public AppsViewModel(
            SharpAdbClient.AdbClient adbClient, 
            SharpAdbClient.AdbServer adbServer, IADBDeviceService deviceService)
        {
            _adbClient = adbClient;
            _adbServer = adbServer;
            _adbDeviceService = deviceService;
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

        public List<WsaApp> Apps
        {
            get { return _apps; }

            set { SetProperty(ref _apps, value); }
        }

        public Dictionary<string, string> Packages
        {
            get { return _packages; }

            set { SetProperty(ref _packages, value); }
        }


        public void OnNavigatedTo(object parameter)
        {
            try
            {
                var status = _adbServer.GetStatus();

                _device = _adbDeviceService.CheckDevie(_adbClient);

                if (_device != null)
                {
                    _packageManager = new PackageManager(_adbClient, this._device);

                    if(_packageManager != null)
                    {
                        _packages = _packageManager.Packages;                       
                    }
                    //CheckAPK();
                }

                if (_packages != null && _packages.Count > 0)
                {
                    var appList = new List<WsaApp>();

                    foreach (var package in _packages)
                    {
                        var app = new WsaApp();

                        var version = _packageManager.GetVersionInfo(package.Key);

                        app.Version = $"{version.VersionName}";
                        app.Name = package.Key;
                        app.PackageName = package.Key;
                        app.Path = package.Value;
                        appList.Add(app);
                    }

                    Apps = appList;
                }
            }
            catch (Exception ex)
            {

            }
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
    }
}
