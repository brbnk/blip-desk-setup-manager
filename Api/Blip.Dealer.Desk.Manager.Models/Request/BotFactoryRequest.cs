namespace Blip.Dealer.Desk.Manager.Models.Request;

public abstract class BotFactoryRequest
{
    private string _bearerToken;

    public void SetBearerToken(string token) 
    {
        _bearerToken = $"{token}";
    }

    public string GetBearerToken() => _bearerToken;
}