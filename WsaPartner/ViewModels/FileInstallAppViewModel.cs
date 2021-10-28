using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Storage;
using WsaPartner.APKViewer;
using WsaPartner.APKViewer.Decoders;

namespace WsaPartner.ViewModels
{
    public class FileInstallAppViewModel : ObservableRecipient
    {
        private string _appPath;

        private string _iconPath;

        private WriteableBitmap _appIcon;

        private BitmapImage _bitmapImage;

        private readonly IEnumerable<IFileDecoder> _fileDecoders;

        private ICommand _loadedCommand;

        private PackageDataModel _targetPackageData;

        public FileInstallAppViewModel(IEnumerable<IFileDecoder> decoders)
        {
            _fileDecoders = decoders;
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

        public WriteableBitmap AppIcon
        {
            get { return _appIcon; }
            set { SetProperty(ref _appIcon, value); }
        }

        public BitmapImage BitmapImage
        {
            get { return _bitmapImage; }
            set { SetProperty(ref _bitmapImage, value); }
        }

        public PackageDataModel TargetPackageData
        {
            get { return _targetPackageData; }

            set { SetProperty(ref _targetPackageData, value); }
        }

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        private void OnLoaded()
        {
         
        }
    }
}
