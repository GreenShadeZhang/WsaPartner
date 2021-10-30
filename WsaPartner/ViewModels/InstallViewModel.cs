
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using WsaPartner.APKViewer;
using WsaPartner.Contracts.Services;
using WsaPartner.Contracts.ViewModels;
using WsaPartner.Helpers;

namespace WsaPartner.ViewModels
{
    public class InstallViewModel : ObservableRecipient, INavigationAware
    {
        private readonly SharpAdbClient.AdbClient _adbClient;
        private readonly SharpAdbClient.AdbServer _adbServer;

        private readonly IADBDeviceService _adbDeviceService;

        private DeviceData _device;
        private bool _isInstalling;

        private string _appPath;

        private Page _installPage;

        private bool _isLoading;

        private bool _isOpen;

        private PackageManager _packageManager;

        private string _selectBtnText;

        private string _updateOrInstallText;

        private PackageDataModel _targetPackageData = new PackageDataModel();
        public InstallViewModel(
            SharpAdbClient.AdbClient adbClient,
            SharpAdbClient.AdbServer adbServer,
            IADBDeviceService deviceService)
        {
            _adbClient = adbClient;
            _adbServer = adbServer;
            _adbDeviceService = deviceService;
        }

        public string SelectBtnText
        {
            get { return _selectBtnText; }

            set { SetProperty(ref _selectBtnText, value); }
        }

        public string UpdateOrInstallText
        {
            get { return _updateOrInstallText; }

            set { SetProperty(ref _updateOrInstallText, value); }
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

        public bool IsOpen
        {
            get { return _isOpen; }

            set { SetProperty(ref _isOpen, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }

            set { SetProperty(ref _isLoading, value); }
        }

        public string AppPath
        {
            get { return _appPath; }

            set { SetProperty(ref _appPath, value); }
        }

        public PackageDataModel TargetPackageData
        {
            get { return _targetPackageData; }

            set { SetProperty(ref _targetPackageData, value); }
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

                                IsLoading = true;

                                var fileDecoder = Ioc.Default.GetService<IFileDecoder>();

                                fileDecoder.SetFilePath(new System.Uri(file.Path));

                                await Task.Run(async () =>
                                {
                                    await fileDecoder.DecodeAsync();
                                });

                                var targetPackageData = fileDecoder.GetDataModel();

                                TargetPackageData = targetPackageData;

                                UpdateOrInstallText = _adbDeviceService
                                    .VersionComparison(_packageManager, targetPackageData.PackageName, targetPackageData.VersionCode);

                                IsLoading = false;

                                IsOpen = true;
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
                            await Task.Run(() =>
                            {
                                _packageManager.InstallPackage(AppPath, true);
                            });
                        });
                }

                return _installAppCommand;
            }
        }


        public async void OnNavigatedTo(object parameter)
        {
            SelectBtnText = "SelectApp".GetLocalized();

            await Task.Run(() =>
            {
                try
                {
                    var ret = _adbServer.GetStatus();

                    if (ret.IsRunning == true)
                    {
                        var device = _adbDeviceService.CheckDevie(_adbClient);

                        if (device != null)
                        {
                            _packageManager = new PackageManager(_adbClient, this._device);
                        }
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
            }
        }
    }
}
