namespace ECS.Data
{
    using UniRx;

    public class SystemData : IData
    {
        public long time;
        public int clientFrame;
        public int serverFrame;
    }

    public class UpdateData : IData
    {
        public int deltaTime;
        public ISubject<int> updateSubject;
    }

    public class LateUpdateData : IData
    {
        public ISubject<int> updateSubject;
    }

    public class FixedUpdateData : IData
    {
        public int deltaTime;
        public ISubject<int> updateSubject;
    }
}