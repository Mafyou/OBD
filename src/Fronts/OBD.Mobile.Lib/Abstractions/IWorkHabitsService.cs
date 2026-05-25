namespace OBD.Mobile.Lib.Services;

public interface IWorkHabitsService
{
    Task<WorkHabits?> GetAsync();
    Task SaveAsync(WorkHabits workHabits);
}
