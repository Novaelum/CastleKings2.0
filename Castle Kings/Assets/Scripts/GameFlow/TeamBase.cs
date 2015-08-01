using UnityEngine;
using System.Collections;

public class TeamBase : MonoBehaviour {

    // Score increase values
    const int PRINCESS_SAVED = 650;
    const int PLAYER_KILLED = 100;
    const int ENEMY_KILLED = 25;

    private int m_princessSaved;

    // Sound FX
    public AudioSource m_scoreFX;

    public GameMaster.Teams m_team;
    private int m_score;

    public int GetScore()           { return m_score; }
    public int GetPrincessSaved()   { return m_princessSaved; }

    
    
	// Use this for initialization
	void Start () {
        GameMaster.g_scoreDelegate.enemyKilled += OnEnemyKilled;
        GameMaster.g_scoreDelegate.playerKilled += OnPlayerKilled;
        m_princessSaved = 0;
	}

    void OnEnemyKilled(GameMaster.Teams p_attackerTeam)
    {
        if (p_attackerTeam == m_team)
        {
            m_score += ENEMY_KILLED;
            Debug.Log(m_team.ToString() + " team score : " + m_score);
        }
    }

    void OnPlayerKilled(GameMaster.Teams p_attackerTeam)
    {
        if (p_attackerTeam == m_team)
        {
            m_score += PLAYER_KILLED;
            Debug.Log(m_team.ToString() + " team score : " + m_score);
        }
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
                m_scoreFX.Play();
            }    
        }
    }
}
