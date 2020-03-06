namespace ECS.Data
{
    using UniRx;

    public class SystemData : IData
    {
        public int deltaTime;
        public long time;
        public int clientFrame;
        public int serverFrame;
        public ISubject<int> updateSubject;
    }
}