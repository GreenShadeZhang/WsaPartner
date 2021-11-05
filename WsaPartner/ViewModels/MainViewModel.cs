using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using SharpAdbClient;
using System;
using System.Net;
using System.Windows.Input;
using WsaPartner.Contracts.Services;
using WsaPartner.Views;

namespace WsaPartner.ViewModels
{
    public class MainViewModel : ObservableRecipient
    {
        private readonly AdbClient _adbClient;
        private readonly AdbServer _adbServer;
        private DeviceData _device;

        private bool _isConnected;

        private readonly IADBDeviceService _adbDeviceService;

        private ICommand _loadedCommand;
        public MainViewModel(
            AdbClient adbClient,
            AdbServer adbServer,
            IADBDeviceService deviceService)
        {
            _adbClient = adbClient;
            _adbServer = adbServer;
            _adbDeviceService = deviceService;

            _adbServer.StartServer($@"{AppDomain.CurrentDomain.BaseDirectory}\CMDTools\adb.exe", restartServerIfNewer: false);
        }

        public DeviceData Device
        {
            get { return _device; }

            set { SetProperty(ref _device, value); }
        }


        public bool IsConnected
        {
            get { return _isConnected; }

            set { SetProperty(ref _isConnected, value); }
        }

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand<object>(OnLoaded));

        private void OnLoaded(object obj)
        {
            var inAppNoty = obj as InAppNotification;

            //var content =  App.MainWindow.Content as ShellPage;
            //inAppNoty.XamlRoot = App.MainWindow.Content.XamlRoot;

            //inAppNoty.Show();

            try
            {
                _adbClient.Connect(new DnsEndPoint("127.0.0.1", 58526));

                Device = _adbDeviceService.CheckDevie(_adbClient);

                if (_device != null)
                {
                    IsConnected = true;
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
            }
        }
    }
}
