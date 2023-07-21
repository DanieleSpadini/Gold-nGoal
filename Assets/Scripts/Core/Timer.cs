using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using S = System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.Events;

namespace Core
{
    /// <summary>
    /// This class manages the stopwatches used for the various cooldowns and for the time of the match.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField]
        private float _duration = 0;
        [SerializeField]
        private float _timeRemaining = 0;
        [SerializeField]
        private bool _timerIsRunning = false;
        [SerializeField]
        private bool _timerIsPaused = false;
        [SerializeField]
        private bool _isRecurring = false;

        // Callback performed when time is over...
        private event UnityAction OnTimeRunOut;

        public float TimeRemaining { get => _timeRemaining; }

        public float Duration { get => _duration; }

        /// <summary>
        /// Suspend the stopwatch.
        /// </summary>
        public void Pause() => _timerIsPaused = true;

        /// <summary>
        /// Resume the stopwatch.
        /// </summary>
        public void Resume() => _timerIsPaused = false;

        /// <summary>
        /// Stop and delete the stopwatch.
        /// </summary>
        public void Cancel()
        {
            OnTimeRunOut = null;
            Destroy(gameObject);
        }

        #region lifecycle method
        void Update()
        {
            if (_timerIsRunning && !_timerIsPaused)
            {
                if (_timeRemaining > 0)
                {
                    _timeRemaining -= Time.deltaTime;
                }
                else
                {
                    OnTimeRunOut?.Invoke();

                    _timeRemaining = 0;
                    _timerIsRunning = false;
                }
            }
            else if (!_timerIsPaused)
            {
                if (_isRecurring)
                {
                    _timeRemaining = _duration;
                    _timerIsRunning = true;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        #endregion

        /// <summary>
        /// Create a new stopwatch.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="duration"></param>
        /// <param name="startImmediately"></param>
        /// <param name="recurring"></param>
        /// <param name="actionAtTimeRunsOut"></param>
        /// <returns></returns>
        public static Timer CreateTimer(string name, float duration, bool startImmediately, bool recurring, UnityAction actionAtTimeRunsOut)
        {
            Timer timer;

            GameObject go = new GameObject();
            go.name = name;

            timer = go.AddComponent<Timer>();

            timer.Init(duration, startImmediately, recurring, actionAtTimeRunsOut);

            return timer;
        }

        private void Init(float duration, bool startImmediately, bool recurring, UnityAction actionAtTimeRunsOut)
        {
            _duration = duration;
            _timeRemaining = duration;

            _timerIsRunning = startImmediately;

            _isRecurring = recurring;

            OnTimeRunOut += actionAtTimeRunsOut;
        }
    }
}
