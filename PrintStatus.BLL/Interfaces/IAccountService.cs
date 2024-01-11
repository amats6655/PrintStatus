namespace PrintStatus.BLL;

public interface IAccountService
{
    Task<bool> ValidateUserCredentialsAsync(string username, string password);
}
