﻿using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class BatchFrameObservable<T> : OperatorObservableBase<IList<T>>
    {
        private readonly IObservable<T> source;
        private readonly int frameCount;
        private readonly FrameCountType frameCountType;

        public BatchFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<IList<T>> observer, IDisposable cancel)
        {
            return new BatchFrame(this, observer, cancel).Run();
        }

        private class BatchFrame : OperatorObserverBase<T, IList<T>>
        {
            private readonly BatchFrameObservable<T> parent;
            private readonly object gate = new object();
            private readonly BooleanDisposable cancellationToken = new BooleanDisposable();
            private readonly System.Collections.IEnumerator timer;
            private bool isRunning;
            private bool isCompleted;
            private List<T> list;

            public BatchFrame(BatchFrameObservable<T> parent, IObserver<IList<T>> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.timer = new ReusableEnumerator(this);
            }

            public IDisposable Run()
            {
                list = new List<T>();
                var sourceSubscription = parent.source.Subscribe(this);
                return StableCompositeDisposable.Create(sourceSubscription, cancellationToken);
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    if (isCompleted) return;
                    list.Add(value);
                    if (!isRunning)
                    {
                        isRunning = true;
                        timer.Reset(); // reuse

                        switch (parent.frameCountType)
                        {
                            case FrameCountType.Update:
                                MainThreadDispatcher.StartUpdateMicroCoroutine(timer);
                                break;
                            case FrameCountType.FixedUpdate:
                                MainThreadDispatcher.StartFixedUpdateMicroCoroutine(timer);
                                break;
                            case FrameCountType.EndOfFrame:
                                MainThreadDispatcher.StartEndOfFrameMicroCoroutine(timer);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                List<T> currentList;
                lock (gate)
                {
                    isCompleted = true;
                    currentList = list;
                }
                if (currentList.Count != 0)
                {
                    observer.OnNext(currentList);
                }
                try { observer.OnCompleted(); } finally { Dispose(); }
            }

            // reuse, no gc allocate
            private class ReusableEnumerator : System.Collections.IEnumerator
            {
                private readonly BatchFrame parent;
                private int currentFrame;

                public ReusableEnumerator(BatchFrame parent)
                {
                    this.parent = parent;
                }

                public object Current
                {
                    get { return null; }
                }

                public bool MoveNext()
                {
                    if (parent.cancellationToken.IsDisposed) return false;

                    List<T> currentList;
                    lock (parent.gate)
                    {
                        if (currentFrame++ == parent.parent.frameCount)
                        {
                            if (parent.isCompleted) return false;

                            currentList = parent.list;
                            parent.list = new List<T>();
                            parent.isRunning = false;

                            // exit lock 
                        }
                        else
                        {
                            return true;
                        }
                    }

                    parent.observer.OnNext(currentList);
                    return false;
                }

                public void Reset()
                {
                    currentFrame = 0;
                }
            }
        }
    }

    internal class BatchFrameObservable : OperatorObservableBase<Unit>
    {
        private readonly IObservable<Unit> source;
        private readonly int frameCount;
        private readonly FrameCountType frameCountType;

        public BatchFrameObservable(IObservable<Unit> source, int frameCount, FrameCountType frameCountType)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
        {
            return new BatchFrame(this, observer, cancel).Run();
        }

        private class BatchFrame : OperatorObserverBase<Unit, Unit>
        {
            private readonly BatchFrameObservable parent;
            private readonly object gate = new object();
            private readonly BooleanDisposable cancellationToken = new BooleanDisposable();
            private readonly System.Collections.IEnumerator timer;

            private bool isRunning;
            private bool isCompleted;

            public BatchFrame(BatchFrameObservable parent, IObserver<Unit> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.timer = new ReusableEnumerator(this);
            }

            public IDisposable Run()
            {
                var sourceSubscription = parent.source.Subscribe(this);
                return StableCompositeDisposable.Create(sourceSubscription, cancellationToken);
            }

            public override void OnNext(Unit value)
            {
                lock (gate)
                {
                    if (!isRunning)
                    {
                        isRunning = true;
                        timer.Reset(); // reuse

                        switch (parent.frameCountType)
                        {
                            case FrameCountType.Update:
                                MainThreadDispatcher.StartUpdateMicroCoroutine(timer);
                                break;
                            case FrameCountType.FixedUpdate:
                                MainThreadDispatcher.StartFixedUpdateMicroCoroutine(timer);
                                break;
                            case FrameCountType.EndOfFrame:
                                MainThreadDispatcher.StartEndOfFrameMicroCoroutine(timer);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                bool running;
                lock (gate)
                {
                    running = isRunning;
                    isCompleted = true;
                }
                if (running)
                {
                    observer.OnNext(Unit.Default);
                }
                try { observer.OnCompleted(); } finally { Dispose(); }
            }

            // reuse, no gc allocate
            private class ReusableEnumerator : System.Collections.IEnumerator
            {
                private readonly BatchFrame parent;
                private int currentFrame;

                public ReusableEnumerator(BatchFrame parent)
                {
                    this.parent = parent;
                }

                public object Current
                {
                    get { return null; }
                }

                public bool MoveNext()
                {
                    if (parent.cancellationToken.IsDisposed) return false;

                    lock (parent.gate)
                    {
                        if (currentFrame++ == parent.parent.frameCount)
                        {
                            if (parent.isCompleted) return false;
                            parent.isRunning = false;

                            // exit lock 
                        }
                        else
                        {
                            return true;
                        }
                    }

                    parent.observer.OnNext(Unit.Default);
                    return false;
                }

                public void Reset()
                {
                    currentFrame = 0;
                }
            }
        }
    }
}