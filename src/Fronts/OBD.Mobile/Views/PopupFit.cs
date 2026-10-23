namespace OBD.Mobile.Views;

/// <summary>
/// Keeps a dialog within the window. A dialog is as tall as its content: on a short window (a phone lying on its side,
/// the outer screen of a foldable) a long list or a long message pushed its own buttons out of reach. The part that
/// may grow scrolls in what the window leaves, and the buttons stay in view.
/// </summary>
public static class PopupFit
{
    /// <summary>
    /// What the system bars and the frame around a dialog take from the height of the window. Measured on a phone
    /// lying on its side: 143 units.
    /// </summary>
    public const double AroundPopup = 150;

    /// <summary>The least a scrolling part is given, however short the window: about one row of a list, which scrolls.</summary>
    public const double MinimumScrollable = 40;

    /// <summary>
    /// A window under this height is short: a phone lying on its side, the outer screen of a foldable (410 units on a
    /// Motorola razr closed).
    /// </summary>
    public const double ShortScreen = 600;

    /// <summary>
    /// The height the scrolling part of a dialog may take, in device-independent units: the one it prefers, or
    /// less on a window too short for it.
    /// </summary>
    /// <param name="screenHeight">The height of the window; zero or less when it is not known: the preferred height then.</param>
    /// <param name="fixedHeight">What the dialog shows around its scrolling part: title, buttons, paddings.</param>
    /// <param name="preferred">The height the scrolling part takes when the window allows it.</param>
    public static double ScrollableHeight(double screenHeight, double fixedHeight, double preferred) =>
        screenHeight > 0
            ? Math.Min(preferred, Math.Max(MinimumScrollable, screenHeight - AroundPopup - fixedHeight))
            : preferred;
}