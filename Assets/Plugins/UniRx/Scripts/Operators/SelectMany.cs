﻿using System;
using System.Collections.Generic;

namespace UniRx.Operators
{
    internal class SelectManyObservable<TSource, TResult> : OperatorObservableBase<TResult>
    {
        private readonly IObservable<TSource> source;
        private readonly Func<TSource, IObservable<TResult>> selector;
        private readonly Func<TSource, int, IObservable<TResult>> selectorWithIndex;
        private readonly Func<TSource, IEnumerable<TResult>> selectorEnumerable;
        private readonly Func<TSource, int, IEnumerable<TResult>> selectorEnumerableWithIndex;

        public SelectManyObservable(IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selector = selector;
        }

        public SelectManyObservable(IObservable<TSource> source, Func<TSource, int, IObservable<TResult>> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selectorWithIndex = selector;
        }

        public SelectManyObservable(IObservable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selectorEnumerable = selector;
        }

        public SelectManyObservable(IObservable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selectorEnumerableWithIndex = selector;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            if (this.selector != null)
            {
                return new SelectManyOuterObserver(this, observer, cancel).Run();
            }
            else if (this.selectorWithIndex != null)
            {
                return new SelectManyObserverWithIndex(this, observer, cancel).Run();
            }
            else if (this.selectorEnumerable != null)
            {
                return new SelectManyEnumerableObserver(this, observer, cancel).Run();
            }
            else if (this.selectorEnumerableWithIndex != null)
            {
                return new SelectManyEnumerableObserverWithIndex(this, observer, cancel).Run();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private class SelectManyOuterObserver : OperatorObserverBase<TSource, TResult>
        {
            private readonly SelectManyObservable<TSource, TResult> parent;

            private CompositeDisposable collectionDisposable;
            private SingleAssignmentDisposable sourceDisposable;
            private object gate = new object();
            private bool isStopped = false;

            public SelectManyOuterObserver(SelectManyObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                collectionDisposable = new CompositeDisposable();
                sourceDisposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(sourceDisposable);

                sourceDisposable.Disposable = parent.source.Subscribe(this);
                return collectionDisposable;
            }

            public override void OnNext(TSource value)
            {
                IObservable<TResult> nextObservable;
                try
                {
                    nextObservable = parent.selector(value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    ;
                    return;
                }

                var disposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(disposable);
                var collectionObserver = new SelectMany(this, disposable);
                disposable.Disposable = nextObservable.Subscribe(collectionObserver);
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

            private class SelectMany : OperatorObserverBase<TResult, TResult>
            {
                private readonly SelectManyOuterObserver parent;
                private readonly IDisposable cancel;

                public SelectMany(SelectManyOuterObserver parent, IDisposable cancel)
                    : base(parent.observer, cancel)
                {
                    this.parent = parent;
                    this.cancel = cancel;
                }

                public override void OnNext(TResult value)
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

        private class SelectManyObserverWithIndex : OperatorObserverBase<TSource, TResult>
        {
            private readonly SelectManyObservable<TSource, TResult> parent;

            private CompositeDisposable collectionDisposable;
            private int index = 0;
            private object gate = new object();
            private bool isStopped = false;
            private SingleAssignmentDisposable sourceDisposable;

            public SelectManyObserverWithIndex(SelectManyObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                collectionDisposable = new CompositeDisposable();
                sourceDisposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(sourceDisposable);

                sourceDisposable.Disposable = parent.source.Subscribe(this);
                return collectionDisposable;
            }

            public override void OnNext(TSource value)
            {
                IObservable<TResult> nextObservable;
                try
                {
                    nextObservable = parent.selectorWithIndex(value, index++);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    ;
                    return;
                }

                var disposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(disposable);
                var collectionObserver = new SelectMany(this, disposable);
                disposable.Disposable = nextObservable.Subscribe(collectionObserver);
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

            private class SelectMany : OperatorObserverBase<TResult, TResult>
            {
                private readonly SelectManyObserverWithIndex parent;
                private readonly IDisposable cancel;

                public SelectMany(SelectManyObserverWithIndex parent, IDisposable cancel)
                    : base(parent.observer, cancel)
                {
                    this.parent = parent;
                    this.cancel = cancel;
                }

                public override void OnNext(TResult value)
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

        private class SelectManyEnumerableObserver : OperatorObserverBase<TSource, TResult>
        {
            private readonly SelectManyObservable<TSource, TResult> parent;

            public SelectManyEnumerableObserver(SelectManyObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(TSource value)
            {
                IEnumerable<TResult> nextEnumerable;
                try
                {
                    nextEnumerable = parent.selectorEnumerable(value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    ;
                    return;
                }

                var e = nextEnumerable.GetEnumerator();
                try
                {
                    var hasNext = true;
                    while (hasNext)
                    {
                        hasNext = false;
                        var current = default(TResult);

                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext)
                            {
                                current = e.Current;
                            }
                        }
                        catch (Exception exception)
                        {
                            try { observer.OnError(exception); } finally { Dispose(); }
                            return;
                        }

                        if (hasNext)
                        {
                            observer.OnNext(current);
                        }
                    }
                }
                finally
                {
                    if (e != null)
                    {
                        e.Dispose();
                    }
                }
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

        private class SelectManyEnumerableObserverWithIndex : OperatorObserverBase<TSource, TResult>
        {
            private readonly SelectManyObservable<TSource, TResult> parent;
            private int index = 0;

            public SelectManyEnumerableObserverWithIndex(SelectManyObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(TSource value)
            {
                IEnumerable<TResult> nextEnumerable;
                try
                {
                    nextEnumerable = parent.selectorEnumerableWithIndex(value, index++);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                var e = nextEnumerable.GetEnumerator();
                try
                {
                    var hasNext = true;
                    while (hasNext)
                    {
                        hasNext = false;
                        var current = default(TResult);

                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext)
                            {
                                current = e.Current;
                            }
                        }
                        catch (Exception exception)
                        {
                            try { observer.OnError(exception); } finally { Dispose(); }
                            return;
                        }

                        if (hasNext)
                        {
                            observer.OnNext(current);
                        }
                    }
                }
                finally
                {
                    if (e != null)
                    {
                        e.Dispose();
                    }
                }
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

    // with resultSelector
    internal class SelectManyObservable<TSource, TCollection, TResult> : OperatorObservableBase<TResult>
    {
        private readonly IObservable<TSource> source;
        private readonly Func<TSource, IObservable<TCollection>> collectionSelector;
        private readonly Func<TSource, int, IObservable<TCollection>> collectionSelectorWithIndex;
        private readonly Func<TSource, IEnumerable<TCollection>> collectionSelectorEnumerable;
        private readonly Func<TSource, int, IEnumerable<TCollection>> collectionSelectorEnumerableWithIndex;
        private readonly Func<TSource, TCollection, TResult> resultSelector;
        private readonly Func<TSource, int, TCollection, int, TResult> resultSelectorWithIndex;

        public SelectManyObservable(IObservable<TSource> source, Func<TSource, IObservable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.collectionSelector = collectionSelector;
            this.resultSelector = resultSelector;
        }

        public SelectManyObservable(IObservable<TSource> source, Func<TSource, int, IObservable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.collectionSelectorWithIndex = collectionSelector;
            this.resultSelectorWithIndex = resultSelector;
        }

        public SelectManyObservable(IObservable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.collectionSelectorEnumerable = collectionSelector;
            this.resultSelector = resultSelector;
        }

        public SelectManyObservable(IObservable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, int, TCollection, int, TResult> resultSelector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.collectionSelectorEnumerableWithIndex = collectionSelector;
            this.resultSelectorWithIndex = resultSelector;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            if (collectionSelector != null)
            {
                return new SelectManyOuterObserver(this, observer, cancel).Run();
            }
            else if (collectionSelectorWithIndex != null)
            {
                return new SelectManyObserverWithIndex(this, observer, cancel).Run();
            }
            else if (collectionSelectorEnumerable != null)
            {
                return new SelectManyEnumerableObserver(this, observer, cancel).Run();
            }
            else if (collectionSelectorEnumerableWithIndex != null)
            {
                return new SelectManyEnumerableObserverWithIndex(this, observer, cancel).Run();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private class SelectManyOuterObserver : OperatorObserverBase<TSource, TResult>
        {
            private readonly SelectManyObservable<TSource, TCollection, TResult> parent;

            private CompositeDisposable collectionDisposable;
            private object gate = new object();
            private bool isStopped = false;
            private SingleAssignmentDisposable sourceDisposable;

            public SelectManyOuterObserver(SelectManyObservable<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                collectionDisposable = new CompositeDisposable();
                sourceDisposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(sourceDisposable);

                sourceDisposable.Disposable = parent.source.Subscribe(this);
                return collectionDisposable;
            }

            public override void OnNext(TSource value)
            {
                IObservable<TCollection> nextObservable;
                try
                {
                    nextObservable = parent.collectionSelector(value);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                var disposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(disposable);
                var collectionObserver = new SelectMany(this, value, disposable);
                disposable.Disposable = nextObservable.Subscribe(collectionObserver);
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

            private class SelectMany : OperatorObserverBase<TCollection, TResult>
            {
                private readonly SelectManyOuterObserver parent;
                private readonly TSource sourceValue;
                private readonly IDisposable cancel;

                public SelectMany(SelectManyOuterObserver parent, TSource value, IDisposable cancel)
                    : base(parent.observer, cancel)
                {
                    this.parent = parent;
                    this.sourceValue = value;
                    this.cancel = cancel;
                }

                public override void OnNext(TCollection value)
                {
                    TResult resultValue;
                    try
                    {
                        resultValue = parent.parent.resultSelector(sourceValue, value);
                    }
                    catch (Exception ex)
                    {
                        OnError(ex);
                        return;
                    }

                    lock (parent.gate)
                    {
                        base.observer.OnNext(resultValue);
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

        private class SelectManyObserverWithIndex : OperatorObserverBase<TSource, TResult>
        {
            private readonly SelectManyObservable<TSource, TCollection, TResult> parent;

            private CompositeDisposable collectionDisposable;
            private object gate = new object();
            private bool isStopped = false;
            private SingleAssignmentDisposable sourceDisposable;
            private int index = 0;

            public SelectManyObserverWithIndex(SelectManyObservable<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                collectionDisposable = new CompositeDisposable();
                sourceDisposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(sourceDisposable);

                sourceDisposable.Disposable = parent.source.Subscribe(this);
                return collectionDisposable;
            }

            public override void OnNext(TSource value)
            {
                var i = index++;
                IObservable<TCollection> nextObservable;
                try
                {
                    nextObservable = parent.collectionSelectorWithIndex(value, i);
                }
                catch (Exception ex)
                {
                    OnError(ex);
                    return;
                }

                var disposable = new SingleAssignmentDisposable();
                collectionDisposable.Add(disposable);
                var collectionObserver = new SelectManyObserver(this, value, i, disposable);
                disposable.Disposable = nextObservable.Subscribe(collectionObserver);
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

            private class SelectManyObserver : OperatorObserverBase<TCollection, TResult>
            {
                private readonly SelectManyObserverWithIndex parent;
                private readonly TSource sourceValue;
                private readonly int sourceIndex;
                private readonly IDisposable cancel;
                private int index;

                public SelectManyObserver(SelectManyObserverWithIndex parent, TSource value, int index, IDisposable cancel)
                    : base(parent.observer, cancel)
                {
                    this.parent = parent;
                    this.sourceValue = value;
                    this.sourceIndex = index;
                    this.cancel = cancel;
                }

                public override void OnNext(TCollection value)
                {
                    TResult resultValue;
                    try
                    {
                        resultValue = parent.parent.resultSelectorWithIndex(sourceValue, sourceIndex, value, index++);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); }
                        ;
                        return;
                    }

                    lock (parent.gate)
                    {
                        base.observer.OnNext(resultValue);
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

        private class SelectManyEnumerableObserver : OperatorObserverBase<TSource, TResult>
        {
            private readonly SelectManyObservable<TSource, TCollection, TResult> parent;

            public SelectManyEnumerableObserver(SelectManyObservable<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(TSource value)
            {
                IEnumerable<TCollection> nextEnumerable;
                try
                {
                    nextEnumerable = parent.collectionSelectorEnumerable(value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    ;
                    return;
                }

                var e = nextEnumerable.GetEnumerator();
                try
                {
                    var hasNext = true;
                    while (hasNext)
                    {
                        hasNext = false;
                        var current = default(TResult);

                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext)
                            {
                                current = parent.resultSelector(value, e.Current);
                            }
                        }
                        catch (Exception exception)
                        {
                            try { observer.OnError(exception); } finally { Dispose(); }
                            return;
                        }

                        if (hasNext)
                        {
                            observer.OnNext(current);
                        }
                    }
                }
                finally
                {
                    if (e != null)
                    {
                        e.Dispose();
                    }
                }
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

        private class SelectManyEnumerableObserverWithIndex : OperatorObserverBase<TSource, TResult>
        {
            private readonly SelectManyObservable<TSource, TCollection, TResult> parent;
            private int index = 0;

            public SelectManyEnumerableObserverWithIndex(SelectManyObservable<TSource, TCollection, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(TSource value)
            {
                var i = index++;
                IEnumerable<TCollection> nextEnumerable;
                try
                {
                    nextEnumerable = parent.collectionSelectorEnumerableWithIndex(value, i);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    ;
                    return;
                }

                var e = nextEnumerable.GetEnumerator();
                try
                {
                    var sequenceI = 0;
                    var hasNext = true;
                    while (hasNext)
                    {
                        hasNext = false;
                        var current = default(TResult);

                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext)
                            {
                                current = parent.resultSelectorWithIndex(value, i, e.Current, sequenceI++);
                            }
                        }
                        catch (Exception exception)
                        {
                            try { observer.OnError(exception); } finally { Dispose(); }
                            return;
                        }

                        if (hasNext)
                        {
                            observer.OnNext(current);
                        }
                    }
                }
                finally
                {
                    if (e != null)
                    {
                        e.Dispose();
                    }
                }
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