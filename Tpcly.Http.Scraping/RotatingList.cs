using Tpcly.Http.Scraping.Abstractions;

namespace Tpcly.Http.Scraping;

public class RotatingList<T>(IList<T> items) : IRotatingList<T>
{
    public Random Random { get; set; } = new();

    public int Interval { get; set; } = 1;

    public RotationMode RotationMode { get; set; } = RotationMode.Sequential;

    public int CurrentIndex { get; set; }

    private int _countSinceLastRotation;

    public T Next()
    {
        if (RotationMode == RotationMode.Random && _countSinceLastRotation == 0)
        {
            CurrentIndex = Random.Next(items.Count);
            _countSinceLastRotation = 1;
        }
        else
        {
            // Check if it's time to rotate
            if (_countSinceLastRotation >= Interval)
            {
                Rotate();
            }
            else
            {
                _countSinceLastRotation++;
            }
        }

        return items[CurrentIndex];
    }

    public void Rotate()
    {
        if (RotationMode == RotationMode.Random)
        {
            CurrentIndex = Random.Next(items.Count);
        }
        else // Sequential
        {
            CurrentIndex = (CurrentIndex + 1) % items.Count;
        }

        _countSinceLastRotation = 1; // Reset counter after rotation
    }
}