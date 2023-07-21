using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{

    [DisallowMultipleComponent]
    public class TeamCircle : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem circle;
    
        [SerializeField]
        private ParticleSystem innercircle;
    
        public float CircleRadius
        {
            get { return circle.shape.radius; }

            set
            {
                ParticleSystem.ShapeModule shape;

                shape = circle.shape;
                shape.radius = value;

                shape = innercircle.shape;
                shape.radius = value - 0.1f;

            }
        }

        public void StopParticle()
        {
            circle.Stop();
            innercircle.Stop();
        }

        public void StartParticle()
        {
            circle.Play();     
            innercircle.Play();
        }
    }
}
