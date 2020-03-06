namespace ECS.Data
{
    using System.Collections.Generic;
    using UniRx;

    public class GameStateData : IData
    {
        public ReactiveProperty<int> currentState;
    }
}