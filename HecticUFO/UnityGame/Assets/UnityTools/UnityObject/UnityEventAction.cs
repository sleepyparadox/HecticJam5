using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityTools_4_6
{
    public class UnityEventAction<T0>
    {
        private event Action<T0> _action;
        public void Fire(T0 val)
        {
            _action(val);
        }

        /// <summary>
        /// Returns event if there wasn't one before
        /// </summary>
        /// <param name="unityAction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static UnityEventAction<T0> operator +(UnityEventAction<T0> unityAction, Action<T0> action)
        {
            if (unityAction == null)
                unityAction = new UnityEventAction<T0>();
            unityAction._action += action;
            return unityAction;
        }

        /// <summary>
        /// Returns null if nothing is subscribed
        /// </summary>
        /// <param name="unityAction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static UnityEventAction<T0> operator -(UnityEventAction<T0> unityAction, Action<T0> action)
        {
            if (unityAction == null)
                return null;

            unityAction._action -= action;

            if (unityAction._action == null)
                return null;
            
            return unityAction;
        }
    }

    public class UnityEventAction<T0, T1>
    {
        private event Action<T0, T1> _action;
        public void Fire(T0 val0, T1 val1)
        {
            _action(val0, val1);
        }

        /// <summary>
        /// Returns event if there wasn't one before
        /// </summary>
        /// <param name="unityAction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static UnityEventAction<T0, T1> operator +(UnityEventAction<T0, T1> unityAction, Action<T0, T1> action)
        {
            if (unityAction == null)
                unityAction = new UnityEventAction<T0, T1>();
            unityAction._action += action;
            return unityAction;
        }

        /// <summary>
        /// Returns null if nothing is subscribed
        /// </summary>
        /// <param name="unityAction"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static UnityEventAction<T0, T1> operator -(UnityEventAction<T0, T1> unityAction, Action<T0, T1> action)
        {
            if (unityAction == null)
                return null;

            unityAction._action -= action;

            if (unityAction._action == null)
                return null;

            return unityAction;
        }
    }
}
