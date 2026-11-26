namespace OBD.Mobile;

[Android.App.Application]
public class MainApplication(IntPtr handle, Android.Runtime.JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
