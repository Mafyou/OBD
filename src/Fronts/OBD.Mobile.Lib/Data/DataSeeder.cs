namespace OBD.Mobile.Lib.Data;

public class DataSeeder(DatabaseContext db)
{
    public async Task SeedAsync()
    {
        var existing = await db.GetAllAsync<Sector>();
        if (existing.Count > 0) return;

        var dev = new Sector { Name = "Développement" };
        var product = new Sector { Name = "Produit" };
        var design = new Sector { Name = "Design" };
        var management = new Sector { Name = "Management" };

        await db.InsertAsync(dev);
        await db.InsertAsync(product);
        await db.InsertAsync(design);
        await db.InsertAsync(management);

        var sophie = new Person
        {
            Name = "Sophie Martin",
            Position = "Engineering Manager",
            SectorId = management.Id,
            Memo = "Mon N+1. Réunion 1:1 tous les mardis à 10h."
        };
        await db.InsertAsync(sophie);
        await db.InsertAsync(new Person
        {
            Name = "Thomas Dubois",
            Position = "Lead Développeur",
            SectorId = dev.Id,
            Memo = "Référent technique. Préfère les échanges en async sur Slack."
        });
        await db.InsertAsync(new Person
        {
            Name = "Léa Bernard",
            Position = "Product Owner",
            SectorId = product.Id,
            Memo = "Valider les specs avec elle avant tout dev."
        });
        await db.InsertAsync(new Person
        {
            Name = "Marc Leroy",
            Position = "Designer UX",
            SectorId = design.Id,
            Memo = "Figma pour tous les designs. Demander accès au projet OBD."
        });
        await db.InsertAsync(new Person
        {
            Name = "Julie Petit",
            Position = "Développeuse Senior",
            SectorId = dev.Id,
            Memo = "Experte CI/CD et infra. À contacter pour les questions de déploiement."
        });

        await db.InsertAsync(new Note
        {
            SectorId = dev.Id,
            Type = TypeNote.Text,
            Content = "Stand-up daily à 9h30, durée max 15 min.\nFormat : hier / aujourd'hui / bloquants.",
            Keywords = "réunion,daily,9h30",
            CreatedAt = DateTime.UtcNow
        });
        await db.InsertAsync(new Note
        {
            SectorId = product.Id,
            Type = TypeNote.Text,
            Content = "Les specs sont rédigées sur Confluence. Toujours valider avec Léa avant de commencer un ticket.",
            Keywords = "specs,confluence,validation",
            CreatedAt = DateTime.UtcNow
        });
        await db.InsertAsync(new Note
        {
            SectorId = design.Id,
            Type = TypeNote.Text,
            Content = "Design system centralisé dans Figma. Demander accès à Marc dès la première semaine.",
            Keywords = "figma,design system,accès",
            CreatedAt = DateTime.UtcNow
        });
        await db.InsertAsync(new Note
        {
            SectorId = management.Id,
            Type = TypeNote.Text,
            Content = "Rétro d'équipe tous les vendredis à 16h. Préparer 1 point positif + 1 point d'amélioration.",
            Keywords = "rétro,vendredi,16h",
            CreatedAt = DateTime.UtcNow
        });

        await db.InsertAsync(new WorkHabits
        {
            RegularMeetings = "Daily 9h30 · Rétro vendredi 16h · 1:1 mardi 10h",
            RemoteWorkDays = "Lundi et vendredi",
            ManagerId = sophie.Id
        });
    }

    public async Task SeedIfEmptyAsync()
    {
        var existing = await db.GetAllAsync<Sector>();
        if (existing.Count > 0) return;

        var product = new Sector { Name = "Produit" };

        await db.InsertAsync(product);

        var john = new Person
        {
            Name = "John Doe",
            Position = "Product Owner",
            SectorId = product.Id,
            Memo = "Valider les specs avec lui avant tout dev."
        };
        await db.InsertAsync(john);
        await db.InsertAsync(new Note
        {
            SectorId = product.Id,
            Type = TypeNote.Text,
            Content = "Une fois les specs sont rédigées. Toujours valider avec John avant de commencer un ticket.",
            Keywords = "specs,validation",
            CreatedAt = DateTime.UtcNow
        });

        await db.InsertAsync(new WorkHabits
        {
            RegularMeetings = "Daily 9h30 · Rétro vendredi 16h · 1:1 mardi 10h",
            RemoteWorkDays = "Lundi et vendredi",
            ManagerId = john.Id
        });
    }
}
