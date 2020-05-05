namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Common;
    using System;
    using UniRx;

    public class ObjectAttackServer : SingleModule
    {
        public override int Group { get; protected set; }
            = WorldManager.Instance.Module.TagToModuleGroupType(Constant.SYSTEM_MODULE_GROUP_NAME);

        public ObjectAttackServer()
        {
            RequiredDataList = new Type[]{
                typeof(ObjectAttackServerData)
            };
        }

        static ObjectAttackServerData _attackServerData;

        protected override void OnAdd(GUnit unit)
        {
            _attackServerData = unit.GetData<ObjectAttackServerData>();
            _attackServerData.onAttack = new Subject<AttackInfo>();
            _attackServerData.onBeforeDamage = new Subject<DamageInfo>();
            _attackServerData.onDamage = new Subject<DamageInfo>();
            _attackServerData.onAfterDamage = new Subject<DamageInfo>();
        }

        protected override void OnRemove(GUnit unit)
        {
            _attackServerData.onAttack.OnCompleted();
            _attackServerData.onBeforeDamage.OnCompleted();
            _attackServerData.onDamage.OnCompleted();
            _attackServerData.onAfterDamage.OnCompleted();
            _attackServerData = null;
        }

        public static IObservable<AttackInfo> ObserverAttack()
        {
            return Observable.Defer(() =>
            {
                return _attackServerData.onAttack;
            });
        }

        public static IObservable<DamageInfo> ObserverBeforeDamage()
        {
            return Observable.Defer(() =>
            {
                return _attackServerData.onBeforeDamage;
            });
        }

        public static IObservable<DamageInfo> ObserverDamage()
        {
            return Observable.Defer(() =>
            {
                return _attackServerData.onDamage;
            });
        }

        public static IObservable<DamageInfo> ObserverAfterDamage()
        {
            return Observable.Defer(() =>
            {
                return _attackServerData.onAfterDamage;
            });
        }

        public static DamageInfo Attack(GUnit source, AttackInfo attackInfo)
        {
            _attackServerData.onAttack.OnNext(attackInfo);

            var damageInfo = Pool.Get<DamageInfo>();
            damageInfo.sourceId = source.UnitId;
            damageInfo.targetId = attackInfo.targetId;
            damageInfo.attack = attackInfo.attack;
            damageInfo.damage = attackInfo.attack;

            return damageInfo;
        }

        public static void Damage(DamageInfo damageInfo)
        {
#if DEBUG
            if (damageInfo == null)
            {
                Log.I("Damage failed, damage should not be null!");
                return;
            }
#endif

            _attackServerData.onBeforeDamage.OnNext(damageInfo);
            _attackServerData.onDamage.OnNext(damageInfo);
            _attackServerData.onAfterDamage.OnNext(damageInfo);
        }
    }
}
