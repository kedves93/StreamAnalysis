namespace Apixu
{
    public interface IStartable
    {
        bool IsStarted { get; }

        void Start();
    }
}