
using CommunityToolkit.Mvvm.ComponentModel;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using WsaPartner.Contracts.Services;
using WsaPartner.Contracts.ViewModels;
using WsaPartner.Core.Models;

namespace WsaPartner.ViewModels
{
    public class AppsViewModel : ObservableRecipient, INavigationAware
    {
        private readonly AdbClient _adbClient;
        private readonly AdbServer _adbServer;
        private DeviceData _device;
        private bool _isInstalling;
        private readonly IADBDeviceService _adbDeviceService;

        private Dictionary<string, string> _packages;

        private List<WsaApp> _apps;

        private PackageManager _packageManager;
        public AppsViewModel(
            AdbClient adbClient,
            AdbServer adbServer,
            IADBDeviceService deviceService)
        {
            _adbClient = adbClient;
            _adbServer = adbServer;
            _adbDeviceService = deviceService;
        }

        public void OnNavigatedFrom()
        {
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

                    if (_packageManager != null)
                    {
                        _packages = _packageManager.Packages;
                    }
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
    }
}
