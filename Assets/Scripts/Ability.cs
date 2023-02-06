using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public string m_name = "New Ability";
    public string m_key_bind = "q";
    public float m_cooldown = 1.0f;
    public float m_duration = 1.0f;

    private float m_cooldown_timer;
    private float m_active_timer;

    // public abstract Ability Initialize();
    public abstract void OnTrigger();   // Custom immediate mechanic upon triggering
    public abstract void OnUpdate();    // Custom continuous mechanics
    public abstract void OnEnd();       // Custom mechanics that occur when the activation duration runs out
    public abstract void OnProjectileCollision(GameObject projectile, GameObject hitObject, Vector3 velocity);
    public abstract float Range();


    // Trigger ability used by the input handler
    public void Trigger()
    {
        if (!OnCooldown()) {
            Activate();
            OnTrigger();
        }
    }

    // Activates timers common to all abilities
    public void Activate()
    {
        m_active_timer = m_duration;
        m_cooldown_timer = m_cooldown;
    }

    // Loop that controls the activation duration and cooldown timers
    void FixedUpdate()
    {
        if (IsActive()) {
            m_active_timer -= Time.fixedDeltaTime;
            OnUpdate();

            if (!IsActive())
                OnEnd();
        }

        if (OnCooldown()) {
            m_cooldown_timer -= Time.fixedDeltaTime;
        }
    }

    // Timer checking methods
        public bool IsActive()
        {
            if (m_active_timer > 0.0)
                return true;
            return false;
        }
        public bool OnCooldown()
        {
            if (m_cooldown_timer > 0.0)
                return true;
            return false;
        }


    // Common Effects

        // Knockback effect
        public void Pushback(GameObject target, Vector2 direction, float force)
        {
            //GameObject enemy = hitbox.gameObject.transform.parent.gameObject;
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * force);
        }

    // Layer Checks
    
        public string GetLayer(GameObject obj)
        {
            return LayerMask.LayerToName(obj.layer);
        }

        public bool IsEnemyHitbox(GameObject obj) {
            if (obj.name == "HitBox" && GetLayer(obj) == "Enemy")
                return true;
            return false;
        }

        public bool IsEnemyPhysical(GameObject obj) {
            if (obj.name != "HitBox" && GetLayer(obj) == "Enemy")
                return true;
            return false;
        }

        public bool IsEnvironment(GameObject obj) {
            if (GetLayer(obj) == "Environment")
                return true;
            return false;
        }

}
