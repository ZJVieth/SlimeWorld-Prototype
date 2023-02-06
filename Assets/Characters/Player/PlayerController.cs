using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Static instantiation
    public static PlayerController instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else {
            Destroy(this);
        }
    }

    // Constants
    public const float TARGET_MARGIN = 0.1f;

    // Linked Components
    public Rigidbody2D m_rigidbody2d;
    public Animator m_animator;
    public CharStats m_charStats;
    public ParticleSystem m_particleSystem;

    // Variables
    private Vector2 m_target_position;
    private bool m_moving;
    private Vector2 m_previous_position;

    private int m_facing = 1;

    private float m_time_of_motion_block;

    // Controller Loop ---------------------------------------------------------------------------
    void FixedUpdate()
    {
        if (m_moving) {
            MoveTowardsTarget();

            if (IsStuck()) {
                StopMoving();
            }
            m_previous_position = m_rigidbody2d.position;
        }

        if (ReachedTargetPosition()) {
            StopMoving();
        }

        m_time_of_motion_block -= Time.fixedDeltaTime;
    }
    // -------------------------------------------------------------------------------------------------

    private void StopMoving()
    {
        m_moving = false;
        m_animator.SetBool("Moving", false);
        m_particleSystem.Stop();
    }

    private void MoveTowardsTarget()
    {
        if (m_time_of_motion_block <= 0)
        {
            // Calculate direction to move in
            Vector2 direction = m_target_position - m_rigidbody2d.position;
            direction.Normalize();

            // Execute movement on rigidbody
            m_rigidbody2d.MovePosition(
                m_rigidbody2d.position + 
                direction * m_charStats.Speed()
                * Time.fixedDeltaTime
            );
        }
    }

    public void SetTargetPosition(Vector2 target)
    {
        m_target_position = target;

        // Only update moving and facing if the target location is far enough away from the current location
        if (!ReachedTargetPosition()) {
            m_moving = true;
            m_animator.SetBool("Moving", true);
            m_particleSystem.Play();

            UpdateFacing(m_target_position);
        }
    }

    private bool ReachedTargetPosition()
    {
        if (Vector2.Distance(m_rigidbody2d.position, m_target_position) <= TARGET_MARGIN)
            return true;
        return false;
    }

    private bool IsStuck()
    {
        // Check if the character is barely moving forward (running into a wall for example)
        if (Vector2.Distance(m_rigidbody2d.position, m_previous_position) <= 0.5*TARGET_MARGIN)
            return true;
        return false;
    }

    public void UpdateFacing(Vector2 target)
    {
        int should_face = 0;
        if (target.x - m_rigidbody2d.position.x >= 0) {
            should_face = -1;
        } else {
            should_face = 1;
        }

        if (should_face != m_facing) {
            m_facing = should_face;
            //transform.Rotate(0f, 180f, 0f);
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    public int Facing()
    {
        return m_facing;
    }

    public void ScratchMaybe()
    {
        if (m_animator.GetBool("Moving") == false) {
            m_animator.SetTrigger("Scratch");
        }
    }

    public void BlockMotion(float set)
    {
        m_time_of_motion_block = set;
    }

    public CharStats GetStats()
    {
        return GetComponent<CharStats>();
    }

    public Vector2 GetPosition()
    {
        return m_rigidbody2d.position;
    }
}
