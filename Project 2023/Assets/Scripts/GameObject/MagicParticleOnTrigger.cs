using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicParticleOnTrigger : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in particles) {
                particle.Play();
            }
        }
    }
}
