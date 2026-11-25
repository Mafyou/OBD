namespace OBD.Mobile.Views;

/// <summary>
/// The launch splash, the same in every Mafyou IT app: the name of the app and its signature, "by Mafyou", for at least 5 seconds, which
/// "Skip", a tap anywhere or Back cut short. It is laid over the whole window, above the app's first page, as soon as
/// the window exists: the app loads underneath meanwhile and its navigation is left as it is (a notification opens its
/// page under the splash). The splash is the time the app loads in: it only goes, at the end of its time or on a skip,
/// once the app's first page is on display and laid out, and once what the app holds it for (<see cref="HoldUntil"/>)
/// has loaded. Nothing is left to appear behind it. Once per launch of the process: a window Android rebuilds (a
/// foldable opened) opens without it.
/// </summary>
/// <example>
/// In App.CreateWindow, with the first page of a first launch or of the others:
/// <code>
/// var window = new Window(startPage);
/// BrandSplash.ShowOver&lt;PersonsPage&gt;(window, "OBD");
/// return window;
/// </code>
/// and where the first page loads what it shows:
/// <code>
/// BrandSplash.HoldUntil(loading);
/// </code>
/// </example>
public static partial class BrandSplash
{
    /// <summary>From the first frame of the splash to the moment it goes, when the app is ready by then.</summary>
    public static readonly TimeSpan Duration = TimeSpan.FromSeconds(5);

    /// <summary>How long the splash waits, after its time or a skip, for an app that is not ready yet. Past it the
    /// splash goes anyway: an app stuck loading must not stay hidden behind it.</summary>
    public static readonly TimeSpan ReadyPatience = TimeSpan.FromSeconds(15);

    // How often the app is looked at while the splash waits for it
    private static readonly TimeSpan ReadyStep = TimeSpan.FromMilliseconds(100);

    // When the platform never says the view is loaded, it is started anyway; and whatever happens, the splash is
    // taken off the window a little after its longest wait, so that the app is never left covered
    private static readonly TimeSpan StartPatience = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan RemovalPatience = StartPatience + Duration + ReadyPatience + TimeSpan.FromSeconds(3);

    private static readonly Lock Gate = new();
    private static readonly List<Task> Loadings = [];

    private static int _popupsWaiting;
    private static bool _requested;
    private static bool _shown;
    private static TaskCompletionSource? _gone;

    /// <summary>The splash on display, if any.</summary>
    public static BrandSplashView? Current { get; private set; }

    /// <summary>While the splash covers the app, from the moment the app asks for it (its window not created yet) to
    /// the moment it has gone: a page that starts meanwhile shows its final state rather than an entrance.</summary>
    public static bool IsShowing => Current is not null || (_requested && !_shown);

    /// <summary>
    /// Lays the splash over <paramref name="window"/> once it is created; call it in CreateWindow. The splash stays
    /// until a <typeparamref name="TFirstPage"/> is on display and laid out: name the app's first real page, not a
    /// loading page that gives way to it.
    /// </summary>
    /// <param name="appName">The name, in large letters.</param>
    /// <param name="lead">A line above the name, for an app that frames it with a phrase.</param>
    /// <param name="tagline">A line under the name, for an app that frames it with a phrase.</param>
    /// <param name="nameFont">The font the app writes its name in, as it does in its own pages.</param>
    public static void ShowOver<TFirstPage>(Window window, string appName, string? lead = null, string? tagline = null,
        string? nameFont = null)
        where TFirstPage : Page =>
        ShowOver(window, appName, lead, tagline, nameFont, static page => page is TFirstPage);

    /// <summary>As <see cref="ShowOver{TFirstPage}"/>, the splash staying until whatever page is on display is laid out.</summary>
    public static void ShowOver(Window window, string appName, string? lead = null, string? tagline = null,
        string? nameFont = null) =>
        ShowOver(window, appName, lead, tagline, nameFont, static _ => true);

    /// <summary>
    /// Keeps the splash on display until <paramref name="loading"/> has completed (failed or not), within
    /// <see cref="ReadyPatience"/>: what the first page loads as the app starts, so that it is on display, whole, as
    /// the splash goes. Nothing happens once the splash has gone.
    /// </summary>
    public static void HoldUntil(Task loading)
    {
        if (_shown && !IsShowing) return;

        lock (Gate)
        {
            Loadings.RemoveAll(static task => task.IsCompleted);
            Loadings.Add(loading);
        }
    }

    /// <summary>
    /// Completes once the splash is off the window, at once when none is on display. On Android a popup or a modal
    /// page opens in a window of its own, above the splash: what an app opens as it starts waits for this first
    /// (the crash report mail does for every popup).
    /// </summary>
    public static Task WhenGoneAsync() => Volatile.Read(ref _gone)?.Task ?? Task.CompletedTask;

    /// <summary>
    /// For the crash report mail: a popup the app asks for waits for the splash to go. The app asking for one has
    /// loaded what it shows the popup for: the splash stops waiting for the loadings it holds it for, which may well
    /// be the very ones waiting for this popup to close.
    /// </summary>
    internal static async Task LetPopupThroughAsync()
    {
        if (Volatile.Read(ref _gone) is not { } gone) return;

        Interlocked.Increment(ref _popupsWaiting);
        try
        {
            await gone.Task;
        }
        finally
        {
            Interlocked.Decrement(ref _popupsWaiting);
        }
    }

    /// <summary>Opens the app now if the splash is on display, or as soon as it is ready. True when it was on display.</summary>
    public static bool Skip()
    {
        if (Current is not { } splash) return false;
        splash.Skip();
        return true;
    }

    private static void ShowOver(Window window, string appName, string? lead, string? tagline, string? nameFont,
        Func<Page, bool> isFirstPage)
    {
        if (_shown) return;
        _requested = true;

        // From now on, what the app opens waits for the splash: its first page may start before the window exists
        if (Volatile.Read(ref _gone) is null)
            Volatile.Write(ref _gone, new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously));

        if (window.Handler is not null)
        {
            Show(window, appName, lead, tagline, nameFont, isFirstPage);
            return;
        }

        window.Created += OnCreated;

        void OnCreated(object? sender, EventArgs e)
        {
            window.Created -= OnCreated;
            // Raised while the platform creates the window: whatever goes wrong, the app opens without the splash
            Safely(() => Show(window, appName, lead, tagline, nameFont, isFirstPage), "The splash could not be shown");
            if (Current is null) LetEverythingThrough();
        }
    }

    /// <summary>Notes a failure of the splash in the app's log; the app opens all the same.</summary>
    internal static void Warn(Exception exception, string message) =>
        Logger?.LogWarning(exception, "[BrandSplash] {Message}", message);

    /// <summary>Notes in the app's log something the splash did not expect.</summary>
    internal static void Warn(string message) => Logger?.LogWarning("[BrandSplash] {Message}", message);

    private static ILogger? Logger =>
        IPlatformApplication.Current?.Services.GetService<ILoggerFactory>()?.CreateLogger(nameof(BrandSplash));

    private static void Show(Window window, string appName, string? lead, string? tagline, string? nameFont,
        Func<Page, bool> isFirstPage)
    {
        if (_shown) return;
        _shown = true;

        // Without a handler there is nothing to lay the splash on: the app opens without it
        if (window.Handler?.MauiContext is not { } context)
        {
            LetEverythingThrough();
            return;
        }

        try
        {
            var splash = new BrandSplashView(appName, lead, tagline, () => WhenReadyAsync(window, isFirstPage), nameFont);
            if (Attach(window, splash, context) is not { } detach)
            {
                LetEverythingThrough();
                return;
            }

            Current = splash;
            var gone = Volatile.Read(ref _gone) ?? new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            Volatile.Write(ref _gone, gone);
            var removed = false;

            void Remove()
            {
                if (removed) return;
                removed = true;
                Current = null;
                Volatile.Write(ref _gone, null);
                gone.TrySetResult();

                lock (Gate)
                {
                    Loadings.Clear();
                }

                try
                {
                    detach();
                    splash.Handler?.DisconnectHandler();
                }
                catch (Exception exception)
                {
                    Warn(exception, "The splash could not be taken off the window");
                }
            }

            splash.Closed += (_, _) => Remove();
            _ = WatchAsync(splash, Remove);
        }
        catch (Exception exception)
        {
            Current = null;
            LetEverythingThrough();
            Warn(exception, "The splash could not be laid over the window");
        }
    }

    // No splash after all: what waited for it goes on
    private static void LetEverythingThrough()
    {
        var gone = Interlocked.Exchange(ref _gone, null);
        gone?.TrySetResult();

        lock (Gate)
        {
            Loadings.Clear();
        }
    }

    /// <summary>
    /// Completes when the app is ready to be seen: its first page on display, loaded and laid out, and every loading
    /// it holds the splash for done; seen twice in a row, so that a page swapped in the meantime gets its layout.
    /// Within <see cref="ReadyPatience"/>.
    /// </summary>
    private static async Task WhenReadyAsync(Window window, Func<Page, bool> isFirstPage)
    {
        var start = System.Diagnostics.Stopwatch.GetTimestamp();
        var readyChecks = 0;

        while (readyChecks < 2)
        {
            if (System.Diagnostics.Stopwatch.GetElapsedTime(start) >= ReadyPatience)
            {
                Warn($"The app was not ready after {ReadyPatience}: the splash goes anyway");
                return;
            }

            bool loaded;
            lock (Gate)
            {
                loaded = Volatile.Read(ref _popupsWaiting) > 0 || Loadings.TrueForAll(static task => task.IsCompleted);
            }

            var ready = loaded && FirstPageOf(window) is { IsLoaded: true, Width: > 0, Height: > 0 } page && isFirstPage(page);
            readyChecks = ready ? readyChecks + 1 : 0;

            if (readyChecks < 2) await Task.Delay(ReadyStep);
        }
    }

    // The page the app shows in the window. A modal page an app pushes as it starts sits above it; the page under it
    // is the one that loads
    private static Page? FirstPageOf(Window window) => window.Page switch
    {
        Shell shell => shell.CurrentPage,
        NavigationPage navigation => navigation.CurrentPage,
        TabbedPage tabs => tabs.CurrentPage,
        FlyoutPage flyout => flyout.Detail is NavigationPage detail ? detail.CurrentPage : flyout.Detail,
        var page => page
    };

    private static async Task WatchAsync(BrandSplashView splash, Action remove)
    {
        await Task.Delay(StartPatience);
        MainThread.BeginInvokeOnMainThread(() => Safely(splash.Start, "The splash could not start"));

        await Task.Delay(RemovalPatience - StartPatience);
        MainThread.BeginInvokeOnMainThread(() => Safely(remove, "The splash could not be taken off the window"));
    }

    /// <summary>Runs what the splash does from a callback of the platform or of the main loop, where an exception
    /// would end the app (on iOS above all): the splash may fail, the app must start.</summary>
    internal static void Safely(Action action, string failure)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            Warn(exception, failure);
        }
    }

    /// <summary>Puts the splash's platform view above everything in the window; returns how to take it away, or null
    /// when the platform has no splash.</summary>
    private static partial Action? Attach(Window window, BrandSplashView splash, IMauiContext context);

#if !ANDROID && !IOS
    private static partial Action? Attach(Window window, BrandSplashView splash, IMauiContext context) => null;
#endif
}
