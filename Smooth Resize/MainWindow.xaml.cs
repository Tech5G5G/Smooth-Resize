using System;
using System.Runtime.InteropServices;
using Windows.UI.Composition.Desktop;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT;
using WinUIEx;

namespace Smooth_Resize
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        DesktopWindowTarget target;

        ISystemBackdropControllerWithTargets controller;

        public MainWindow()
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SystemBackdrop = new TransparentTintBackdrop();

            DispatcherQueue.EnsureSystemDispatcherQueue();
            var compositor = new Windows.UI.Composition.Compositor();
            var interop = compositor.As<ICompositorDesktopInterop>();
            interop.CreateDesktopWindowTarget((nint)AppWindow.Id.Value, isTopmost: false, out IntPtr targetPtr);

            target = DesktopWindowTarget.FromAbi(targetPtr);
            target.Root = compositor.CreateContainerVisual();

            controller = new MicaController();
            controller.SetTarget(AppWindow.Id, target);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (!(sender as ComboBox).IsLoaded)
                return;

            controller.RemoveAllSystemBackdropTargets();
            controller.Dispose();

            controller = ((sender as ComboBox).SelectedItem as string) switch
            {
                "Tabbed" => new MicaController { Kind = MicaKind.BaseAlt },
                "Acrylic" => new DesktopAcrylicController(),
                "Mica" or _ => new MicaController()
            };
            controller.SetTarget(AppWindow.Id, target);
        }
    }

    [ComImport]
    [Guid("29E691FA-4567-4DCA-B319-D0F207EB6807")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICompositorDesktopInterop
    {
        void CreateDesktopWindowTarget(IntPtr hwndTarget, bool isTopmost, out IntPtr result);
    }
}
