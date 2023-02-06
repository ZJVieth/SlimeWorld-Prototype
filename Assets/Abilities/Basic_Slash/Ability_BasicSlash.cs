using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_BasicSlash : Ability
{
    //public PlayerController m_playerController;
    //public Animator m_slotAnimator;
    //public Animator m_abilityAnimator;
    public Transform m_attackPoint;
    public LayerMask m_enemyLayers;
    public GameObject m_projectilePrefab;

    // Ability Attributes
    public float m_spawn_range = 0.5f;

    // Execute all mechanics on trigger
    override public void OnTrigger() 
    {
        GameObject character_obj = PlayerController.instance.gameObject;

        // Reset Attack Point in case cooldown is shorter than activation duration
        m_attackPoint.position = character_obj.transform.position;

        // Make Character Face in right Direction
        PlayerController.instance.UpdateFacing(CamManager.instance.mouse_position_world);

        // Animation
        //SlotRenderer.instance.PivotAnimator().SetTrigger("Slash");

        // Set Up Attack Point for spawning a projectile
        Vector3 attack_direction = CamManager.instance.MouseDirectionFromWorld(character_obj.transform.position);

        m_attackPoint.position +=  attack_direction * m_spawn_range;

        m_attackPoint.rotation = CamManager.instance.RotationTowardsMouse(transform.position, m_attackPoint.rotation, 0f);

        // Create Slash Projectile
        Instantiate(m_projectilePrefab, m_attackPoint.position, m_attackPoint.rotation);

        // Find hit enemies
        // Collider2D[] enemies_hit = Physics2D.OverlapCircleAll(m_attackPoint.position, m_attack_radius, m_enemyLayers);

        // Apply Effect to hit enemies
        // foreach (Collider2D enemy in enemies_hit) {
        //     if (enemy.name.Equals("HitBox")) {
        //         Pushback(enemy, attack_direction, 1000f);
        //         Debug.Log("We hit " + enemy.name);
        //     }
        // }
    }

    override public void OnProjectileCollision(GameObject projectile, GameObject hitObject, Vector3 velocity)
    {
        if (IsEnemyHitbox(hitObject) || IsEnvironment(hitObject))
        {
            projectile.GetComponent<Projectile>().ImpactParticles();
             projectile.GetComponent<Projectile>().DestroyAfterParticle();
        }
        
        if (IsEnemyHitbox(hitObject)) {
            Vector3 direction = velocity;
            direction.Normalize();
            GameObject enemy = hitObject.transform.parent.gameObject;
            Pushback(enemy, direction, 2000f);
        }
    }

    override public float Range()
    {
        return Inventory.instance.GetActiveSlot().m_range;
    }  

    // This ability has no continuous mechanic
    override public void OnUpdate() {}

    // This ability has no terminal mechanic
    override public void OnEnd() {}

    // Debug draw hit circle
    void OnDrawGizmosSelected()
    {
        if (m_attackPoint != null)
            Gizmos.DrawWireSphere(m_attackPoint.position, 0.5f);
    }
}
