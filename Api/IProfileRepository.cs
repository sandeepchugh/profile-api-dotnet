namespace Api;

public interface IProfileRepository
{
    Task<Profile> GetProfile(string customerId);
}