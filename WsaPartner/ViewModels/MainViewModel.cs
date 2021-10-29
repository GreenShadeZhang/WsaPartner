using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using WsaPartner.Contracts.Services;
using WsaPartner.Contracts.ViewModels;
using WsaPartner.Helpers;

namespace WsaPartner.ViewModels
{
    public class MainViewModel : ObservableRecipient, INavigationAware
    {
        private readonly SharpAdbClient.AdbClient _adbClient;
        private readonly SharpAdbClient.AdbServer _adbServer;
        private DeviceData _device;
        private bool _isInstalling;
        private string _appPath;

        private bool _isConnected;

        private PackageManager _packageManager;

        private readonly INavigationService _navigationService;
        private readonly IADBDeviceService _adbDeviceService;
        public MainViewModel(
            INavigationService navigationService, 
            SharpAdbClient.AdbClient adbClient, 
            SharpAdbClient.AdbServer adbServer,
            IADBDeviceService deviceService)
        {
            _navigationService = navigationService;
            _adbClient = adbClient;
            _adbServer = adbServer;
            _adbDeviceService = deviceService;
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

        private ICommand _gridViewItemCommand;

        public ICommand GridViewItemCommand
        {
            get
            {
                if (_gridViewItemCommand == null)
                {
                    _gridViewItemCommand = new RelayCommand<string>(
                        async (param) =>
                        {
                            _navigationService.NavigateTo(typeof(InstallViewModel).FullName, param);
                        });
                }

                return _gridViewItemCommand;
            }
        }

        public void OnNavigatedTo(object parameter)
        {
            try
            {
                _adbServer.StartServer($@"{AppDomain.CurrentDomain.BaseDirectory}\CMDTools\adb.exe", restartServerIfNewer: false);

                _adbClient.Connect(new DnsEndPoint("127.0.0.1", 58526));

                //ADBHelper.Monitor.DeviceChanged += OnDeviceChanged;

                _device = _adbDeviceService.CheckDevie(_adbClient);

                if (_device != null)
                {
                    _packageManager = new PackageManager(_adbClient, this._device);
                    IsConnected = true;
                    //CheckAPK();
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
            }
        }

        public void OnNavigatedFrom()
        {
            //throw new NotImplementedException();
        }
    }
}
