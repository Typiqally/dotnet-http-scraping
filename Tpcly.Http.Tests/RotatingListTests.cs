using Moq;

namespace Tpcly.Http.Tests;

public class RotatingListTests
{
    [Test]
    public void Next_Sequential_ReturnsNextInSequence()
    {
        // Arrange
        var items = new[] { 1, 2 };
        var rotatingList = new RotatingList<int>(items);

        // Act
        var itemOne = rotatingList.Next();
        var itemTwo = rotatingList.Next();

        // Assert
        // randomMock.Verify(r => r.Next(It.IsAny<int>()), Times.Exactly(1));
        Assert.That(itemOne, Is.EqualTo(items[0]));
        Assert.That(itemTwo, Is.EqualTo(items[1]));
    }

    [TestCase(5, 15, new[] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1 })]
    [TestCase(5, 10, new[] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2 })]
    [TestCase(2, 10, new[] { 1, 1, 2, 2, 1, 1, 2, 2, 1, 1 })]
    [TestCase(2, 5, new[] { 1, 1, 2, 2, 1 })]
    public void Next_WithInterval_Sequential_ReturnsNextInSequence(int interval, int runTimes, int[] expectedSequence)
    {
        // Arrange
        var items = new[] { 1, 2 };
        var rotatingList = new RotatingList<int>(items)
        {
            Interval = interval
        };

        // Act
        var sequence = Enumerable.Range(0, runTimes).Select(i => rotatingList.Next()).ToList();

        // Assert
        CollectionAssert.AreEqual(expectedSequence, sequence);
    }

    [TestCase(5, 15, 3)]
    [TestCase(5, 10, 2)]
    [TestCase(2, 10, 5)]
    [TestCase(2, 5, 3)]
    public void Next_WithInterval_Random_ReturnsNextInSequence(int interval, int runTimes, int expectedRandomCalls)
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var randomMock = new Mock<Random>();
        randomMock
            .Setup(r => r.Next(It.IsAny<int>()))
            .Returns(0);

        var rotatingList = new RotatingList<int>(items)
        {
            Interval = interval,
            RotationMode = RotationMode.Random,
            Random = randomMock.Object
        };

        // Act
        var sequence = Enumerable.Range(0, runTimes).Select(i => rotatingList.Next()).ToList();

        // Assert
        randomMock.Verify(c => c.Next(It.IsAny<int>()), Times.Exactly(expectedRandomCalls));
    }
}