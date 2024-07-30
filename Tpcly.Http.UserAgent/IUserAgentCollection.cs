namespace Tpcly.Http.UserAgent;

public interface IUserAgentCollection
{
    public string? Get(int index);
    
    public string GetRandom(Random? random = null);
}