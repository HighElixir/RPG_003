﻿using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class RepeatSafeObservable<T> : OperatorObservableBase<T>
    {
        private readonly IEnumerable<IObservable<T>> sources;

        public RepeatSafeObservable(IEnumerable<IObservable<T>> sources, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.sources = sources;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new RepeatSafe(this, observer, cancel).Run();
        }

        private class RepeatSafe : OperatorObserverBase<T, T>
        {
            private readonly RepeatSafeObservable<T> parent;
            private readonly object gate = new object();

            private IEnumerator<IObservable<T>> e;
            private SerialDisposable subscription;
            private Action nextSelf;
            private bool isDisposed;
            private bool isRunNext;

            public RepeatSafe(RepeatSafeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                isDisposed = false;
                isRunNext = false;
                e = parent.sources.GetEnumerator();
                subscription = new SerialDisposable();

                var schedule = Scheduler.DefaultSchedulers.TailRecursion.Schedule(RecursiveRun);

                return StableCompositeDisposable.Create(schedule, subscription, Disposable.Create(() =>
                {
                    lock (gate)
                    {
                        isDisposed = true;
                        e.Dispose();
                    }
                }));
            }

            private void RecursiveRun(Action self)
            {
                lock (gate)
                {
                    this.nextSelf = self;
                    if (isDisposed) return;

                    var current = default(IObservable<T>);
                    var hasNext = false;
                    var ex = default(Exception);

                    try
                    {
                        hasNext = e.MoveNext();
                        if (hasNext)
                        {
                            current = e.Current;
                            if (current == null) throw new InvalidOperationException("sequence is null.");
                        }
                        else
                        {
                            e.Dispose();
                        }
                    }
                    catch (Exception exception)
                    {
                        ex = exception;
                        e.Dispose();
                    }

                    if (ex != null)
                    {
                        try { observer.OnError(ex); }
                        finally { Dispose(); }
                        return;
                    }

                    if (!hasNext)
                    {
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                        return;
                    }

                    var source = e.Current;
                    var d = new SingleAssignmentDisposable();
                    subscription.Disposable = d;
                    d.Disposable = source.Subscribe(this);
                }
            }

            public override void OnNext(T value)
            {
                isRunNext = true;
                base.observer.OnNext(value);
            }
            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (isRunNext && !isDisposed)
                {
                    isRunNext = false;
                    this.nextSelf();
                }
                else
                {
                    e.Dispose();
                    if (!isDisposed)
                    {
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }
        }
    }
}