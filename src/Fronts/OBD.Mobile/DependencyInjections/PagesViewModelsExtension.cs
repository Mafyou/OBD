namespace OBD.Mobile.DependencyInjections;

public static class PagesViewModelsExtension
{
    extension(IServiceCollection services)
    {
        public void AddPagesViewModels()
        {
            services.AddSingleton<AppShell>();
            services.AddSingletonWithShellRoute<OnboardingPage, OnboardingViewModel>(nameof(OnboardingPage));
            services.AddSingletonWithShellRoute<PersonsPage, PersonsViewModel>(nameof(PersonsPage));
            services.AddSingletonWithShellRoute<PersonDetailsPage, PersonDetailsViewModel>(nameof(PersonDetailsPage));
            services.AddSingletonWithShellRoute<SectorDetailsPage, SectorDetailsViewModel>(nameof(SectorDetailsPage));
            services.AddSingletonWithShellRoute<NotesPage, NotesViewModel>(nameof(NotesPage));
            services.AddSingletonWithShellRoute<NotesSectorPage, NotesSectorViewModel>(nameof(NotesSectorPage));
            services.AddTransientWithShellRoute<NoteDetailPage, NoteDetailViewModel>(nameof(NoteDetailPage));
            services.AddTransientWithShellRoute<SketchDetailsPage, SketchDetailsViewModel>(nameof(SketchDetailsPage));
            services.AddSingletonWithShellRoute<SearchPage, SearchViewModel>(nameof(SearchPage));
            services.AddSingletonWithShellRoute<ParametersPage, ParametersViewModel>(nameof(ParametersPage));
            services.AddSingletonWithShellRoute<SensitiveNotesPage, SensitiveNotesViewModel>(nameof(SensitiveNotesPage));
        }
    }
}