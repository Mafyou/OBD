namespace OBD.Mobile.Lib.Services;

public class WorkHabitsService(DatabaseContext db) : IWorkHabitsService
{
    public async Task<WorkHabits?> GetAsync()
    {
        var all = await db.GetAllAsync<WorkHabits>();
        return all.FirstOrDefault();
    }

    public async Task SaveAsync(WorkHabits workHabits)
    {
        if (workHabits.Id is 0)
            await db.InsertAsync(workHabits);
        else
            await db.UpdateAsync(workHabits);
    }
}
