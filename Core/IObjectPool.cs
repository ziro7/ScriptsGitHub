namespace RPG.Core
{
    public interface IObjectPool<out T> 
    {
        T GetInstance();
    }
}