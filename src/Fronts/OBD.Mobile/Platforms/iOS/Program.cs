namespace OBD.Mobile;

public static class Program
{
    // The entry point of the app on iOS
    private static void Main(string[] args) => UIKit.UIApplication.Main(args, null, typeof(AppDelegate));
}
