namespace OBD.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Geist-SemiBold.ttf", "GeistSemiBold");
                fonts.AddFont("MaterialSymbolsOutlined-SemiBold.ttf", "MaterialSymbols");
            });

        builder.Services.AddData();

        builder.Services.AddServices();

        builder.Services.AddPagesViewModels();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}