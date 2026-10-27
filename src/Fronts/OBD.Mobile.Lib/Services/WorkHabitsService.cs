namespace OBD.Mobile.Lib.Services;

public class WorkHabitsService(DatabaseContext db) : IWorkHabitsService
{
    // The notebook keeps a single row of work habits
    public async Task<WorkHabits?> GetAsync()
    {
        var first = await db.QueryAsync<WorkHabits>("SELECT * FROM WorkHabits ORDER BY Id LIMIT 1");
        return first.Count > 0 ? first[0] : null;
    }

    public async Task SaveAsync(WorkHabits workHabits)
    {
        if (workHabits.Id is 0)
            await db.InsertAsync(workHabits);
        else
            await db.UpdateAsync(workHabits);
    }
}