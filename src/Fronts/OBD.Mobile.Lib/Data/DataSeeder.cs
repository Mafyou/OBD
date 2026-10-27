namespace OBD.Mobile.Lib.Data;

public class DataSeeder(DatabaseContext db)
{
    /// <summary>
    /// Fills an empty notebook with an example team. All of it is written in one transaction: an app closed half-way
    /// left a few sectors behind, and since the notebook was then no longer empty, the rest never came.
    /// </summary>
    public async Task SeedAsync()
    {
        var sectorCount = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Sector");
        if (sectorCount > 0) return;

        await db.RunInTransactionAsync(Seed);
    }

    private static void Seed(SQLiteConnection connection)
    {
        var dev = new Sector { Name = "Développement" };
        var product = new Sector { Name = "Produit" };
        var design = new Sector { Name = "Design" };
        var management = new Sector { Name = "Management" };
        var rh = new Sector { Name = "RH" };

        connection.Insert(dev);
        connection.Insert(product);
        connection.Insert(design);
        connection.Insert(management);
        connection.Insert(rh);

        var sophie = new Person
        {
            Name = "Sophie Martin",
            Position = "Engineering Manager",
            SectorId = management.Id,
            Memo = "Mon N+1. Réunion 1:1 tous les mardis à 10h."
        };
        connection.Insert(sophie);
        connection.Insert(new Person
        {
            Name = "Thomas Dubois",
            Position = "Lead Développeur",
            SectorId = dev.Id,
            Memo = "Référent technique. Préfère les échanges en async sur Slack."
        });
        connection.Insert(new Person
        {
            Name = "Léa Bernard",
            Position = "Product Owner",
            SectorId = product.Id,
            Memo = "Valider les specs avec elle avant tout dev."
        });
        connection.Insert(new Person
        {
            Name = "Alice Dupont",
            Position = "Responsable RH",
            SectorId = rh.Id,
            Memo = "Gérer les questions RH et les congés."
        });
        connection.Insert(new Person
        {
            Name = "Marc Leroy",
            Position = "Designer UX",
            SectorId = design.Id,
            Memo = "Figma pour tous les designs. Demander accès au projet OBD."
        });
        connection.Insert(new Person
        {
            Name = "Julie Petit",
            Position = "Développeuse Senior",
            SectorId = dev.Id,
            Memo = "Experte CI/CD et infra. À contacter pour les questions de déploiement."
        });

        connection.Insert(new Note
        {
            SectorId = dev.Id,
            Type = TypeNote.Text,
            Content = "Stand-up daily à 9h30, durée max 15 min.\nFormat : hier / aujourd'hui / bloquants.",
            Keywords = "réunion,daily,9h30",
            CreatedAt = DateTime.UtcNow
        });
        connection.Insert(new Note
        {
            SectorId = product.Id,
            Type = TypeNote.Text,
            Content = "Les specs sont rédigées sur Confluence. Toujours valider avec Léa avant de commencer un ticket.",
            Keywords = "specs,confluence,validation",
            CreatedAt = DateTime.UtcNow
        });
        connection.Insert(new Note
        {
            SectorId = design.Id,
            Type = TypeNote.Text,
            Content = "Design system centralisé dans Figma. Demander accès à Marc dès la première semaine.",
            Keywords = "figma,design system,accès",
            CreatedAt = DateTime.UtcNow
        });
        connection.Insert(new Note
        {
            SectorId = management.Id,
            Type = TypeNote.Text,
            Content = "Rétro d'équipe tous les vendredis à 16h. Préparer 1 point positif + 1 point d'amélioration.",
            Keywords = "rétro,vendredi,16h",
            CreatedAt = DateTime.UtcNow
        });

        connection.Insert(new WorkHabits
        {
            RegularMeetings = "Daily 9h30 · Rétro vendredi 16h · 1:1 mardi 10h",
            RemoteWorkDays = "Lundi et vendredi",
            ManagerId = sophie.Id
        });
    }
}