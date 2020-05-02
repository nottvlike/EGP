namespace ECS.Data
{
    using ECS.Common;
    using UniRx;

    public class AttackInfo : IPoolObject
    {
        public uint sourceId;
        public uint targetId;
        public int attack;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            sourceId = 0;
            targetId = 0;
            attack = 0;
        }
    }

    public class DamageInfo : IPoolObject
    {
        public uint sourceId;
        public uint targetId;

        public int attack;
        public int damage;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            sourceId = 0;
            targetId = 0;
            attack = 0;
            damage = 0;
        }
    }

    public class ObjectAttackServerData : IData, IPoolObject
    {
        public ISubject<AttackInfo> onAttack;
        public ISubject<DamageInfo> onBeforeDamage;
        public ISubject<DamageInfo> onDamage;
        public ISubject<DamageInfo> onAfterDamage;

        public bool IsInUse { get; set; }
        public void Clear()
        {
        }
    }
}
