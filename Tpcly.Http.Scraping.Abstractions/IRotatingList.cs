namespace Tpcly.Http.Scraping.Abstractions;

public interface IRotatingList<out T>
{
    public T Next();

    public void Rotate();
}