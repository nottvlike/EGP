namespace ECS.Data
{
    using System;

    public enum AnimationValueType
    {
        None,
        Bool,
        Float,
        Integer,
        Trigger,
        ResetTrigger,
    }

    [Serializable]
    public struct AnimationValueInfo
    {
        public AnimationValueType type;
        public string name;
        public string value;
    }

}