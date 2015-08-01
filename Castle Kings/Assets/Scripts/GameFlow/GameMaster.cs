using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class GameMaster : MonoBehaviour {
    public static KillScore g_scoreDelegate = new KillScore();

    public const int MAX_PLAYERS = 8;
    const int VICTORY_COUNT = 3;

    public enum Teams
    {
        BLUE,
        RED
    }

    private const int GAME_DURATION = 180; // Game duration is 3 minutes
    private int m_gameTimer;

    public Player m_P1;
    public Player m_P2;

    public TeamBase team1;
    public TeamBase team2;

	// Use this for initialization
	void Start () {
        m_gameTimer = 0;
        StartCoroutine(GameRoutine());
	}
	
	// Update is called once per frame
	void Update () {
        CheckControls();
        CheckVictory();
	}

    void CheckControls()
    {
        InputManagement(0, m_P1);
        if (InputManager.Devices.Count > 1)
            InputManagement(1, m_P2);
    }

    private void InputManagement(int p_device, Player p_player) {
        // ========================
        // Right Trigger : Attacking
        // ------------------------
        if (InputManager.Devices[p_device].RightTrigger.IsPressed)
        {
            p_player.Attack();
        }
        // ========================
        // Left Trigger
        // ------------------------
        if (InputManager.Devices[p_device].LeftTrigger.IsPressed)
        {
            Debug.Log("left trigger!" + InputManager.Devices[p_device].LeftTrigger.Value * -1);
        }
        // ========================
        // Left Stick : Movement
        // ------------------------
        if (InputManager.Devices[p_device].LeftStickX.IsPressed || InputManager.Devices[p_device].LeftStickY.IsPressed)
        {
            p_player.Move(InputManager.Devices[p_device].LeftStickX.Value, InputManager.Devices[p_device].LeftStickY.Value);
        }
        if (InputManager.Devices[p_device].LeftStickY.WasReleased || InputManager.Devices[p_device].LeftStickX.WasReleased)
        {
            p_player.StopMoving();
        }
        // ========================
        // D-PAD : Movement
        // ------------------------
        // Pressed
        if (InputManager.Devices[p_device].DPad.Up.IsPressed || InputManager.Devices[p_device].DPad.Right.IsPressed || InputManager.Devices[p_device].DPad.Down.IsPressed || InputManager.Devices[p_device].DPad.Left.IsPressed)
        {
            p_player.Move(InputManager.Devices[p_device].DPad.X, InputManager.Devices[p_device].DPad.Y);
        }
        // Released
        if (InputManager.Devices[p_device].DPad.Up.WasReleased || InputManager.Devices[p_device].DPad.Right.WasReleased || InputManager.Devices[p_device].DPad.Down.WasReleased || InputManager.Devices[p_device].DPad.Left.WasReleased)
        {
            p_player.StopMoving();
        }
        // ========================
        // Right Stick : Orientation
        // ------------------------
        if (InputManager.Devices[p_device].RightStickX.IsPressed || InputManager.Devices[p_device].RightStickY.IsPressed)
        {
            // Check to see to witch extend the joystick is pushed to prevent unwilling action when it is released (might go in the opposing direction before stopping in the middle)
            // Debug.Log("PosX" + InputManager.Devices[p_device].RightStickX.Value + " PosY " + InputManager.Devices[p_device].RightStickY.Value);
            p_player.SetSide((Mathf.Atan2(InputManager.Devices[p_device].RightStickX.Value, InputManager.Devices[p_device].RightStickY.Value)) * 180 / Mathf.PI);
        }

        // ========================
        // Back
        // ------------------------
        if (InputManager.Devices[p_device].GetControl(InputControlType.Back).WasPressed)
        {
            
        }


        // ========================
        // LeftBumper || Action Button 4 (Y): Use PowerUp
        // ------------------------
        if (InputManager.Devices[p_device].LeftBumper.WasPressed || InputManager.Devices[p_device].Action4.WasPressed)
        {
            p_player.UsePowerUp();
        }


        // ========================
        // Action Button 1 (A) : Attacking
        // ------------------------
        if (InputManager.Devices[p_device].Action1.WasPressed)
        {
            if (InputManager.Devices[p_device].LeftStickX.IsPressed || InputManager.Devices[p_device].LeftStickY.IsPressed)
            {
                p_player.Dash(InputManager.Devices[p_device].LeftStickX.Value, InputManager.Devices[p_device].LeftStickY.Value);
            }
            else if (InputManager.Devices[p_device].DPad.Up.IsPressed || InputManager.Devices[p_device].DPad.Right.IsPressed || InputManager.Devices[p_device].DPad.Down.IsPressed || InputManager.Devices[p_device].DPad.Left.IsPressed)
            {
                p_player.Dash(InputManager.Devices[p_device].DPad.X, InputManager.Devices[p_device].DPad.Y);
            }
            else
            {
                p_player.Dash(0, 0);
            }

        }

        // ========================
        // Action Button 3 (X) : Attacking
        // ------------------------
        if (InputManager.Devices[p_device].Action3.WasPressed)
        {
            p_player.Attack();
        }
    }

    private void CheckVictory()
    {
        // TODO: Victory screen

        if (team1.GetPrincessSaved() == VICTORY_COUNT || team2.GetPrincessSaved() == VICTORY_COUNT)
        {
            string victoryMessage = (team1.GetScore() > team2.GetScore()) ? (Localizater.GetTranslationFor("Victory") + team1.m_team.ToString() + Localizater.GetTranslationFor("TeamWith") + team1.GetScore() + Localizater.GetTranslationFor("points"))
                                                                          : (Localizater.GetTranslationFor("Victory") + team2.m_team.ToString() + Localizater.GetTranslationFor("TeamWith") + team2.GetScore() + Localizater.GetTranslationFor("points"));
            Debug.Log(victoryMessage);
            Application.LoadLevel(Application.loadedLevel);
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
