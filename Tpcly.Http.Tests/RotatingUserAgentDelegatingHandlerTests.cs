using System.Net;
using System.Net.Http.Headers;
using Moq;
using Moq.Protected;
using Tpcly.Http.Abstractions;

namespace Tpcly.Http.Tests;

public class RotatingUserAgentDelegatingHandlerTests
{
    private HttpRequestMessage _requestMock;
    private Mock<IUserAgentCollection> _userAgentCollectionMock;
    private Mock<DelegatingHandler> _innerHandlerMock;

    [SetUp]
    public void Setup()
    {
        _requestMock = new HttpRequestMessage();

        _userAgentCollectionMock = new Mock<IUserAgentCollection>(MockBehavior.Strict);
        _userAgentCollectionMock
            .Setup(a => a.GetRandom(It.IsAny<Random>()))
            .Returns("ua_1");

        _innerHandlerMock = new Mock<DelegatingHandler>(MockBehavior.Strict);
        _innerHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", _requestMock, It.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = _requestMock });
    }

    [Test]
    public async Task SendAsync_RequestMessage_AddsRandomUserAgent()
    {
        // Arrange
        var handler = new RotatingUserAgentDelegatingHandler(_userAgentCollectionMock.Object, 10)
        {
            InnerHandler = _innerHandlerMock.Object
        };
        var invoker = new HttpMessageInvoker(handler);

        // Act
        var response = await invoker.SendAsync(_requestMock, default);

        // Assert
        _userAgentCollectionMock.Verify(c => c.GetRandom(It.IsAny<Random>()), Times.Exactly(1));
        Assert.That(response.RequestMessage?.Headers.UserAgent.ToArray(), Does.Contain(ProductInfoHeaderValue.Parse("ua_1")));
    }

    [TestCase(5, 15, 3)]
    [TestCase(5, 10, 2)]
    [TestCase(2, 10, 5)]
    [TestCase(2, 5, 3)]
    public async Task SendAsync_RequestMessage_WithRotation_AddsRandomUserAgent_EveryRotation(int rotationInterval, int runTimes, int expectedRandomCalls)
    {
        // Arrange
        var handler = new RotatingUserAgentDelegatingHandler(_userAgentCollectionMock.Object, rotationInterval)
        {
            InnerHandler = _innerHandlerMock.Object
        };
        var invoker = new HttpMessageInvoker(handler);

        // Act
        foreach (var i in Enumerable.Range(0, runTimes))
        {
            await invoker.SendAsync(_requestMock, default);
        }

        // Assert
        _userAgentCollectionMock.Verify(c => c.GetRandom(It.IsAny<Random>()), Times.Exactly(expectedRandomCalls));
    }
}