using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using SharpAdbClient.DeviceCommands;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using WsaPartner.APKViewer;
using WsaPartner.Contracts.Services;
using SharpAdbClient;
using Microsoft.UI.Xaml.Controls;
using System;
using WsaPartner.Views;

namespace WsaPartner.ViewModels
{
    public class FileInstallAppViewModel : ObservableRecipient
    {
        private string _appPath;

        private string _iconPath;

        private readonly IEnumerable<IFileDecoder> _fileDecoders;

        private ICommand _loadedCommand;

        private PackageDataModel _targetPackageData = new PackageDataModel();

        private bool _isLoading;

        private bool _isOpen;

        private readonly AdbClient _adbClient;
        private readonly AdbServer _adbServer;

        private PackageManager _packageManager;

        private readonly IADBDeviceService _adbDeviceService;

        private ICommand _installAppCommand;

        private string _updateOrInstallText;

        private Page _page;
        public FileInstallAppViewModel(
            IEnumerable<IFileDecoder> decoders,
            AdbClient adbClient,
            AdbServer adbServer,
            IADBDeviceService deviceService)
        {
            _fileDecoders = decoders;
            _adbClient = adbClient;
            _adbServer = adbServer;
            _adbDeviceService = deviceService;
        }

        public void SetPage(Page page)
        {
            _page = page;
        }
        public string UpdateOrInstallText
        {
            get { return _updateOrInstallText; }

            set { SetProperty(ref _updateOrInstallText, value); }
        }

        public ICommand InstallAppCommand
        {
            get
            {
                if (_installAppCommand == null)
                {
                    _installAppCommand = new RelayCommand(
                        async () =>
                        {
                            var noWifiDialog = new ContentDialog
                            {
                                Title = "install apk",
                                Content = "是否安装？",
                                CloseButtonText = "Cancel",
                                PrimaryButtonText = "OK"
                            };

                            noWifiDialog.XamlRoot = _page.XamlRoot;

                            ContentDialogResult result = await noWifiDialog.ShowAsync();

                            if (result == ContentDialogResult.Primary)
                            {
                                IsLoading = true;

                                await Task.Run(() =>
                                {
                                    _packageManager.InstallPackage(AppPath, true);
                                });

                                IsLoading = false;
                            }
                        });
                }

                return _installAppCommand;
            }
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

        public string IconPath
        {
            get { return _iconPath; }

            set { SetProperty(ref _iconPath, value); }
        }
        
        public PackageDataModel TargetPackageData
        {
            get { return _targetPackageData; }

            set { SetProperty(ref _targetPackageData, value); }
        }

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(async () => { await OnLoaded(); }));

        private async Task OnLoaded()
        {
            IsLoading = true;

            var fileDecoder = Ioc.Default.GetService<IFileDecoder>();

            fileDecoder.SetFilePath(new System.Uri(_appPath));

            await Task.Run(async () =>
            {
                await fileDecoder.DecodeAsync();
            });

            var targetPackageData = fileDecoder.GetDataModel();

            IsLoading = false;

            IsOpen = true;

            TargetPackageData = targetPackageData;

            var ret = _adbServer.GetStatus();

            if (ret.IsRunning == true)
            {
                var device = _adbDeviceService.CheckDevie(_adbClient);

                if (device != null)
                {
                    _packageManager = new PackageManager(_adbClient, device);

                    UpdateOrInstallText = _adbDeviceService
                                    .VersionComparison(_packageManager, targetPackageData.PackageName, targetPackageData.VersionCode);
                }
            }
        }
    }
}
