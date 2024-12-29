using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdDamaged : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(this.gameObject, 1.5f);
    }
}
