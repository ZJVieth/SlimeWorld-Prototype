using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Linked Components
    public Rigidbody2D m_rigidbody2d;
    public Animator m_animator;
    public CharStats m_charStats;

    // Sensor Properties
    public int m_moveDirectionPrecision = 16;
    public float m_surroundSensorDistance = 3f;
    public LayerMask m_surroundSensorLayers;

    // Heuristic Properties
    public float m_desiredRange = 1f;
    public float m_desiredRangeMargin = 0.25f;

    // States
    private int m_facing = 1;
    private float m_facingTimer;
    private const float FACING_DELAY = 0.5f;

    void Awake()
    {
        InitializeDirectionWeights();
    }

    // MOVEMENT FRAMEWORK --------------------------------------------------------------------
    private Dictionary<Vector2, float> m_directionWeights = new Dictionary<Vector2, float>();
    private void InitializeDirectionWeights()
    {
        for (float a = 0; a <= 2*Mathf.PI; a += 2*Mathf.PI/m_moveDirectionPrecision) {
            float x1 = Vector2.right.x;
            float y1 = Vector2.right.y;
            float x2 = Mathf.Cos(a)*x1 - Mathf.Sin(a)*y1;
            float y2 = Mathf.Sin(a)*x1 + Mathf.Cos(a)*y1;
            Vector2 direction = new Vector2(x2, y2);
            m_directionWeights[direction] = 0;
        }
    }
    public void ChangeNearestDirectionWeight(Vector2 targetDir, float changeWeight)
    {
        Vector2 closest = new Vector2(0f, 0f);
        foreach (Vector2 moveDir in m_directionWeights.Keys) {
            if (closest ==  new Vector2(0f, 0f)) {
                closest = moveDir;
            } else {
                if (Vector2.Angle(moveDir, targetDir) < Vector2.Angle(closest, targetDir)) {
                    closest = moveDir;
                }
            }
        }
        m_directionWeights[closest] += changeWeight;
    }
    public void ChangeSurroundingDirectionWeights(Vector2 targetDir, float changeWeight)
    {
        int count = 0; 
        float angleMargin = 360/m_moveDirectionPrecision;
        foreach (Vector2 moveDir in m_directionWeights.Keys) {
            if (Vector2.Angle(moveDir, targetDir) <= angleMargin) {
                m_directionWeights[moveDir] += changeWeight;
                count++;
            }
            if (count >= 2)
                return;
        }
    }
    public Vector2 RotatedBy(Vector2 input, float arad)
    {
        float x1 = input.x;
        float y1 = input.y;
        float x2 = Mathf.Cos(arad)*x1 - Mathf.Sin(arad)*y1;
        float y2 = Mathf.Sin(arad)*x1 + Mathf.Cos(arad)*y1;
        Vector2 output = new Vector2(x2, y2);
        return output;
    }
    // Debug draw directions
    void OnDrawGizmos()
    {
        //Debug.Log("Start");
        foreach (KeyValuePair<Vector2, float> dir in m_directionWeights) {
            float w = dir.Value;
            Gizmos.color = new Color(255-w*50, 0, w*50);
            Gizmos.DrawLine(transform.position, transform.position + (Mathf.Max(0.2f,0.25f*w)* new Vector3(dir.Key.x, dir.Key.y, 0f)));
        }
    }

    public delegate void MovementDelegate();
    private MovementDelegate movementHeuristics;
    public void ChangeHeuristics(MovementDelegate setHeuristics)
    {
        movementHeuristics = setHeuristics;
    }
    public void ExecuteMovement()
    {
        // Apply current movement heuristics to possible directions
        if (movementHeuristics != null) {
            InitializeDirectionWeights();
            movementHeuristics();
        }
        
        // Determine best direction
        KeyValuePair<Vector2, float> bestDir = m_directionWeights.First();
        foreach (KeyValuePair<Vector2, float> dir in m_directionWeights) {
            if (dir.Value > bestDir.Value) bestDir = dir;
        }
        Vector2 direction = bestDir.Key;

        // Move in best direction
        MoveInDirection(direction);
        //InitializeDirectionWeights();

        // Update facing direction
        UpdateFacing(direction);
    }    
    public void MoveInDirection(Vector2 direction)
    {
        direction.Normalize();
        m_rigidbody2d.MovePosition(
            m_rigidbody2d.position +
            direction * m_charStats.Speed()
            * Time.fixedDeltaTime
        );
    }

    // BEHAVIOUR FRAMEWORK -------------------------------------------------------------------
    public delegate void BehaviourDelegate();
    private BehaviourDelegate behaviour;
    public void ChangeBehaviour(BehaviourDelegate setBehaviour)
    {
        behaviour = setBehaviour;
    }
    public void ExecuteBehaviour()
    {
        behaviour();
    }

    // COMMON SENSORS -------------------------------------------------------------------------

    public Vector2 Sensor_PlayerDirection()
    {
        Vector2 output = PlayerController.instance.GetPosition() - m_rigidbody2d.position;
        output.Normalize();
        return output;
    }

    public float Sensor_PlayerDistance()
    {
        return Vector2.Distance(PlayerController.instance.GetPosition(), m_rigidbody2d.position);
    }

    public Dictionary<Vector2, float> Sensor_DirectionObstacleRays()
    {
        Dictionary<Vector2, float> output = new Dictionary<Vector2, float>();
        foreach (KeyValuePair<Vector2, float> dir in m_directionWeights) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.Key, m_surroundSensorDistance, m_surroundSensorLayers);
            if (hit.collider != null) {
                output[dir.Key] = hit.distance;
                //Debug.DrawRay(transform.position, dir.Key * hit.distance, Color.yellow);
            }
        }
        return output;
    }

    // UNUSED
    public Dictionary<GameObject, float> Sensor_Surrounding()
    {
        Dictionary<GameObject, float> output = new Dictionary<GameObject, float>();
        Collider2D[] surroundingObjects = Physics2D.OverlapCircleAll(transform.position, m_surroundSensorDistance, m_surroundSensorLayers); 
        foreach (Collider2D obj in surroundingObjects) {
            if (obj.gameObject.name == "Hitbox") {
                output[obj.gameObject.transform.parent.gameObject] = Vector2.Distance(obj.gameObject.transform.position, transform.position);
            } else {
                output[obj.gameObject] = Vector2.Distance(obj.gameObject.transform.position, transform.position);
            }
        }
        return output;
    }


    // Basic Active Methods
    public void UpdateFacing(Vector2 direction)
    {
        if (m_facingTimer <= 0) {
            int should_face = 0;
            if (direction.x >= 0) {
                should_face = -1;
            } else {
                should_face = 1;
            }

            if (should_face != m_facing) {
                m_facing = should_face;
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;

                m_facingTimer = FACING_DELAY;
            }
        } else {
            m_facingTimer -= Time.fixedDeltaTime;
        }

    }


}
