using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace WsaPartner.ViewModels
{
    public class FileInstallAppViewModel : ObservableRecipient
    {
        private string _appPath;
        public FileInstallAppViewModel()
        {
        }

        public string AppPath
        {
            get { return _appPath; }

            set { SetProperty(ref _appPath, value); }
        }
    }
}
