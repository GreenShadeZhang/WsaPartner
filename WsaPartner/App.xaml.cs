using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using SharpAdbClient;
using System.Linq;
using WsaPartner.Activation;
using WsaPartner.Contracts.Services;
using WsaPartner.Helpers;
using WsaPartner.Services;
using WsaPartner.ViewModels;
using WsaPartner.Views;
using WinRT;
using WsaPartner.APKViewer;
using WsaPartner.APKViewer.Decoders;

// To learn more about WinUI3, see: https://docs.microsoft.com/windows/apps/winui/winui3/.
namespace WsaPartner
{
    public partial class App : Application
    {
        public static Window MainWindow { get; set; } = new Window() { Title = "AppDisplayName".GetLocalized() };

        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/windows/winui/api/microsoft.ui.xaml.unhandledexceptioneventargs
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            IWindowNative windowNative = MainWindow.As<IWindowNative>();

            var m_windowHandle = windowNative.WindowHandle;
            // The Window object doesn't have Width and Height properties in WInUI 3 Desktop yet.
            // To set the Width and Height, you can use the Win32 API SetWindowPos.
            // Note, you should apply the DPI scale factor if you are thinking of dpi instead of pixels.
            SetWindowSize(m_windowHandle, 800, 600);

            Windows.ApplicationModel.Activation.IActivatedEventArgs fileArgs = 
                Windows.ApplicationModel.AppInstance.GetActivatedEventArgs();

            switch (fileArgs.Kind)
            {
                case Windows.ApplicationModel.Activation.ActivationKind.File:
                    var path = (fileArgs as Windows.ApplicationModel.Activation.IFileActivatedEventArgs).Files.First().Path;

                    var  appPage = Ioc.Default.GetService<FileInstallAppPage>();

                    var fileDecoder = Ioc.Default.GetService<IFileDecoder>();

                    fileDecoder.SetFilePath(new System.Uri(path));

                    await fileDecoder.Decode();

                    var targetPackageData = fileDecoder.GetDataModel();

                    appPage.ViewModel.TargetPackageData = targetPackageData;

                    appPage.ViewModel.AppPath = path;

                    App.MainWindow.Content = appPage;

                    App.MainWindow.Activate();
                    // to do
                    break;
                default:
                    var activationService = Ioc.Default.GetService<IActivationService>();
                    await activationService.ActivateAsync(args);
                    break;
            }                  
        }

        private void SetWindowSize(System.IntPtr hwnd, int width, int height)
        {
            int dpi = PInvoke.User32.GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);

            _ = PInvoke.User32.SetWindowPos(hwnd, PInvoke.User32.SpecialWindowHandles.HWND_TOP,
                                        0, 0, width, height,
                                        PInvoke.User32.SetWindowPosFlags.SWP_NOMOVE);
        }

        [System.Runtime.InteropServices.ComImport]
        [System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
        [System.Runtime.InteropServices.Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative
        {
            System.IntPtr WindowHandle { get; }
        }

        private System.IServiceProvider ConfigureServices()
        {
            // TODO WTS: Register your services, viewmodels and pages here
            var services = new ServiceCollection();

            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();
            services.AddTransient<IADBDeviceService, ADBDeviceService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<AdbServer>();
            services.AddSingleton<AdbClient>();
            services.AddSingleton<ICmdPathProvider, WindowsCmdPath>();
            services.AddSingleton<IFileDecoder, DefaultAABDecoder>();
            services.AddSingleton<IFileDecoder, DefaultAPKDecoder>();
            // Core Services

            // Views and ViewModels
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<BlankViewModel>();
            services.AddTransient<BlankPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<InstallViewModel>();
            services.AddTransient<InstallPage>();
            services.AddTransient<AppsViewModel>();
            services.AddTransient<AppsPage>();
            services.AddTransient<FileInstallAppViewModel>();
            services.AddTransient<FileInstallAppPage>();
            return services.BuildServiceProvider();
        }
    }
}
