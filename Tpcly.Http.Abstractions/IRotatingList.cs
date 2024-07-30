namespace Tpcly.Http.Abstractions;

public interface IRotatingList<out T>
{
    public T Next();

    public void Rotate();
}