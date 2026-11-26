namespace OBD.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseObdFonts()
            .UseObdLogging();

        builder.Services
            .AddData()
            .AddServices()
            .AddPagesViewModels();

        return builder.Build();
    }
}
