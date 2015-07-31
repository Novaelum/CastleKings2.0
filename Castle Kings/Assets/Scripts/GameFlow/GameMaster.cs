﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class GameMaster : MonoBehaviour {

    public const int MAX_PLAYERS = 8;

    public enum Teams
    {
        BLUE,
        RED
    }

    private const int GAME_DURATION = 180; // Game duration is 3 minutes
    private int m_gameTimer;

    public Player m_P1;
   // public Player p2;

	// Use this for initialization
	void Start () {
        m_gameTimer = 0;
        StartCoroutine(GameRoutine());
	}
	
	// Update is called once per frame
	void Update () {
        CheckControls();
	}

    void CheckControls()
    {
        // ========================
        // Right Trigger : Attacking
        // ------------------------
        if (InputManager.Devices[0].RightTrigger.IsPressed)
        {
            m_P1.Attack();
        }
        // ========================
        // Left Trigger
        // ------------------------
        if (InputManager.Devices[0].LeftTrigger.IsPressed)
        {
            Debug.Log("left trigger!" + InputManager.Devices[0].LeftTrigger.Value * -1);
        }
        // ========================
        // Left Stick : Movement
        // ------------------------
        if (InputManager.Devices[0].LeftStickX.IsPressed || InputManager.Devices[0].LeftStickY.IsPressed)
        {
            m_P1.Move(InputManager.Devices[0].LeftStickX.Value, InputManager.Devices[0].LeftStickY.Value);
        }
        if (InputManager.Devices[0].LeftStickY.WasReleased || InputManager.Devices[0].LeftStickX.WasReleased)
        {
            m_P1.StopMoving();
        }
        // ========================
        // D-PAD : Movement
        // ------------------------
        // Pressed
        if (InputManager.Devices[0].DPad.Up.IsPressed || InputManager.Devices[0].DPad.Right.IsPressed || InputManager.Devices[0].DPad.Down.IsPressed || InputManager.Devices[0].DPad.Left.IsPressed)
        {
            m_P1.Move(InputManager.Devices[0].DPad.X, InputManager.Devices[0].DPad.Y);
        }
        // Released
        if (InputManager.Devices[0].DPad.Up.WasReleased || InputManager.Devices[0].DPad.Right.WasReleased || InputManager.Devices[0].DPad.Down.WasReleased || InputManager.Devices[0].DPad.Left.WasReleased)
        {
            m_P1.StopMoving();
        }
        // ========================
        // Right Stick : Orientation
        // ------------------------
        if (InputManager.Devices[0].RightStickX.IsPressed || InputManager.Devices[0].RightStickY.IsPressed)
        {
            // Check to see to witch extend the joystick is pushed to prevent unwilling action when it is released (might go in the opposing direction before stopping in the middle)
            // Debug.Log("PosX" + InputManager.Devices[0].RightStickX.Value + " PosY " + InputManager.Devices[0].RightStickY.Value);
            m_P1.SetSide((Mathf.Atan2(InputManager.Devices[0].RightStickX.Value, InputManager.Devices[0].RightStickY.Value)) * 180 / Mathf.PI);
        }
        if (InputManager.Devices[0].GetControl(InputControlType.Back).WasPressed)
        {

        }


        // ========================
        // LeftBumper || Action Button 4 (Y): Use PowerUp
        // ------------------------
        if (InputManager.Devices[0].LeftBumper.WasPressed || InputManager.Devices[0].Action4.WasPressed)
        {
            m_P1.UsePowerUp();
        }


        // ========================
        // Action Button 1 (A) : Attacking
        // ------------------------
        if (InputManager.Devices[0].Action1.WasPressed)
        {
            if (InputManager.Devices[0].LeftStickX.IsPressed || InputManager.Devices[0].LeftStickY.IsPressed)
            {
                m_P1.Dash(InputManager.Devices[0].LeftStickX.Value, InputManager.Devices[0].LeftStickY.Value);
            }
            else if (InputManager.Devices[0].DPad.Up.IsPressed || InputManager.Devices[0].DPad.Right.IsPressed || InputManager.Devices[0].DPad.Down.IsPressed || InputManager.Devices[0].DPad.Left.IsPressed)
            {
                m_P1.Dash(InputManager.Devices[0].DPad.X, InputManager.Devices[0].DPad.Y);
            }
            else
            {
                m_P1.Dash(0, 0);
            }
           
        }

        // ========================
        // Action Button 3 (X) : Attacking
        // ------------------------
        if (InputManager.Devices[0].Action3.WasPressed)
        {
            m_P1.Attack();
        }
    }

    IEnumerator GameRoutine()
    {
        for (; m_gameTimer < GAME_DURATION; m_gameTimer += 5)
        {
            // Wait time before updating the current number of active enemies
            yield return new WaitForSeconds(5);
        }
        // Once the loop completes, the game ends
        GameDone();
        yield return null;
    }

    public void GameDone()
    {
        // TODO: Ending screen or whatev
    }
}
