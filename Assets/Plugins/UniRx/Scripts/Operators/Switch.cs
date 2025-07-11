﻿using System;

namespace UniRx.Operators
{
    internal class SwitchObservable<T> : OperatorObservableBase<T>
    {
        private readonly IObservable<IObservable<T>> sources;

        public SwitchObservable(IObservable<IObservable<T>> sources)
            : base(true)
        {
            this.sources = sources;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new SwitchObserver(this, observer, cancel).Run();
        }

        private class SwitchObserver : OperatorObserverBase<IObservable<T>, T>
        {
            private readonly SwitchObservable<T> parent;

            private readonly object gate = new object();
            private readonly SerialDisposable innerSubscription = new SerialDisposable();
            private bool isStopped = false;
            private ulong latest = 0UL;
            private bool hasLatest = false;

            public SwitchObserver(SwitchObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var subscription = parent.sources.Subscribe(this);
                return StableCompositeDisposable.Create(subscription, innerSubscription);
            }

            public override void OnNext(IObservable<T> value)
            {
                var id = default(ulong);
                lock (gate)
                {
                    id = unchecked(++latest);
                    hasLatest = true;
                }

                var d = new SingleAssignmentDisposable();
                innerSubscription.Disposable = d;
                d.Disposable = value.Subscribe(new Switch(this, id));
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { observer.OnError(error); }
                    finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    isStopped = true;
                    if (!hasLatest)
                    {
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }

            private class Switch : IObserver<T>
            {
                private readonly SwitchObserver parent;
                private readonly ulong id;

                public Switch(SwitchObserver observer, ulong id)
                {
                    this.parent = observer;
                    this.id = id;
                }

                public void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        if (parent.latest == id)
                        {
                            parent.observer.OnNext(value);
                        }
                    }
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        if (parent.latest == id)
                        {
                            parent.observer.OnError(error);
                        }
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        if (parent.latest == id)
                        {
                            parent.hasLatest = false;
                            if (parent.isStopped)
                            {
                                parent.observer.OnCompleted();
                            }
                        }
                    }
                }
            }
        }
    }
}
