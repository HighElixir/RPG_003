﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
    public partial struct UniTask
    {
        private static readonly UniTask CanceledUniTask = new Func<UniTask>(() =>
        {
            return new UniTask(new CanceledResultSource(CancellationToken.None), 0);
        })();

        private static class CanceledUniTaskCache<T>
        {
            public static readonly UniTask<T> Task;

            static CanceledUniTaskCache()
            {
                Task = new UniTask<T>(new CanceledResultSource<T>(CancellationToken.None), 0);
            }
        }

        public static readonly UniTask CompletedTask = new UniTask();

        public static UniTask FromException(Exception ex)
        {
            if (ex is OperationCanceledException oce)
            {
                return FromCanceled(oce.CancellationToken);
            }

            return new UniTask(new ExceptionResultSource(ex), 0);
        }

        public static UniTask<T> FromException<T>(Exception ex)
        {
            if (ex is OperationCanceledException oce)
            {
                return FromCanceled<T>(oce.CancellationToken);
            }

            return new UniTask<T>(new ExceptionResultSource<T>(ex), 0);
        }

        public static UniTask<T> FromResult<T>(T value)
        {
            return new UniTask<T>(value);
        }

        public static UniTask FromCanceled(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == CancellationToken.None)
            {
                return CanceledUniTask;
            }
            else
            {
                return new UniTask(new CanceledResultSource(cancellationToken), 0);
            }
        }

        public static UniTask<T> FromCanceled<T>(CancellationToken cancellationToken = default)
        {
            if (cancellationToken == CancellationToken.None)
            {
                return CanceledUniTaskCache<T>.Task;
            }
            else
            {
                return new UniTask<T>(new CanceledResultSource<T>(cancellationToken), 0);
            }
        }

        public static UniTask Create(Func<UniTask> factory)
        {
            return factory();
        }

        public static UniTask Create(Func<CancellationToken, UniTask> factory, CancellationToken cancellationToken)
        {
            return factory(cancellationToken);
        }

        public static UniTask Create<T>(T state, Func<T, UniTask> factory)
        {
            return factory(state);
        }

        public static UniTask<T> Create<T>(Func<UniTask<T>> factory)
        {
            return factory();
        }

        public static AsyncLazy Lazy(Func<UniTask> factory)
        {
            return new AsyncLazy(factory);
        }

        public static AsyncLazy<T> Lazy<T>(Func<UniTask<T>> factory)
        {
            return new AsyncLazy<T>(factory);
        }

        /// <summary>
        /// helper of fire and forget void action.
        /// </summary>
        public static void Void(Func<UniTaskVoid> asyncAction)
        {
            asyncAction().Forget();
        }

        /// <summary>
        /// helper of fire and forget void action.
        /// </summary>
        public static void Void(Func<CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
        {
            asyncAction(cancellationToken).Forget();
        }

        /// <summary>
        /// helper of fire and forget void action.
        /// </summary>
        public static void Void<T>(Func<T, UniTaskVoid> asyncAction, T state)
        {
            asyncAction(state).Forget();
        }

        /// <summary>
        /// helper of create add UniTaskVoid to delegate.
        /// For example: FooAction = UniTask.Action(async () => { /* */ })
        /// </summary>
        public static Action Action(Func<UniTaskVoid> asyncAction)
        {
            return () => asyncAction().Forget();
        }

        /// <summary>
        /// helper of create add UniTaskVoid to delegate.
        /// </summary>
        public static Action Action(Func<CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
        {
            return () => asyncAction(cancellationToken).Forget();
        }

        /// <summary>
        /// helper of create add UniTaskVoid to delegate.
        /// </summary>
        public static Action Action<T>(T state, Func<T, UniTaskVoid> asyncAction)
        {
            return () => asyncAction(state).Forget();
        }

#if UNITY_2018_3_OR_NEWER

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(async () => { /* */ } ))
        /// </summary>
        public static UnityEngine.Events.UnityAction UnityAction(Func<UniTaskVoid> asyncAction)
        {
            return () => asyncAction().Forget();
        }

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(FooAsync, this.GetCancellationTokenOnDestroy()))
        /// </summary>
        public static UnityEngine.Events.UnityAction UnityAction(Func<CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
        {
            return () => asyncAction(cancellationToken).Forget();
        }

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(FooAsync, Argument))
        /// </summary>
        public static UnityEngine.Events.UnityAction UnityAction<T>(T state, Func<T, UniTaskVoid> asyncAction)
        {
            return () => asyncAction(state).Forget();
        }

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(async (T arg) => { /* */ } ))
        /// </summary>
        public static UnityEngine.Events.UnityAction<T> UnityAction<T>(Func<T, UniTaskVoid> asyncAction)
        {
            return (arg) => asyncAction(arg).Forget();
        }

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(async (T0 arg0, T1 arg1) => { /* */ } ))
        /// </summary>
        public static UnityEngine.Events.UnityAction<T0, T1> UnityAction<T0, T1>(Func<T0, T1, UniTaskVoid> asyncAction)
        {
            return (arg0, arg1) => asyncAction(arg0, arg1).Forget();
        }

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(async (T0 arg0, T1 arg1, T2 arg2) => { /* */ } ))
        /// </summary>
        public static UnityEngine.Events.UnityAction<T0, T1, T2> UnityAction<T0, T1, T2>(Func<T0, T1, T2, UniTaskVoid> asyncAction)
        {
            return (arg0, arg1, arg2) => asyncAction(arg0, arg1, arg2).Forget();
        }

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(async (T0 arg0, T1 arg1, T2 arg2, T3 arg3) => { /* */ } ))
        /// </summary>
        public static UnityEngine.Events.UnityAction<T0, T1, T2, T3> UnityAction<T0, T1, T2, T3>(Func<T0, T1, T2, T3, UniTaskVoid> asyncAction)
        {
            return (arg0, arg1, arg2, arg3) => asyncAction(arg0, arg1, arg2, arg3).Forget();
        }

        // <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(async (T arg, CancellationToken cancellationToken) => { /* */ } ))
        /// </summary>
        public static UnityEngine.Events.UnityAction<T> UnityAction<T>(Func<T, CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
        {
            return (arg) => asyncAction(arg, cancellationToken).Forget();
        }

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(async (T0 arg0, T1 arg1, CancellationToken cancellationToken) => { /* */ } ))
        /// </summary>
        public static UnityEngine.Events.UnityAction<T0, T1> UnityAction<T0, T1>(Func<T0, T1, CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
        {
            return (arg0, arg1) => asyncAction(arg0, arg1, cancellationToken).Forget();
        }

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(async (T0 arg0, T1 arg1, T2 arg2, CancellationToken cancellationToken) => { /* */ } ))
        /// </summary>
        public static UnityEngine.Events.UnityAction<T0, T1, T2> UnityAction<T0, T1, T2>(Func<T0, T1, T2, CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
        {
            return (arg0, arg1, arg2) => asyncAction(arg0, arg1, arg2, cancellationToken).Forget();
        }

        /// <summary>
        /// Create async void(UniTaskVoid) UnityAction.
        /// For example: onClick.AddListener(UniTask.UnityAction(async (T0 arg0, T1 arg1, T2 arg2, T3 arg3, CancellationToken cancellationToken) => { /* */ } ))
        /// </summary>
        public static UnityEngine.Events.UnityAction<T0, T1, T2, T3> UnityAction<T0, T1, T2, T3>(Func<T0, T1, T2, T3, CancellationToken, UniTaskVoid> asyncAction, CancellationToken cancellationToken)
        {
            return (arg0, arg1, arg2, arg3) => asyncAction(arg0, arg1, arg2, arg3, cancellationToken).Forget();
        }

#endif

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static UniTask Defer(Func<UniTask> factory)
        {
            return new UniTask(new DeferPromise(factory), 0);
        }

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static UniTask<T> Defer<T>(Func<UniTask<T>> factory)
        {
            return new UniTask<T>(new DeferPromise<T>(factory), 0);
        }

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static UniTask Defer<TState>(TState state, Func<TState, UniTask> factory)
        {
            return new UniTask(new DeferPromiseWithState<TState>(state, factory), 0);
        }

        /// <summary>
        /// Defer the task creation just before call await.
        /// </summary>
        public static UniTask<TResult> Defer<TState, TResult>(TState state, Func<TState, UniTask<TResult>> factory)
        {
            return new UniTask<TResult>(new DeferPromiseWithState<TState, TResult>(state, factory), 0);
        }

        /// <summary>
        /// Never complete.
        /// </summary>
        public static UniTask Never(CancellationToken cancellationToken)
        {
            return new UniTask<AsyncUnit>(new NeverPromise<AsyncUnit>(cancellationToken), 0);
        }

        /// <summary>
        /// Never complete.
        /// </summary>
        public static UniTask<T> Never<T>(CancellationToken cancellationToken)
        {
            return new UniTask<T>(new NeverPromise<T>(cancellationToken), 0);
        }

        private sealed class ExceptionResultSource : IUniTaskSource
        {
            private readonly ExceptionDispatchInfo exception;
            private bool calledGet;

            public ExceptionResultSource(Exception exception)
            {
                this.exception = ExceptionDispatchInfo.Capture(exception);
            }

            public void GetResult(short token)
            {
                if (!calledGet)
                {
                    calledGet = true;
                    GC.SuppressFinalize(this);
                }
                exception.Throw();
            }

            public UniTaskStatus GetStatus(short token)
            {
                return UniTaskStatus.Faulted;
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return UniTaskStatus.Faulted;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }

            ~ExceptionResultSource()
            {
                if (!calledGet)
                {
                    UniTaskScheduler.PublishUnobservedTaskException(exception.SourceException);
                }
            }
        }

        private sealed class ExceptionResultSource<T> : IUniTaskSource<T>
        {
            private readonly ExceptionDispatchInfo exception;
            private bool calledGet;

            public ExceptionResultSource(Exception exception)
            {
                this.exception = ExceptionDispatchInfo.Capture(exception);
            }

            public T GetResult(short token)
            {
                if (!calledGet)
                {
                    calledGet = true;
                    GC.SuppressFinalize(this);
                }
                exception.Throw();
                return default;
            }

            void IUniTaskSource.GetResult(short token)
            {
                if (!calledGet)
                {
                    calledGet = true;
                    GC.SuppressFinalize(this);
                }
                exception.Throw();
            }

            public UniTaskStatus GetStatus(short token)
            {
                return UniTaskStatus.Faulted;
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return UniTaskStatus.Faulted;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }

            ~ExceptionResultSource()
            {
                if (!calledGet)
                {
                    UniTaskScheduler.PublishUnobservedTaskException(exception.SourceException);
                }
            }
        }

        private sealed class CanceledResultSource : IUniTaskSource
        {
            private readonly CancellationToken cancellationToken;

            public CanceledResultSource(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
            }

            public void GetResult(short token)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return UniTaskStatus.Canceled;
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return UniTaskStatus.Canceled;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }
        }

        private sealed class CanceledResultSource<T> : IUniTaskSource<T>
        {
            private readonly CancellationToken cancellationToken;

            public CanceledResultSource(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
            }

            public T GetResult(short token)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            void IUniTaskSource.GetResult(short token)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return UniTaskStatus.Canceled;
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return UniTaskStatus.Canceled;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                continuation(state);
            }
        }

        private sealed class DeferPromise : IUniTaskSource
        {
            private Func<UniTask> factory;
            private UniTask task;
            private UniTask.Awaiter awaiter;

            public DeferPromise(Func<UniTask> factory)
            {
                this.factory = factory;
            }

            public void GetResult(short token)
            {
                awaiter.GetResult();
            }

            public UniTaskStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref factory, null);
                if (f != null)
                {
                    task = f();
                    awaiter = task.GetAwaiter();
                }

                return task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return task.Status;
            }
        }

        private sealed class DeferPromise<T> : IUniTaskSource<T>
        {
            private Func<UniTask<T>> factory;
            private UniTask<T> task;
            private UniTask<T>.Awaiter awaiter;

            public DeferPromise(Func<UniTask<T>> factory)
            {
                this.factory = factory;
            }

            public T GetResult(short token)
            {
                return awaiter.GetResult();
            }

            void IUniTaskSource.GetResult(short token)
            {
                awaiter.GetResult();
            }

            public UniTaskStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref factory, null);
                if (f != null)
                {
                    task = f();
                    awaiter = task.GetAwaiter();
                }

                return task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return task.Status;
            }
        }

        private sealed class DeferPromiseWithState<TState> : IUniTaskSource
        {
            private Func<TState, UniTask> factory;
            private TState argument;
            private UniTask task;
            private UniTask.Awaiter awaiter;

            public DeferPromiseWithState(TState argument, Func<TState, UniTask> factory)
            {
                this.argument = argument;
                this.factory = factory;
            }

            public void GetResult(short token)
            {
                awaiter.GetResult();
            }

            public UniTaskStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref factory, null);
                if (f != null)
                {
                    task = f(argument);
                    awaiter = task.GetAwaiter();
                }

                return task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return task.Status;
            }
        }

        private sealed class DeferPromiseWithState<TState, TResult> : IUniTaskSource<TResult>
        {
            private Func<TState, UniTask<TResult>> factory;
            private TState argument;
            private UniTask<TResult> task;
            private UniTask<TResult>.Awaiter awaiter;

            public DeferPromiseWithState(TState argument, Func<TState, UniTask<TResult>> factory)
            {
                this.argument = argument;
                this.factory = factory;
            }

            public TResult GetResult(short token)
            {
                return awaiter.GetResult();
            }

            void IUniTaskSource.GetResult(short token)
            {
                awaiter.GetResult();
            }

            public UniTaskStatus GetStatus(short token)
            {
                var f = Interlocked.Exchange(ref factory, null);
                if (f != null)
                {
                    task = f(argument);
                    awaiter = task.GetAwaiter();
                }

                return task.Status;
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                awaiter.SourceOnCompleted(continuation, state);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return task.Status;
            }
        }

        private sealed class NeverPromise<T> : IUniTaskSource<T>
        {
            private static readonly Action<object> cancellationCallback = CancellationCallback;

            private CancellationToken cancellationToken;
            private UniTaskCompletionSourceCore<T> core;

            public NeverPromise(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
                if (this.cancellationToken.CanBeCanceled)
                {
                    this.cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
                }
            }

            private static void CancellationCallback(object state)
            {
                var self = (NeverPromise<T>)state;
                self.core.TrySetCanceled(self.cancellationToken);
            }

            public T GetResult(short token)
            {
                return core.GetResult(token);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            void IUniTaskSource.GetResult(short token)
            {
                core.GetResult(token);
            }
        }
    }

    internal static class CompletedTasks
    {
        public static readonly UniTask<AsyncUnit> AsyncUnit = UniTask.FromResult(Cysharp.Threading.Tasks.AsyncUnit.Default);
        public static readonly UniTask<bool> True = UniTask.FromResult(true);
        public static readonly UniTask<bool> False = UniTask.FromResult(false);
        public static readonly UniTask<int> Zero = UniTask.FromResult(0);
        public static readonly UniTask<int> MinusOne = UniTask.FromResult(-1);
        public static readonly UniTask<int> One = UniTask.FromResult(1);
    }
}
