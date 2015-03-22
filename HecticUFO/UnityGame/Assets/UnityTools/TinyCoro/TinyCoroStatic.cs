using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTools_4_6
{
    public partial class TinyCoro
    {
        /// <summary>
        /// Adds an new TinyCoro to the pool after the currently exceutiny one (or last if none are running) 
        /// </summary>
        /// <param name="threadedOperation"></param>
        /// <returns></returns>
        public static TinyCoro SpawnNext(Func<IEnumerator> operation)
        {
            var coro = new TinyCoro(operation);
            var index = _allCoros.Count;
            if (_currentCoro != null)
                index = _allCoros.IndexOf(_currentCoro) + 1;
            _allCoros.Insert(index, coro);
            return coro;
        }

        public static void StepAllCoros()
        {
            for (int i = 0; i < _allCoros.Count; )
            {
                //Step normal
                _currentCoro = _allCoros[i];
                if (_currentCoro.Alive)
                    _currentCoro.Step();
                if (!_currentCoro.Alive)
                {
                    _allCoros.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
            _currentCoro = null;
        }


        /// <summary>
        /// Human readable version of "yield return of Func<bool>"
        /// Any half decent compiler should optimize this :P
        /// </summary>
        /// <param name="conditionMet"></param>
        /// <returns></returns>
        public static Func<bool> WaitUntil(Func<bool> conditionMet)
        {
            return conditionMet;
        }

        public static Func<bool> Wait(float seconds)
        {
            var destinationTime = Time.time + seconds;
            return () => Time.time >= destinationTime;
        }

        public static IEnumerable<TinyCoro> AllCoroutines { get { return _allCoros; } }

        private static List<TinyCoro> _allCoros = new List<TinyCoro>();
        private static TinyCoro _currentCoro;
    }
}
