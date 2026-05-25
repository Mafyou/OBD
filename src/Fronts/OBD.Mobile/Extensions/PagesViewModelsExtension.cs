namespace OBD.Mobile.Extensions;

public static class PagesViewModelsExtension
{
    extension(IServiceCollection services)
    {
        public void AddPagesViewModels()
        {
            services.AddSingletonWithShellRoute<PersonsPage, PersonsViewModel>(nameof(PersonsPage));
            services.AddSingletonWithShellRoute<PersonDetailsPage, PersonDetailsViewModel>(nameof(PersonDetailsPage));
            services.AddSingletonWithShellRoute<SectorDetailsPage, SectorDetailsViewModel>(nameof(SectorDetailsPage));
            services.AddSingletonWithShellRoute<NotesPage, NotesViewModel>(nameof(NotesPage));
            services.AddSingletonWithShellRoute<NotesSectorPage, NotesSectorViewModel>(nameof(NotesSectorPage));
            services.AddSingletonWithShellRoute<NoteDetailPage, NoteDetailViewModel>(nameof(NoteDetailPage));
            services.AddSingletonWithShellRoute<SketchDetailsPage, SketchDetailsViewModel>(nameof(SketchDetailsPage));
            services.AddSingletonWithShellRoute<SearchPage, SearchViewModel>(nameof(SearchPage));
            services.AddSingletonWithShellRoute<ParametersPage, ParametersViewModel>(nameof(ParametersPage));
        }
    }
}