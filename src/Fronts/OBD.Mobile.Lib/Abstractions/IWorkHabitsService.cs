namespace OBD.Mobile.Lib.Abstractions;

public interface IWorkHabitsService
{
    Task<WorkHabits?> GetAsync();
    Task SaveAsync(WorkHabits workHabits);
}