namespace OBD.Mobile.Lib.Services;

public class ReperesTravailService(DatabaseContext db) : IReperesTravailService
{
    public async Task<ReperesTravail?> GetAsync()
    {
        var all = await db.GetAllAsync<ReperesTravail>();
        return all.FirstOrDefault();
    }

    public async Task SaveAsync(ReperesTravail reperes)
    {
        if (reperes.Id is 0)
            await db.InsertAsync(reperes);
        else
            await db.UpdateAsync(reperes);
    }
}