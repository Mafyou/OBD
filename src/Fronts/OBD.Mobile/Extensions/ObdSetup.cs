namespace OBD.Mobile.Extensions;

/// <summary>
/// What the app takes from its builder, one domain per member: MauiProgram only puts them together with the services
/// (AddData, AddServices, AddPagesViewModels).
/// </summary>
public static class ObdSetup
{
    extension(MauiAppBuilder builder)
    {
        /// <summary>Geist for the titles and the Material Symbols icons.</summary>
        public MauiAppBuilder UseObdFonts() => builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("Geist-SemiBold.ttf", "GeistSemiBold");
            fonts.AddFont("MaterialSymbolsOutlined-SemiBold.ttf", "MaterialSymbols");
        });

        /// <summary>The debug output, in Debug only.</summary>
        public MauiAppBuilder UseObdLogging()
        {
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder;
        }
    }
}
