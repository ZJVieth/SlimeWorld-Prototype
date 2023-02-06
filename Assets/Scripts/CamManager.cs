using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    public Camera m_camera;
    public Animator m_animator;

    public static CamManager instance;

    public Vector3 mouse_position_world;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else {
            Destroy(this);
        }
    }

    void Update()
    {
        mouse_position_world = m_camera.ScreenToWorldPoint(Input.mousePosition);
    }

    public Vector3 WorldToScreenPoint(Vector2 input)
    {
        return m_camera.WorldToScreenPoint(input);
    }

    public float pixelHeight()
    {
        return m_camera.pixelHeight;
    }

    public Vector3 MouseDirectionFromWorld(Vector2 obj_position)
    {
        Vector2 direction = new Vector2(mouse_position_world.x, mouse_position_world.y) - obj_position;
        direction.Normalize();
        Vector3 output = new Vector3(direction.x, direction.y, 0f);
        return output;
    }

    public Quaternion RotationTowardsMouse(Vector2 input_position, Quaternion obj_rotation, float angle_offset)
    {
        Vector2 mouse = Input.mousePosition;
        Vector2 obj_position = CamManager.instance.WorldToScreenPoint(input_position);

        //Vector2 target = new Vector2(mouse.x, obj_position.y + 10);

        Vector2 direction = mouse - obj_position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angle_offset;
        //angle = Math.Max(-90, angle);
        //angle = Math.Min(0, angle);

        //Quaternion target = Quaternion.Euler(new Vector3(0, 0, angle));
        //return Quaternion.Slerp(obj_rotation, target, 0.1f);

        return Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void Shake()
    {
        m_animator.SetTrigger("Shake");
    }
}
