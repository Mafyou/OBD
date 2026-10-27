namespace OBD.Mobile.Pages.Notes;

public partial class NoteDetailPage : ContentPage
{
    private readonly NoteDetailViewModel _vm;

    private static Color AccentColor => AppColors.Get("AccentPrimary", Colors.White);
    private static Color TextColor => AppColors.Get("TextPrimary", Colors.White);

    public NoteDetailPage(NoteDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
        _vm.ContentRendered += RenderHashtags;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // A popup over the page makes it disappear, which ends the subscription: listen again once it is closed
        _vm.ContentRendered -= RenderHashtags;
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
            AppResources.GetString("NoteDetail_DeleteTitle"),
            AppResources.GetString("Common_IrreversibleAction"),
            confirmText: AppResources.GetString("Common_Delete"),
            isDanger: true);
        var result = await Dialogs.ConfirmAsync(popup);
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

        // A set, not a dictionary: two links on the same word (a double tap) made ToDictionary throw and closed the app
        var linkedWords = _vm.Links.Select(l => l.Word).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var parts = HashtagParser.Split(content);
        var fs = new FormattedString();

        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part)) continue;

            if (HashtagParser.TryGetWord(part, out var word))
            {
                var isLinked = linkedWords.Contains(word);
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
