namespace ECS.Helper
{
    using UnityEngine;
    using ECS.Module;
    using System;
    using UniRx;

    public class Despawner : MonoBehaviour
    {
        public float delay;
        IDisposable _disposable;

        void OnEnable()
        {
            Despawn();
        }

        void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        void Despawn()
        {
            if (delay <= 0)
            {
                return;
            }

            if (_disposable != null)
            {
                Dispose();
            }

            _disposable = Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe(_ =>
            {
                Dispose();

                if (gameObject.activeInHierarchy)
                {
                    gameObject.Despawn();
                }
            });
        }
    }
}