using Android.App;
using Android.Runtime;

namespace c971_project
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override void OnCreate()
        {
            base.OnCreate();

            // The plugin automatically creates channels when .UseLocalNotification() is called
            // No need for manual channel creation in newer versions
        }
    }
}