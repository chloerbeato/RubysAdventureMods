using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    public GameObject pickupParticlesPrefab;

    public AudioClip collectedClip;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            	controller.speed=5.0f;
                GameObject pickupParticleObject = Instantiate(pickupParticlesPrefab, transform.position, Quaternion.identity);
            	Destroy(gameObject);
            	controller.PlaySound(collectedClip);
        }

    }
}