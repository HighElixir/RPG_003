﻿using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class MergeObservable<T> : OperatorObservableBase<T>
    {
        private readonly IObservable<IObservable<T>> sources;
        private readonly int maxConcurrent;

        public MergeObservable(IObservable<IObservable<T>> sources, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.sources = sources;
        }

        public MergeObservable(IObservable<IObservable<T>> sources, int maxConcurrent, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.sources = sources;
            this.maxConcurrent = maxConcurrent;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (maxConcurrent > 0)
            {
                return new MergeConcurrentObserver(this, observer, cancel).Run();
            }
            else
            {
                return new MergeOuterObserver(this, observer, cancel).Run();
            }
        }

        private class MergeOuterObserver : OperatorObserverBase<IObservable<T>, T>
        {
            private readonly MergeObservable<T> parent;

            private CompositeDisposable collectionDisposable;
            private SingleAssignmentDisposable sourceDisposable;
            private object gate = new object();
            private bool isStopped = false;

            public MergeOuterObserver(MergeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                collectionDisposable = new CompositeDisposable();
                sourceDisposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(sourceDisposable);

                sourceDisposable.Disposable = parent.sources.Subscribe(this);
                return collectionDisposable;
            }

            public override void OnNext(IObservable<T> value)
            {
                var disposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(disposable);
                var collectionObserver = new Merge(this, disposable);
                disposable.Disposable = value.Subscribe(collectionObserver);
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); }
                    ;
                }
            }

            public override void OnCompleted()
            {
                isStopped = true;
                if (collectionDisposable.Count == 1)
                {
                    lock (gate)
                    {
                        try { observer.OnCompleted(); } finally { Dispose(); }
                        ;
                    }
                }
                else
                {
                    sourceDisposable.Dispose();
                }
            }

            private class Merge : OperatorObserverBase<T, T>
            {
                private readonly MergeOuterObserver parent;
                private readonly IDisposable cancel;

                public Merge(MergeOuterObserver parent, IDisposable cancel)
                    : base(parent.observer, cancel)
                {
                    this.parent = parent;
                    this.cancel = cancel;
                }

                public override void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        base.observer.OnNext(value);
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        try { observer.OnError(error); } finally { Dispose(); }
                        ;
                    }
                }

                public override void OnCompleted()
                {
                    parent.collectionDisposable.Remove(cancel);
                    if (parent.isStopped && parent.collectionDisposable.Count == 1)
                    {
                        lock (parent.gate)
                        {
                            try { observer.OnCompleted(); } finally { Dispose(); }
                            ;
                        }
                    }
                }
            }
        }

        private class MergeConcurrentObserver : OperatorObserverBase<IObservable<T>, T>
        {
            private readonly MergeObservable<T> parent;

            private CompositeDisposable collectionDisposable;
            private SingleAssignmentDisposable sourceDisposable;
            private object gate = new object();
            private bool isStopped = false;

            // concurrency
            private Queue<IObservable<T>> q;
            private int activeCount;

            public MergeConcurrentObserver(MergeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                q = new Queue<IObservable<T>>();
                activeCount = 0;

                collectionDisposable = new CompositeDisposable();
                sourceDisposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(sourceDisposable);

                sourceDisposable.Disposable = parent.sources.Subscribe(this);
                return collectionDisposable;
            }

            public override void OnNext(IObservable<T> value)
            {
                lock (gate)
                {
                    if (activeCount < parent.maxConcurrent)
                    {
                        activeCount++;
                        Subscribe(value);
                    }
                    else
                    {
                        q.Enqueue(value);
                    }
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); }
                    ;
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    isStopped = true;
                    if (activeCount == 0)
                    {
                        try { observer.OnCompleted(); } finally { Dispose(); }
                        ;
                    }
                    else
                    {
                        sourceDisposable.Dispose();
                    }
                }
            }

            private void Subscribe(IObservable<T> innerSource)
            {
                var disposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(disposable);
                var collectionObserver = new Merge(this, disposable);
                disposable.Disposable = innerSource.Subscribe(collectionObserver);
            }

            private class Merge : OperatorObserverBase<T, T>
            {
                private readonly MergeConcurrentObserver parent;
                private readonly IDisposable cancel;

                public Merge(MergeConcurrentObserver parent, IDisposable cancel)
                    : base(parent.observer, cancel)
                {
                    this.parent = parent;
                    this.cancel = cancel;
                }

                public override void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        base.observer.OnNext(value);
                    }
                }

                public override void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        try { observer.OnError(error); } finally { Dispose(); }
                        ;
                    }
                }

                public override void OnCompleted()
                {
                    parent.collectionDisposable.Remove(cancel);
                    lock (parent.gate)
                    {
                        if (parent.q.Count > 0)
                        {
                            var source = parent.q.Dequeue();
                            parent.Subscribe(source);
                        }
                        else
                        {
                            parent.activeCount--;
                            if (parent.isStopped && parent.activeCount == 0)
                            {
                                try { observer.OnCompleted(); } finally { Dispose(); }
                                ;
                            }
                        }
                    }
                }
            }
        }
    }
}