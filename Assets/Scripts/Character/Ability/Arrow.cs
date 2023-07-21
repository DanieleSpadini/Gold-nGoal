using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core;
using UnityEngine.Events;

namespace Ability
{
    /// <summary>
    /// This represents the arrow used by the goblin's ability.
    /// </summary>
    public class Arrow : MonoBehaviour
    {
        // Speed of the arrow.
        [SerializeField, Tooltip("Speed of the arrow (m/s).")]
        private float _arrowSpeed;

        // Debuff...
        [SerializeField, Tooltip("Debuff.")]
        public ModifierDefinition _modifierAbility;

        // The player who fired...
        [SerializeField]
        private Player _arrowShooter;

        // Counter to destroy arrow over time limit...
        private Timer _timer;

        // VFx ref...
        [SerializeField]
        private ParticleSystem _stunVFX;

        /// <summary>
        /// Configure the arrow.
        /// </summary>
        /// <param name="arrowSpeed"></param>
        /// <param name="modifierDefinition"></param>
        /// <param name="playerShooter"></param>
        public void Init(float arrowSpeed, ModifierDefinition modifierDefinition, Player playerShooter)
        {
            _arrowSpeed = arrowSpeed;

            _modifierAbility = modifierDefinition;

            _arrowShooter = playerShooter;

            _timer = Timer.CreateTimer($"{this} - Arrow timer", 10f, true, false, this.Dispose);
        }

        /// <summary>
        /// Destroy the arrow if time is over...
        /// </summary>
        private void Dispose()
        {
            //Debug.Log($"{this}: Didn't caught opponents...");
            Destroy(gameObject);
        }

        #region lifecycle method
        void Update()
        {
            // Move the arrow frame by frame...
            transform.position += transform.forward * _arrowSpeed * Time.deltaTime;   
        }

        // if arrow collides with something...
        private void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent<Player>(out Player target);

            // If that's who shot it ignore it...
            if (target == _arrowShooter)
            {
                return;
            }
            // Otherwise, if it's an opponent, drops the debuff...
            else if (!(MatchManager.Instance.GetTeamByPlayerRef(_arrowShooter).Players.Contains(target)))
            {
                target.AddModifier(Instantiate<ModifierDefinition>(_modifierAbility));
                target.Animator.SetBool("WasHit", true);
                ParticleSystem stunvfx = Instantiate(_stunVFX, target.transform);
                stunvfx.Emit(2);
                stunvfx.Play();
                _timer.Cancel(); 
                Destroy(gameObject);
            }
        }
        #endregion
    }

}
