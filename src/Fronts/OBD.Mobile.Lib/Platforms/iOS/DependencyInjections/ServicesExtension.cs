namespace OBD.Mobile.Lib.DependencyInjections;

public static partial class ServicesExtension
{
    extension(IServiceCollection services)
    {
        public void AddServices()
        {
            services.AddSingleton<IPersonService, PersonService>();
            services.AddSingleton<INoteService, NoteService>();
            services.AddSingleton<ISectorService, SectorService>();
            services.AddSingleton<INoteLinkService, NoteLinkService>();
            services.AddSingleton<IWorkHabitsService, WorkHabitsService>();
        }
    }
}