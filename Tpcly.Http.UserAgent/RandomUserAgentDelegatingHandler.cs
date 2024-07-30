namespace Tpcly.Http.UserAgent;

public class RandomUserAgentDelegatingHandler(IUserAgentCollection userAgents, int rotationInterval, Random? random = null) : DelegatingHandler
{
    private readonly Random _random = random ?? new Random();
    private int _currentIndex;
    private string? _currentUserAgent;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_currentUserAgent == null || _currentIndex % rotationInterval == 0)
        {
            // Reset current index
            _currentIndex = 0;
            
            do
            {
                _currentUserAgent = userAgents.GetRandom(_random);
            } while (!request.Headers.UserAgent.TryParseAdd(_currentUserAgent));
        }

        _currentIndex++;

        return base.SendAsync(request, cancellationToken);
    }
}