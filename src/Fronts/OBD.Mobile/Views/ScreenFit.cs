namespace OBD.Mobile.Views;

/// <summary>
/// The room the app is given, rather than the size of the phone. On the outer screen of a foldable (a Motorola razr
/// closed: 480 x 410 units once its cameras and shortcuts are left out) the app has under half the height of the main
/// screen, which <see cref="DeviceDisplay.MainDisplayInfo"/> still reports: dialogs sized from it were cut off there,
/// "Later" out of reach. Dialogs size themselves from the window, and what only decorates a page (a large header, a
/// subtitle) gives way when the window is short (<see cref="TallOnlyProperty"/>).
/// </summary>
public static class ScreenFit
{
    /// <summary>
    /// The height of the window of the app, in device-independent units; the height of the main screen while there is
    /// no window yet, and zero when there is no screen to ask (a unit test).
    /// </summary>
    public static double WindowHeight()
    {
        try
        {
            if (Application.Current?.Windows is [{ Height: > 0 } window, ..])
            {
                return window.Height;
            }

            var display = DeviceDisplay.Current.MainDisplayInfo;
            return display.Density > 0 ? display.Height / display.Density : 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    /// <summary>The width of the window of the app, in device-independent units, found as <see cref="WindowHeight"/> is.</summary>
    public static double WindowWidth()
    {
        try
        {
            if (Application.Current?.Windows is [{ Width: > 0 } window, ..])
            {
                return window.Width;
            }

            var display = DeviceDisplay.Current.MainDisplayInfo;
            return display.Density > 0 ? display.Width / display.Density : 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    /// <summary>A window under <see cref="PopupFit.ShortScreen"/>: a phone lying on its side, the outer screen of a foldable.</summary>
    public static bool IsShort() => WindowHeight() is > 0 and < PopupFit.ShortScreen;

    /// <summary>
    /// Runs <paramref name="fit"/> now, then each time the window of the app changes size while <paramref name="owner"/>
    /// is on screen: a foldable opened or closed, a phone turned.
    /// </summary>
    public static void FollowWindow(VisualElement owner, Action fit)
    {
        Window? window = null;

        void OnWindowSizeChanged(object? sender, EventArgs e) => fit();

        owner.Loaded += (_, _) =>
        {
            window = Application.Current?.Windows.FirstOrDefault();
            if (window is not null)
            {
                window.SizeChanged += OnWindowSizeChanged;
            }

            fit();
        };
        owner.Unloaded += (_, _) =>
        {
            if (window is not null)
            {
                window.SizeChanged -= OnWindowSizeChanged;
            }

            window = null;
        };

        fit();
    }

    /// <summary>
    /// True on what only decorates a page: it is hidden while the window is short, and shown again when it is tall.
    /// Its own IsVisible is then driven from here, so an element bound to something else for it does not take it.
    /// </summary>
    public static readonly BindableProperty TallOnlyProperty = BindableProperty.CreateAttached(
        "TallOnly", typeof(bool), typeof(ScreenFit), false, propertyChanged: OnTallOnlyChanged);

    public static bool GetTallOnly(BindableObject view) => (bool)view.GetValue(TallOnlyProperty);

    public static void SetTallOnly(BindableObject view, bool value) => view.SetValue(TallOnlyProperty, value);

    private static void OnTallOnlyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is VisualElement view && newValue is true)
        {
            FollowWindow(view, () => view.IsVisible = !IsShort());
        }
    }

    /// <summary>
    /// The height an element takes while the window is short (a card, an image, a chart that would push the rest out
    /// of the screen); it gets its own height back when the window is tall. Settable from a style.
    /// </summary>
    public static readonly BindableProperty ShortHeightRequestProperty = BindableProperty.CreateAttached(
        "ShortHeightRequest", typeof(double), typeof(ScreenFit), -1d, propertyChanged: OnShortHeightRequestChanged);

    public static double GetShortHeightRequest(BindableObject view) => (double)view.GetValue(ShortHeightRequestProperty);

    public static void SetShortHeightRequest(BindableObject view, double value) => view.SetValue(ShortHeightRequestProperty, value);

    // The height the element had before a short window changed it: given back once the window is tall again
    private static readonly BindableProperty TallHeightRequestProperty = BindableProperty.CreateAttached(
        "TallHeightRequest", typeof(double), typeof(ScreenFit), -1d);

    private static void OnShortHeightRequestChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement view || newValue is not double shortHeight || shortHeight < 0)
        {
            return;
        }

        FollowWindow(view, () =>
        {
            if (IsShort())
            {
                if (!view.IsSet(TallHeightRequestProperty))
                {
                    view.SetValue(TallHeightRequestProperty, view.HeightRequest);
                }

                view.HeightRequest = shortHeight;
            }
            else if (view.IsSet(TallHeightRequestProperty))
            {
                view.HeightRequest = (double)view.GetValue(TallHeightRequestProperty);
                view.ClearValue(TallHeightRequestProperty);
            }
        });
    }
}