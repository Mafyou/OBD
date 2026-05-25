namespace OBD.Mobile.Pages.Search;

public partial class SearchPage : ContentPage
{
    public SearchPage(SearchViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        MainSearchBar.Focus();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        MainSearchBar.Unfocus();
    }
}
