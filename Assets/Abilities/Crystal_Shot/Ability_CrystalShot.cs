using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_CrystalShot : Ability
{

    public Transform m_attackPoint;
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

        // Set Up Attack Point for spawning a projectile
        Vector3 attack_direction = CamManager.instance.MouseDirectionFromWorld(character_obj.transform.position);

        m_attackPoint.position +=  attack_direction * m_spawn_range;

        m_attackPoint.rotation = CamManager.instance.RotationTowardsMouse(transform.position, m_attackPoint.rotation, 0f);

        // Create Slash Projectile
        Instantiate(m_projectilePrefab, m_attackPoint.position, m_attackPoint.rotation);

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
        return 5;
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