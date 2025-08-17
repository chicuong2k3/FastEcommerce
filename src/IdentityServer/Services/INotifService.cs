namespace IdentityServer.Services;

public interface INotifService
{
    void Success(string message);
    void Error(string message);
}
