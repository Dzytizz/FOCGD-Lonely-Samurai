using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public ProjectileType projectileType;
    [SerializeField, Tooltip("Set to 0 if infinite")] private float projectileLifetime;
    [SerializeField] private int damage;
    [SerializeField] private float projectileSpeed;
    [SerializeField, Tooltip("Don't add anything if no effect is needed")] private GameObject explosionEffect = null;

    private int collisionCount = 0;
    private Rigidbody rb;
    private float timer = 0;

    // Called when Instantiate() of prefab is used
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        KillAfterLifetime();
    }

    // EACH POSSIBLE ACTION OF BULLET IS A SHORT FUNCTION
    public void LookAtV3(Vector3 position)
    {
        transform.LookAt(position);
    }

    public void LookAtPlayer(GameObject player)
    {
        Vector3 targetPostition = new Vector3(player.transform.position.x,
                                       transform.position.y,
                                       player.transform.position.z);
        transform.LookAt(targetPostition);
    }

    public void ShootForward()
    {
        rb.velocity = transform.forward * projectileSpeed;
    }

    public void RotateAroundY(float angle)
    {
        transform.Rotate(Vector3.up, angle);
    }

    public IEnumerator SeekPlayer(GameObject player)
    {
        LookAtPlayer(player);
        while(true)
        {
            var heading = player.transform.position - transform.position;
            var rotation = Quaternion.LookRotation(heading);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, 500 * Time.deltaTime));  // hardcoded rotation speed - can be changed
            rb.velocity = transform.forward * projectileSpeed;
            yield return null;
        }
    }

    private void KillAfterLifetime()
    {
        if (projectileLifetime > 0)
        {
            timer += Time.deltaTime;

            if (timer > projectileLifetime || collisionCount == 8)
            {
                DestroyProjectile();
            } 
        }
    }

    public void DestroyProjectile()
    {
        if (explosionEffect != null)    // if there are explosion animations, run them separately and destroy them afterwards
        {
            ParticleSystem[] particleSystems = explosionEffect.GetComponents<ParticleSystem>();
            float delay = 0;
            foreach (ParticleSystem system in particleSystems)
            {
                if (system.main.duration > delay)
                {
                    delay = system.main.duration;
                }
            }

            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, delay);
            GameObject.Find("AudioManager")?.GetComponent<AudioManager>()?.PlaySound("explosion", 1.0f, 0.3f);
            
            StartCoroutine(GameObject.Find("Main Camera")?.GetComponent<CameraShake>()?.Shake(1.0f, 0.5f));
        }

        Destroy(this.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().TakeDamage(damage);
            DestroyProjectile();
        }
        else if(other.CompareTag("Untagged") || other.CompareTag("Wall"))
        {
            if (projectileType == ProjectileType.Bounce)
            {
                collisionCount++;
            }
            else
            {
                DestroyProjectile();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collider)
    {
        HandleCollision(collider.gameObject);
    }
}
