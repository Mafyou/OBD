namespace OBD.Mobile.Views;

/// <summary>
/// The launch splash laid over the window by <see cref="BrandSplash"/>: the name of the app in a frame, "by Mafyou"
/// at the foot of the screen, and a line that fills up over <see cref="BrandSplash.Duration"/>. Then, once the app is
/// ready underneath, it slides up off the screen, uncovering the app from the bottom up, and raises
/// <see cref="Closed"/>. It has the same layout in every app, in the colours of
/// this one (its Colors.xaml). <see cref="Skip"/> ("Skip", a tap anywhere, Back) cuts the 5 seconds short; never the loading of the app.
/// </summary>
public partial class BrandSplashView : ContentView
{
    // Cancelled by a skip: the wait ends where it is, and the splash goes once the app is ready
    private readonly CancellationTokenSource _skip = new();
    private readonly Func<Task> _whenAppReady;

    private bool _started;
    private bool _closed;

    // The name in its frame: as large as it can be up to NameSize, never a word cut in two (a long name such as
    // "Commandements" ran past the frame and broke in the middle of the word)
    private const double NameSize = 40;
    private const double SmallestNameSize = 22;

    /// <param name="appName">The name, in large letters.</param>
    /// <param name="lead">A line above the name, if the app frames it with a phrase.</param>
    /// <param name="tagline">A line under the name, if the app frames it with a phrase.</param>
    /// <param name="whenAppReady">Completes once the app underneath is ready to be seen.</param>
    /// <param name="nameFont">The font the app writes its name in (a font it registers), if any.</param>
    public BrandSplashView(string appName, string? lead, string? tagline, Func<Task> whenAppReady, string? nameFont = null)
    {
        InitializeComponent();
        _whenAppReady = whenAppReady;

        AppName.Text = appName;
        if (!string.IsNullOrWhiteSpace(nameFont)) AppName.FontFamily = nameFont;
        ShowLine(Lead, lead);
        ShowLine(Tagline, tagline);
        SkipButton.Text = AppResources.GetString("Splash_Skip");
        SemanticProperties.SetHint(SkipButton, AppResources.GetString("Splash_SkipHint"));
        Paint();

        Loaded += OnLoaded;
        Root.SizeChanged += OnRootSizeChanged;
    }

    // An exception in an event handler ends the app on iOS: the name keeps its size rather than take the app down
    private void OnRootSizeChanged(object? sender, EventArgs e)
    {
        try
        {
            FitName();
        }
        catch (Exception exception)
        {
            BrandSplash.Warn(exception, "The name could not be fitted to the screen");
        }
    }

    /// <summary>The splash has slid away: the app is uncovered and the splash can be taken off the window.</summary>
    public event EventHandler? Closed;

    /// <summary>Opens the app as soon as it is ready, whatever is left of the 5 seconds. Does nothing once the splash is going.</summary>
    public void Skip()
    {
        // The button stays enabled: an app's style greys a disabled button, which would flash as the splash slides
        if (_skip.IsCancellationRequested) return;
        _skip.Cancel();
    }

    /// <summary>Plays the splash from its first frame. Called by <see cref="BrandSplash"/> when the platform never
    /// says the view is loaded, so that the app is never left covered.</summary>
    internal void Start() => OnLoaded(this, EventArgs.Empty);

    /// <summary>The largest size at which every word of the name fits on a line of the frame, the screen as it is.</summary>
    private void FitName()
    {
        var available = Root.Width - Titles.Padding.HorizontalThickness - NameFrame.Padding.HorizontalThickness
            - 2 * NameFrame.StrokeThickness;
        var name = AppName.Text;
        if (available <= 0 || string.IsNullOrWhiteSpace(name)) return;

        var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        AppName.LineBreakMode = LineBreakMode.NoWrap;

        var size = NameSize;
        for (; size > SmallestNameSize; size -= 2)
        {
            AppName.FontSize = size;
            if (words.All(word => WidthOf(word) <= available)) break;
        }

        AppName.FontSize = size;
        AppName.Text = name;
        AppName.LineBreakMode = LineBreakMode.WordWrap;

        double WidthOf(string word)
        {
            AppName.Text = word;
            return AppName.Measure(double.PositiveInfinity, double.PositiveInfinity).Width;
        }
    }

    private static void ShowLine(Label line, string? text)
    {
        line.Text = text;
        line.IsVisible = !string.IsNullOrWhiteSpace(text);
    }

    // The view is not in the app's tree: its colours are read from the app's resources (Colors.xaml)
    private void Paint()
    {
        var (backgroundLight, backgroundDark) = Pair("BrandSplashBackground");
        var (titleLight, titleDark) = Pair("BrandSplashTitle");
        var (signatureLight, signatureDark) = Pair("BrandSplashSignature");
        var (accentLight, accentDark) = Pair("BrandSplashAccent");
        // The line takes the accent unless the app gives it a colour of its own
        var (lineLight, lineDark) = HasPair("BrandSplashLine") ? Pair("BrandSplashLine") : (accentLight, accentDark);

        Root.SetAppThemeColor(BackgroundColorProperty, backgroundLight, backgroundDark);
        AppName.SetAppThemeColor(Label.TextColorProperty, titleLight, titleDark);
        Lead.SetAppThemeColor(Label.TextColorProperty, titleLight, titleDark);
        Tagline.SetAppThemeColor(Label.TextColorProperty, titleLight, titleDark);
        Signature.SetAppThemeColor(Label.TextColorProperty, signatureLight, signatureDark);
        NameFrame.SetAppTheme<Brush>(Border.StrokeProperty, new SolidColorBrush(accentLight), new SolidColorBrush(accentDark));
        Progress.SetAppThemeColor(BoxView.ColorProperty, lineLight, lineDark);
        SkipButton.SetAppThemeColor(Button.TextColorProperty, signatureLight, signatureDark);
        SkipButton.SetAppThemeColor(Button.BorderColorProperty, signatureLight, signatureDark);
    }

    /// <summary>"BrandSplashTitle" gives BrandSplashTitleLight and BrandSplashTitleDark.</summary>
    private static (Color Light, Color Dark) Pair(string name) => (Resource($"{name}Light"), Resource($"{name}Dark"));

    private static bool HasPair(string name) =>
        Application.Current?.Resources is { } resources
        && resources.TryGetValue($"{name}Light", out _) && resources.TryGetValue($"{name}Dark", out _);

    private static Color Resource(string key) =>
        Application.Current?.Resources.TryGetValue(key, out var value) is true && value is Color color
            ? color
            : throw new KeyNotFoundException($"Colors.xaml has no colour {key}");

    private async void OnLoaded(object? sender, EventArgs e)
    {
        if (_started) return;
        _started = true;
        var start = System.Diagnostics.Stopwatch.GetTimestamp();

        try
        {
            _ = Progress.ScaleXToAsync(1, (uint)BrandSplash.Duration.TotalMilliseconds, Easing.Linear);
            _ = SkipButton.FadeToAsync(1, 400);
            await PlayEntranceAsync(_skip.Token);

            var left = BrandSplash.Duration - System.Diagnostics.Stopwatch.GetElapsedTime(start);
            if (left > TimeSpan.Zero) await Task.Delay(left, _skip.Token);
        }
        catch (System.OperationCanceledException)
        {
            // Skipped
        }
        catch (Exception exception)
        {
            // An animation that fails must never keep the app covered
            BrandSplash.Warn(exception, "The splash could not play");
        }

        // The splash is the time the app loads in: it only goes once the first page is there, whole
        try
        {
            if (_skip.IsCancellationRequested) Progress.CancelAnimations();
            await _whenAppReady();
        }
        catch (Exception exception)
        {
            BrandSplash.Warn(exception, "Could not tell whether the app was ready");
        }

        await CloseAsync();
    }

    // The titles rise into place, the frame of the name settles, then the signature appears: about 1.2 s
    private async Task PlayEntranceAsync(CancellationToken stoppingToken)
    {
        await Task.WhenAll(
            Titles.FadeToAsync(1, 500, Easing.CubicOut),
            Titles.TranslateToAsync(0, 0, 500, Easing.CubicOut));
        stoppingToken.ThrowIfCancellationRequested();

        await NameFrame.ScaleToAsync(1, 350, Easing.SpringOut);
        stoppingToken.ThrowIfCancellationRequested();

        await Signature.FadeToAsync(1, 350, Easing.CubicIn);
    }

    private async Task CloseAsync()
    {
        if (_closed) return;
        _closed = true;

        try
        {
            // The splash slides up off the screen: the app shows from the bottom up as it goes
            Progress.CancelAnimations();
            await this.TranslateToAsync(0, -Height, 600, Easing.CubicIn);
        }
        catch (Exception exception)
        {
            BrandSplash.Warn(exception, "The splash could not slide away");
        }

        try
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception exception)
        {
            BrandSplash.Warn(exception, "The splash could not be taken off the window");
        }
    }

    private void OnSkip(object? sender, EventArgs e) => Skip();

    private void OnTapped(object? sender, TappedEventArgs e) => Skip();
}
