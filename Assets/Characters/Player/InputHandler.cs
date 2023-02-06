using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public PlayerController m_playerController;
    public AbilityHandler m_abilityHandler;

    private bool m_mouse_down_right;

    private float m_time_since_input;
    private int m_time_til_scratch;

    void Start()
    {
        SetTimeTilScratch();
    }

    // Update is called once per frame
    void Update()
    {
        // Right Mouse Down Movement
        if (Input.GetMouseButtonDown(1)) {
            m_mouse_down_right = true;
        }
        if (Input.GetMouseButtonUp(1)) {
            m_mouse_down_right = false;
        }

        if (m_mouse_down_right) {
            m_playerController.SetTargetPosition(
                CamManager.instance.mouse_position_world
            );
            m_time_since_input = 0;
        }

        // Ability Activations
        foreach (Ability ability in m_abilityHandler.GetAbilities()) {
            if (Input.GetKeyDown(ability.m_key_bind)) {
                ability.Trigger();
                m_time_since_input = 0;
            }
        }
    }

    void FixedUpdate()
    {
        m_time_since_input += Time.fixedDeltaTime;
        if (m_time_since_input >= m_time_til_scratch) {
            m_playerController.ScratchMaybe();
            m_time_since_input = 0;
            SetTimeTilScratch();
        }
    }

    private void SetTimeTilScratch()
    {
        m_time_til_scratch = 4 + Random.Range(4, 10);
    }
}
