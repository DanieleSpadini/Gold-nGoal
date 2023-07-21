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

namespace Core
{
    /// <summary>
    /// This class handles the ball and its movements.
    /// </summary>
    [DisallowMultipleComponent]
	public class Ball : MonoBehaviour
	{
        [Header("Configs")]
        // Ball speed curve. At point 0 it is the speed at the moment of departure.
        // Typically the speed decreases over time.
        [SerializeField]
		private AnimationCurve _movementCurve;

        // Flag to indicate if the ball is released by the player.
		private bool _isFreeToRolls = false;
		public bool IsFreeToRolls { get => _isFreeToRolls; }

		[Space(20)]
		[Header("Debug")]
		// Ball direction.
		private Vector3 _direction;

		// Ball speed.
		private float _speed;

		// Actual ball speed.
        private float _currentSpeed;
		public float CurrentSpeed { get =>_currentSpeed; }

        // Counter used to evaluate the speed of the ball as a function of the passage of time.
        private float _time;

        #region lifecycle method
        private void Update()
		{
			// If the ball is released by the character...
			if (IsFreeToRolls)
			{
				// Increase counter...
                _time += Time.deltaTime;

				// Evaluate speed as a function of time...
                _currentSpeed = _movementCurve.Evaluate(_time) * _speed;

				// Update ball position...
                transform.position += _direction * Time.deltaTime * _currentSpeed;
			}
		}

        /// <summary>
        /// Manages the bounce of the ball on the banks.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag(Globals.LEFT_BARRIER))
			{
				_direction = new Vector3(-_direction.x, 0, _direction.z);
			}
			else if (other.CompareTag(Globals.RIGHT_BARRIER))
			{
				_direction = new Vector3(-_direction.x, 0, _direction.z);
            }

            if (other.CompareTag(Globals.TOP_GOAL_AREA) || other.CompareTag(Globals.BOTTOM_GOAL_AREA))
			{
				_direction = Vector3.zero;
                _speed = 0;
            }
        }
        #endregion

        /// <summary>
        /// Called when a character throws the ball in the direction of a teammate.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <param name="speed">Speed</param>
        public void ShootTowards(Vector3 position, float speed)
		{
			Vector3 to = new Vector3(position.x, 0, position.z);
			Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

			_speed = speed;

			_direction = (to - from).normalized;

            _time = 0;
            _currentSpeed = _movementCurve.Evaluate(_time) * _speed;
            _isFreeToRolls = true;			
		}

        /// <summary>
        /// Called when the ball is blocked by a character.
        /// </summary>
        /// <param name="ballSpot"></param>
        public void BallStop(Transform ballSpot)
		{
			_direction = Vector3.zero;
			_speed = 0;
			_currentSpeed = 0;
			_isFreeToRolls = false;
		}
	}
}
