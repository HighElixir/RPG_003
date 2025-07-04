﻿using System;

namespace UniRx.Operators
{
    internal class PairwiseObservable<T, TR> : OperatorObservableBase<TR>
    {
        private readonly IObservable<T> source;
        private readonly Func<T, T, TR> selector;

        public PairwiseObservable(IObservable<T> source, Func<T, T, TR> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selector = selector;
        }

        protected override IDisposable SubscribeCore(IObserver<TR> observer, IDisposable cancel)
        {
            return source.Subscribe(new Pairwise(this, observer, cancel));
        }

        private class Pairwise : OperatorObserverBase<T, TR>
        {
            private readonly PairwiseObservable<T, TR> parent;
            private T prev = default(T);
            private bool isFirst = true;

            public Pairwise(PairwiseObservable<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    prev = value;
                    return;
                }

                TR v;
                try
                {
                    v = parent.selector(prev, value);
                    prev = value;
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    return;
                }

                observer.OnNext(v);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }
    }

    internal class PairwiseObservable<T> : OperatorObservableBase<Pair<T>>
    {
        private readonly IObservable<T> source;

        public PairwiseObservable(IObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<Pair<T>> observer, IDisposable cancel)
        {
            return source.Subscribe(new Pairwise(observer, cancel));
        }

        private class Pairwise : OperatorObserverBase<T, Pair<T>>
        {
            private T prev = default(T);
            private bool isFirst = true;

            public Pairwise(IObserver<Pair<T>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    prev = value;
                    return;
                }

                var pair = new Pair<T>(prev, value);
                prev = value;
                observer.OnNext(pair);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }
    }
}