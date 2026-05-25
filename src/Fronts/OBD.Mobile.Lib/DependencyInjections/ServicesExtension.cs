namespace OBD.Mobile.Lib.DependencyInjections;

public static class ServicesExtension
{
    extension(IServiceCollection services)
    {
        public void AddServices()
        {
            services.AddSingleton<IPersonService, PersonService>();
            services.AddSingleton<INoteService, NoteService>();
            services.AddSingleton<ISectorService, SectorService>();
            services.AddSingleton<INoteLinkService, NoteLinkService>();
            services.AddSingleton<IReperesTravailService, ReperesTravailService>();
        }
    }
}