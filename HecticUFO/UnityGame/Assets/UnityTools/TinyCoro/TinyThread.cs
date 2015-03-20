using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnityTools_4_6
{
    public static class TinyThread
    {
        public static TinyCoro SpawnAfter(Action threadedOperation)
        {
            return TinyCoro.SpawnNext(() => PreformThreadedOperation(threadedOperation));
        }

        private static IEnumerator PreformThreadedOperation(Action threadedOperation)
        {
            var thread = new Thread(new ThreadStart(threadedOperation));

            thread.Start();

            yield return TinyCoro.WaitUntil(() => !thread.IsAlive);
        }
    }

    public static class TinyThread<TResult>
    {
        public static TinyCoro SpawnAfter(Func<TResult> threadedOperation, Action<TResult> onSuccess)
        {
            return TinyCoro.SpawnNext(() => PreformThreadedOperation(threadedOperation, onSuccess));
        }

        private static IEnumerator PreformThreadedOperation(Func<TResult> threadedOperation, Action<TResult> onSuccess)
        {
            object threadResult = null;

            var thread = new Thread(new ThreadStart(() =>
            {
                threadResult = threadedOperation();
            }));

            thread.Start();

            yield return TinyCoro.WaitUntil(() => !thread.IsAlive);

            onSuccess((TResult)threadResult);
        }
    }
}
