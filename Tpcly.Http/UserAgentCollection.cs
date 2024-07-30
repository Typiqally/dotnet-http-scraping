using Tpcly.Http.Abstractions;

namespace Tpcly.Http;

public class UserAgentCollection(IList<string> userAgents) : IUserAgentCollection
{
    public string? Get(int index)
    {
        return userAgents.ElementAtOrDefault(index);
    }

    public string GetRandom(Random? random = null)
    {
        random ??= new Random();
        return userAgents[random.Next(userAgents.Count)];
    }
}