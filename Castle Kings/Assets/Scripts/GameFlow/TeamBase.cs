using UnityEngine;
using System.Collections;

public class TeamBase : MonoBehaviour {

    // Score increase values
    const int PRINCESS_SAVED = 650;
    const int PLAYER_KILLED = 100;
    const int ENEMY_KILLED = 25;

    const int VICTORY_COUNT = 3;

    private int m_princessSaved;

    public GameMaster.Teams m_team;

    [HideInInspector] public int m_score;

	// Use this for initialization
	void Start () {
        m_princessSaved = 0;
	}

    void OnTriggerEnter2D(Collider2D objHit)
    {
        if (objHit.gameObject.tag == "Player")
        {
            // Makes sure if the player is from it's team and that he is carrying the princess
            Player plHit = objHit.gameObject.GetComponent<Player>();
            if ((plHit.m_team == m_team) && plHit.IsCarryingPrincess()) {
                m_princessSaved++;
                m_score += PRINCESS_SAVED;
                Debug.Log(m_team.ToString() + " team score : " + m_score);
                plHit.SendMessage("Scored");
                Princess prin = (Princess)GameObject.FindObjectOfType(typeof(Princess));
                prin.Reset();
                CheckVictory();
            }    
        }
    }

    private void CheckVictory() {
        if (m_princessSaved == VICTORY_COUNT) {
            Debug.Log("Victory for " + m_team.ToString() + " team!");
        }
    }
	
}
