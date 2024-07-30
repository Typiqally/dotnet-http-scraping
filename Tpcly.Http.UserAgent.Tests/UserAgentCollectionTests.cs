using Moq;

namespace Tpcly.Http.UserAgent.Tests;

public class UserAgentCollectionTests
{
    private readonly List<string> _defaultUserAgents = ["ua_1", "ua_2"];

    [Test]
    public void Get_ValidIndex_ReturnsUserAgentAtIndex()
    {
        // Arrange
        const int validIndex = 1;
        var userAgentCollection = new UserAgentCollection(_defaultUserAgents);

        // Act
        var userAgent = userAgentCollection.Get(validIndex);

        // Assert
        Assert.That(userAgent, Is.EqualTo(_defaultUserAgents[validIndex]));
    }
    
    [Test]
    public void Get_InvalidIndex_ReturnsUserAgentAtIndex()
    {
        // Arrange
        var invalidIndex = _defaultUserAgents.Count + 1;
        var userAgentCollection = new UserAgentCollection(_defaultUserAgents);

        // Act
        var userAgent = userAgentCollection.Get(invalidIndex);

        // Assert
        Assert.That(userAgent, Is.EqualTo(null));
    }
    
    [Test]
    public void GetRandom_ReturnsRandomUserAgent()
    {
        // Arrange
        const int predeterminedIndex = 0;
        
        var randomMock = new Mock<Random>();
        randomMock
            .Setup(r => r.Next(It.IsAny<int>()))
            .Returns(predeterminedIndex);
        
        var userAgentCollection = new UserAgentCollection(_defaultUserAgents);

        // Act
        var userAgent = userAgentCollection.GetRandom(randomMock.Object);

        // Assert
        randomMock.Verify(r => r.Next(It.IsAny<int>()), Times.Exactly(1));
        Assert.That(userAgent, Is.EqualTo(_defaultUserAgents[predeterminedIndex]));
    }
}