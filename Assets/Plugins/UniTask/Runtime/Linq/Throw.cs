﻿using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
    public static partial class UniTaskAsyncEnumerable
    {
        public static IUniTaskAsyncEnumerable<TValue> Throw<TValue>(Exception exception)
        {
            return new Throw<TValue>(exception);
        }
    }

    internal class Throw<TValue> : IUniTaskAsyncEnumerable<TValue>
    {
        private readonly Exception exception;

        public Throw(Exception exception)
        {
            this.exception = exception;
        }

        public IUniTaskAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new _Throw(exception, cancellationToken);
        }

        private class _Throw : IUniTaskAsyncEnumerator<TValue>
        {
            private readonly Exception exception;
            private CancellationToken cancellationToken;

            public _Throw(Exception exception, CancellationToken cancellationToken)
            {
                this.exception = exception;
                this.cancellationToken = cancellationToken;
            }

            public TValue Current => default;

            public UniTask<bool> MoveNextAsync()
            {
                cancellationToken.ThrowIfCancellationRequested();
                return UniTask.FromException<bool>(exception);
            }

            public UniTask DisposeAsync()
            {
                return default;
            }
        }
    }
}