namespace OBD.Mobile.Pages.Notes;

public partial class NoteDetailPage : ContentPage
{
    private readonly NoteDetailViewModel _vm;

    private static Color AccentColor => (Color)Application.Current!.Resources["AccentPrimary"];
    private static Color TextColor => (Color)Application.Current!.Resources["TextPrimary"];

    public NoteDetailPage(NoteDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
        _vm.ContentRendered += RenderHashtags;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (_vm.IsViewMode)
            RenderHashtags();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.ContentRendered -= RenderHashtags;
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        var popup = new ConfirmPopup(
            "Supprimer cette note ?",
            "Cette action est irréversible.",
            confirmText: "Supprimer",
            isDanger: true);
        var result = await App.Current!.Windows[0].Page!.ShowPopupAsync<bool>(popup, new PopupOptions
        {
            CanBeDismissedByTappingOutsideOfPopup = true,
            Shadow = null,
            Shape = null,
        });
        if (result.WasDismissedByTappingOutsideOfPopup)
            return;
        if (result.Result is true && _vm.DeleteCommand is IAsyncRelayCommand asyncCmd)
            await asyncCmd.ExecuteAsync(null);
        else
            _vm.EditCommand.Execute(null);
    }

    private void RenderHashtags()
    {
        var content = _vm.Note?.Content ?? string.Empty;

        if (string.IsNullOrEmpty(content))
        {
            ContentLabel.FormattedText = null;
            return;
        }

        var linksMap = _vm.Links.ToDictionary(
            l => l.Word.ToLower(),
            StringComparer.OrdinalIgnoreCase);

        var parts = Regex.Split(content, @"(#\w+)");
        var fs = new FormattedString();

        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part)) continue;

            if (part.StartsWith('#') && part.Length > 1)
            {
                var word = part[1..];
                var isLinked = linksMap.ContainsKey(word);
                var span = new Span
                {
                    Text = part,
                    TextColor = AccentColor,
                    FontAttributes = isLinked ? FontAttributes.Bold : FontAttributes.None,
                    TextDecorations = isLinked ? TextDecorations.Underline : TextDecorations.None,
                };
                var tap = new TapGestureRecognizer();
                var capturedWord = word;
                tap.Tapped += (_, _) => _ = _vm.TapHashtagCommand.ExecuteAsync(capturedWord);
                span.GestureRecognizers.Add(tap);
                fs.Spans.Add(span);
            }
            else
            {
                fs.Spans.Add(new Span { Text = part, TextColor = TextColor });
            }
        }

        ContentLabel.FormattedText = fs;
    }
}
