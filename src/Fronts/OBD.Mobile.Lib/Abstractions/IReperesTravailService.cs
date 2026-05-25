namespace OBD.Mobile.Lib.Services;

public interface IReperesTravailService
{
    Task<ReperesTravail?> GetAsync();
    Task SaveAsync(ReperesTravail reperes);
}