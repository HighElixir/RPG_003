﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Operators
{
    internal class RepeatUntilObservable<T> : OperatorObservableBase<T>
    {
        private readonly IEnumerable<IObservable<T>> sources;
        private readonly IObservable<Unit> trigger;
        private readonly GameObject lifeTimeChecker;

        public RepeatUntilObservable(IEnumerable<IObservable<T>> sources, IObservable<Unit> trigger, GameObject lifeTimeChecker)
            : base(true)
        {
            this.sources = sources;
            this.trigger = trigger;
            this.lifeTimeChecker = lifeTimeChecker;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new RepeatUntil(this, observer, cancel).Run();
        }

        private class RepeatUntil : OperatorObserverBase<T, T>
        {
            private readonly RepeatUntilObservable<T> parent;
            private readonly object gate = new object();

            private IEnumerator<IObservable<T>> e;
            private SerialDisposable subscription;
            private SingleAssignmentDisposable schedule;
            private Action nextSelf;
            private bool isStopped;
            private bool isDisposed;
            private bool isFirstSubscribe;
            private IDisposable stopper;

            public RepeatUntil(RepeatUntilObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                isFirstSubscribe = true;
                isDisposed = false;
                isStopped = false;
                e = parent.sources.GetEnumerator();
                subscription = new SerialDisposable();
                schedule = new SingleAssignmentDisposable();

                stopper = parent.trigger.Subscribe(_ =>
                {
                    lock (gate)
                    {
                        isStopped = true;
                        e.Dispose();
                        subscription.Dispose();
                        schedule.Dispose();
                        observer.OnCompleted();
                    }
                }, observer.OnError);

                schedule.Disposable = Scheduler.CurrentThread.Schedule(RecursiveRun);

                return new CompositeDisposable(schedule, subscription, stopper, Disposable.Create(() =>
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
                    if (isStopped) return;

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
                        stopper.Dispose();
                        observer.OnError(ex);
                        return;
                    }

                    if (!hasNext)
                    {
                        stopper.Dispose();
                        observer.OnCompleted();
                        return;
                    }

                    var source = e.Current;
                    var d = new SingleAssignmentDisposable();
                    subscription.Disposable = d;

                    if (isFirstSubscribe)
                    {
                        isFirstSubscribe = false;
                        d.Disposable = source.Subscribe(this);
                    }
                    else
                    {
                        MainThreadDispatcher.SendStartCoroutine(SubscribeAfterEndOfFrame(d, source, this, parent.lifeTimeChecker));
                    }
                }
            }

            private static IEnumerator SubscribeAfterEndOfFrame(SingleAssignmentDisposable d, IObservable<T> source, IObserver<T> observer, GameObject lifeTimeChecker)
            {
                yield return YieldInstructionCache.WaitForEndOfFrame;
                if (!d.IsDisposed && lifeTimeChecker != null)
                {
                    d.Disposable = source.Subscribe(observer);
                }
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (!isDisposed)
                {
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