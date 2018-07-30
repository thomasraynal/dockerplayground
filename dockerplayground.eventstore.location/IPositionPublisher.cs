namespace DockerPlayground.Location.EventStore
{
    public interface IPositionPublisher
    {
        void EndStream();
        void StartStream();
    }
}