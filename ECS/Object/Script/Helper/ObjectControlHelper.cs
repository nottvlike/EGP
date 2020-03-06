namespace ECS.Object.Helper
{
    using ECS.Object.Data;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UniRx;
    using System;

    public class ObjectControlHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        ISubject<PointerEventData> pointerDownSubject = new Subject<PointerEventData>();
        ISubject<PointerEventData> pointerUpSubject = new Subject<PointerEventData>();

        void OnDestroy()
        {
            pointerDownSubject.OnCompleted();
            pointerUpSubject.OnCompleted();
        }

        public IObservable<PointerEventData> ObservePointerDown()
        {
            return pointerDownSubject;
        }

        public IObservable<PointerEventData> ObservePointerUp()
        {
            return pointerUpSubject;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerDownSubject.OnNext(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerUpSubject.OnNext(eventData);
        }
    }
}