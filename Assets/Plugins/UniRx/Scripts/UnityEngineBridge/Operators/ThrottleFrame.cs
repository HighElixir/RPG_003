﻿using System;

#if UniRxLibrary
using UnityObservable = UniRx.ObservableUnity;
#else
using UnityObservable = UniRx.Observable;
#endif

namespace UniRx.Operators
{
    internal class ThrottleFrameObservable<T> : OperatorObservableBase<T>
    {
        private readonly IObservable<T> source;
        private readonly int frameCount;
        private readonly FrameCountType frameCountType;

        public ThrottleFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ThrottleFrame(this, observer, cancel).Run();
        }

        private class ThrottleFrame : OperatorObserverBase<T, T>
        {
            private readonly ThrottleFrameObservable<T> parent;
            private readonly object gate = new object();
            private T latestValue = default(T);
            private bool hasValue = false;
            private SerialDisposable cancelable;
            private ulong id = 0;

            public ThrottleFrame(ThrottleFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                cancelable = new SerialDisposable();
                var subscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(cancelable, subscription);
            }

            public override void OnNext(T value)
            {
                ulong currentid;
                lock (gate)
                {
                    hasValue = true;
                    latestValue = value;
                    id = unchecked(id + 1);
                    currentid = id;
                }

                var d = new SingleAssignmentDisposable();
                cancelable.Disposable = d;
                d.Disposable = UnityObservable.TimerFrame(parent.frameCount, parent.frameCountType)
                    .Subscribe(new ThrottleFrameTick(this, currentid));
            }

            public override void OnError(Exception error)
            {
                cancelable.Dispose();

                lock (gate)
                {
                    hasValue = false;
                    id = unchecked(id + 1);
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                cancelable.Dispose();

                lock (gate)
                {
                    if (hasValue)
                    {
                        observer.OnNext(latestValue);
                    }
                    hasValue = false;
                    id = unchecked(id + 1);
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }

            private class ThrottleFrameTick : IObserver<long>
            {
                private readonly ThrottleFrame parent;
                private readonly ulong currentid;

                public ThrottleFrameTick(ThrottleFrame parent, ulong currentid)
                {
                    this.parent = parent;
                    this.currentid = currentid;
                }

                public void OnCompleted()
                {
                }

                public void OnError(Exception error)
                {
                }

                public void OnNext(long _)
                {
                    lock (parent.gate)
                    {
                        if (parent.hasValue && parent.id == currentid)
                        {
                            parent.observer.OnNext(parent.latestValue);
                        }
                        parent.hasValue = false;
                    }
                }
            }
        }
    }
}