using Tpcly.Http.Scraping.Abstractions;

namespace Tpcly.Http.Scraping;

public class RotatingUserAgentDelegatingHandler : DelegatingHandler
{
    private readonly IRotatingList<string> _userAgents;

    public RotatingUserAgentDelegatingHandler(IRotatingList<string> userAgents)
    {
        _userAgents = userAgents;
    }

    public RotatingUserAgentDelegatingHandler(IRotatingList<string> userAgents, HttpMessageHandler innerHandler) : base(innerHandler)
    {
        _userAgents = userAgents;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        do
        {
        } while (!request.Headers.UserAgent.TryParseAdd(_userAgents.Next()));

        return base.SendAsync(request, cancellationToken);
    }
}