using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public Sprite newSprite;
    private SpriteRenderer spriteRenderer;
    private bool isChanged = false;
    private bool isDestroyed = false;
    public GameObject particlePrefab;
    
    public AudioClip damagedSound; 
    private AudioSource audioSource; 
    
    public delegate void PigDestroyed();
    public static event PigDestroyed OnPigDestroyed; // 이벤트 밖에서 구독
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && !isDestroyed)
        {
            Debug.Log("magnitude: "+ other.relativeVelocity.magnitude);
            if (other.relativeVelocity.magnitude >= 7.5f)
            {
                if (isChanged) // Sprite 한 번 바뀌고 Destory
                    DestroyObject();
                            
                ChangeSprite();
            }
        }
    }
    
    private void OnDestroy()
    {
        OnPigDestroyed?.Invoke();
    }
    
    public void DestroyObject()
    {
        isDestroyed = true;
        if (audioSource != null && damagedSound != null)
            audioSource.PlayOneShot(damagedSound); // 효과음

        Invoke("InstantiateParticle", damagedSound.length / 2); // 사운드 좀 출력하고 파티클 나오게(차이 많이 안남..)
        
        Destroy(gameObject, damagedSound.length); // 효과음 다 나오고 삭제하게
    }

    void InstantiateParticle()
    {
        if (particlePrefab != null) 
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
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
