namespace OBD.Mobile.Lib.Resources.Strings;

/// <summary>
/// XAML markup extension for resx-based localization: {strings:Translate Key}.
/// Produces a one-way binding on <see cref="LocalizationResourceManager"/>'s indexer so the UI
/// updates live when the language changes.
/// </summary>
[ContentProperty(nameof(Key))]
public sealed class TranslateExtension : IMarkupExtension<BindingBase>
{
    public string Key { get; set; } = string.Empty;

    public BindingBase ProvideValue(IServiceProvider serviceProvider) =>
        new Binding($"[{Key}]", BindingMode.OneWay, source: LocalizationResourceManager.Instance);

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}
