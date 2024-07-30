using System.Net;
using System.Net.Http.Headers;
using Moq;
using Moq.Protected;
using Tpcly.Http.Scraping.Abstractions;

namespace Tpcly.Http.Scraping.Tests;

public class RotatingUserAgentDelegatingHandlerTests
{
    private HttpRequestMessage _requestMock;
    private Mock<IRotatingList<string>> _rotatingCollectionMock;
    private Mock<DelegatingHandler> _innerHandlerMock;

    [SetUp]
    public void Setup()
    {
        _requestMock = new HttpRequestMessage();

        _rotatingCollectionMock = new Mock<IRotatingList<string>>(MockBehavior.Strict);
        _rotatingCollectionMock
            .Setup(a => a.Next())
            .Returns("ua_1");

        _innerHandlerMock = new Mock<DelegatingHandler>(MockBehavior.Strict);
        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", _requestMock, It.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = _requestMock });
    }

    [Test]
    public async Task SendAsync_RequestMessage_AddsUserAgent()
    {
        // Arrange
        var handler = new RotatingUserAgentDelegatingHandler(_rotatingCollectionMock.Object)
        {
            InnerHandler = _innerHandlerMock.Object
        };
        var invoker = new HttpMessageInvoker(handler);

        // Act
        var response = await invoker.SendAsync(_requestMock, default);

        // Assert
        _rotatingCollectionMock.Verify(c => c.Next(), Times.Exactly(1));
        Assert.That(response.RequestMessage?.Headers.UserAgent.ToArray(), Does.Contain(ProductInfoHeaderValue.Parse("ua_1")));
    }
}