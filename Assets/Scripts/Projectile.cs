using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float m_speed = 20f;
    public SpriteRenderer m_spriteRenderer;
    public Collider2D m_collider2d;
    public Rigidbody2D m_rigidbody2d;
    public Ability m_ability;
    public ParticleSystem m_impactParticleSystem;

    private Vector3 m_position_start;

    private bool m_destroy_after_particle;

    // Start is called before the first frame update
    void Start()
    {
        m_position_start = m_rigidbody2d.position;
        m_rigidbody2d.velocity = transform.right * m_speed;
    }

    // Per Frame Mechanic
    void FixedUpdate()
    {
        if (Vector3.Distance(m_rigidbody2d.position, m_position_start) >= m_ability.Range()) {
            DestroyAfterParticle();
        }

        if (m_destroy_after_particle && !m_impactParticleSystem.IsAlive())
            Destroy(gameObject);
    }

    public void DisableProjectile()
    {
        m_spriteRenderer.enabled = false;
        m_collider2d.enabled = false;
    }

    public void DestroyAfterParticle()
    {
        m_destroy_after_particle = true;
        DisableProjectile();
    }

    void OnTriggerEnter2D(Collider2D hitBox)
    {
        m_ability.OnProjectileCollision(gameObject, hitBox.gameObject, m_rigidbody2d.velocity);
    }

    public void ImpactParticles()
    {
        m_impactParticleSystem.Play();
    }
}
