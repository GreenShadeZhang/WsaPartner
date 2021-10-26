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

        private async void OnLoaded()
        {
            foreach (var fileDecoder in _fileDecoders)
            {
                if (fileDecoder is DefaultAPKDecoder)
                {
                    fileDecoder.SetFilePath(new Uri(_appPath));

                    await fileDecoder.Decode();

                    _targetPackageData = fileDecoder.GetDataModel();


                    //var image = _targetPackageData.MaxIconContent.AsBuffer().AsStream().AsRandomAccessStream();

                    //_bitmapImage = new BitmapImage();

                    //await _bitmapImage.SetSourceAsync(image);

                    //// decode image
                    //var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(Windows.Graphics.Imaging.BitmapDecoder.JpegDecoderId, image);

                    //image.Seek(0);

                    //// create bitmap
                    //var output = new WriteableBitmap((int)decoder.PixelHeight, (int)decoder.PixelWidth);

                    //await output.SetSourceAsync(image);

                    //_appIcon = output;

                    var fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";

                    var destinationFolder = 
                        await KnownFolders.PicturesLibrary.CreateFolderAsync("testapp",CreationCollisionOption.OpenIfExists);

                    var destinationFile = await destinationFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                    await FileIO.WriteBytesAsync(destinationFile, _targetPackageData.MaxIconContent);
                }
            }
        }
    }
}
