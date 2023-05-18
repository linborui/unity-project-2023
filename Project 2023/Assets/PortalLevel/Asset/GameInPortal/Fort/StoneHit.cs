using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneHit : MonoBehaviour
{

    [Header("Basic Settings")]

    public int damage = 500;

    // Start is called before the first frame update
    
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            var otherStates = other.gameObject.GetComponent<CharacterStates>();
            otherStates.TakeDamage(damage,otherStates);
            Destroy(gameObject);
        }
        
    }
}
