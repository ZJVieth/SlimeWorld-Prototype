using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallRat : EnemyController
{

    // STATES
    public int m_confidence = 80;
    public int m_rage = 20;

    private bool m_quad;

    private float m_circlingNoiseStep;

    void Start()
    {
        ChangeBehaviour(Behaviour_ConfidentSpar);
        ChangeHeuristics(HeuristicSet_ConfidentSpar);
    }

    // Event Based Behvaiour
    void OnDamaged()
    {
        Confidence(-5);
        Rage(+5);
    }

    // UPDATE LOOP -----------------------------------------------------------------------
    void FixedUpdate()
    {
        ExecuteMovement();
        ExecuteBehaviour();
    }

    // BEHAVIOUR: CONFIDENT SPAR -----------------------------------------------------------------
    private void Behaviour_ConfidentSpar()
    {
        if (m_quad) {
            ToBi();
        }
    }

    private void HeuristicSet_ConfidentSpar()
    {
        Heuristic_TowardsPlayer();
        Heuristic_AvoidObstacles();
    }

    // BEHAVIOURS ----------------------------------------------------------------------------

    private void Behaviour_RagingChase()
    {
        if (!m_quad) {
            ToQuad();
        }
    }

    private void Behaviour_Cowardice()
    {

    }

    // Basic Heuristics
    private void Heuristic_TowardsPlayer()
    {
        // Move towards player when far away
        ChangeNearestDirectionWeight(Sensor_PlayerDirection(), Mathf.Min(1.75f, Sensor_PlayerDistance()/2));

        // Circle around player when at desired range
        float circleHeuristic = 0.2f / Mathf.Abs(Sensor_PlayerDistance()-m_desiredRange);
        float noise = Mathf.PerlinNoise(m_circlingNoiseStep, 0f) - 0.5f;
        m_circlingNoiseStep += 0.01f;
        Debug.Log(noise);
        ChangeNearestDirectionWeight(RotatedBy(Sensor_PlayerDirection(), Mathf.PI/2), circleHeuristic+noise);
        ChangeNearestDirectionWeight(RotatedBy(Sensor_PlayerDirection(), -Mathf.PI/2), circleHeuristic-noise);

        // Move away from player when too close
        ChangeNearestDirectionWeight(RotatedBy(Sensor_PlayerDirection(), Mathf.PI), 0.6f/Mathf.Max(m_desiredRange, Mathf.Abs(Sensor_PlayerDistance()-m_desiredRange)));
    }

    private void Heuristic_AvoidObstacles()
    {
        foreach (KeyValuePair<Vector2, float> dir in Sensor_DirectionObstacleRays()) {
            //Debug.Log(dir.Value);

            // Make it less likely to move towards an obstacle
            ChangeNearestDirectionWeight(dir.Key, -1/dir.Value);

            // Try walking to one of the sides of the obstacle
            float circleHeuristic = 0.4f / dir.Value;
            float noise = Mathf.PerlinNoise(0f, m_circlingNoiseStep) - 0.5f;
            ChangeNearestDirectionWeight(RotatedBy(dir.Key, Mathf.PI/2), circleHeuristic+noise);
            ChangeNearestDirectionWeight(RotatedBy(dir.Key, Mathf.PI/2), circleHeuristic-noise);
        }
    }

    // Active Methods

    private int Confidence(int change = 0)
    {
        m_confidence += change;
        if (m_confidence >= 100) {
            m_confidence = 99;
        }
        else if (m_confidence <= 0) {
            m_confidence = 1;
        }
        return m_confidence;
    }

    private int Rage(int change = 0)
    {
        m_rage += change;
        if (m_rage >= 100) {
            m_rage = 99;
        }
        else if (m_rage <= 0) {
            m_rage = 1;
        }
        return m_rage;
    }

    private void ToBi()
    {
        m_animator.SetTrigger("ToBi");
        m_quad = false;
    }

    private void ToQuad()
    {
        m_animator.SetTrigger("ToQuad");
        m_quad = true;
    }

}
