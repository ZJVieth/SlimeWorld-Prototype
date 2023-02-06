using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_SlimeJump : Ability
{

    public LayerMask m_enemyLayers;
    public ParticleSystem m_impactParticles;

    // Ability Attributes
    public float m_attackRange = 0.25f;
    public float m_blockPreJump = 0.5f;
    public float m_blockPostJump = 0.6f;
    public float m_pushForce = 25000f;

    // Execute all mechanics on trigger
    override public void OnTrigger() 
    {
        GameObject character_obj = PlayerController.instance.gameObject;

        // Make Character Face in right Direction
        PlayerController.instance.UpdateFacing(CamManager.instance.mouse_position_world);

        PlayerController.instance.m_animator.SetTrigger("Slime_Jump");

        PlayerController.instance.GetComponent<Collider2D>().enabled = false;

        PlayerController.instance.BlockMotion(m_blockPreJump);

    }

    // Execute terminal mechanics (jump impact)
    override public void OnEnd()
    {
        // Activate Particle System
        m_impactParticles.Emit(50);

        // Activate ScreenShake
        CamManager.instance.Shake();

        // Find hit enemies
        Collider2D[] enemies_hit = Physics2D.OverlapCircleAll(transform.position, m_attackRange, m_enemyLayers);

        // Apply Effect to hit enemies
        foreach (Collider2D enemy in enemies_hit) {
            GameObject enemyObj = enemy.gameObject;
            if (IsEnemyPhysical(enemyObj)) {

                // Calculate Damage
                CharStats ps = PlayerController.instance.GetStats();
                int damage = (int)((ps.GetProperty(Property.Agility) + ps.GetProperty(Property.Strength)) / 2);

                // Deal Damage
                enemyObj.GetComponent<CharStats>().DealDamage(damage);

                // Pushback Enemy
                Vector2 attack_direction = enemyObj.transform.position - transform.position;
                attack_direction.Normalize();
                Pushback(enemyObj, attack_direction, m_pushForce);
            }
        }

        // Reenable Collider upon landing
        PlayerController.instance.GetComponent<Collider2D>().enabled = true;

        // Ground player for a short time before being able to move again
        PlayerController.instance.BlockMotion(m_blockPostJump);
    }

    // Debug draw hit circle
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_attackRange);
    }


    // Unused ------------------------------------------------------------------------------------
    // This ability has no projectil
    override public void OnProjectileCollision(GameObject projectile, GameObject hitObject, Vector3 velocity)
    {}

    // This ability has no range
    override public float Range()
    {
        return 0f;
    } 

    // This ability has no continuous mechanic
    override public void OnUpdate() {}
}