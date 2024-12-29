using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDamaged : MonoBehaviour // 효과음, 파티클 X
{
    public Sprite newSprite;
    private SpriteRenderer spriteRenderer;
    private bool isChanged = false;
    private bool isDestroyed = false;
    
    public AudioClip damagedSound; 
    private AudioSource audioSource;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && !isDestroyed)
        {
            if (isChanged)
            {
                isDestroyed = true;
                if (audioSource != null && damagedSound != null)
                    audioSource.PlayOneShot(damagedSound);
                
                Destroy(gameObject, damagedSound.length);
            }

            ChangeSprite();
        }
    }
    
    void ChangeSprite()
    {
        if (spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
            isChanged = true;
        }
    }
}
