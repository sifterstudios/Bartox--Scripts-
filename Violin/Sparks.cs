using System;
using System.Collections;
using UnityEngine;

namespace Bartox.Violin
{
    public class Sparks : MonoBehaviour
    {
        [SerializeField] float _delayUntilStashed = 3f;
        ParticleSystem _ps;


        void OnEnable()
        {
            _ps = GetComponentInChildren<ParticleSystem>();
            _ps.Play();
        }

        void Update()
        {
            if (!_ps.IsAlive())
            {
                SparksSpawner.Singleton.Stash(this);
            }
        }
    }
}