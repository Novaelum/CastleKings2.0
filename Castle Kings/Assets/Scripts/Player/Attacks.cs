using UnityEngine;
using System.Collections;

public class Attacks : MonoBehaviour {

    public Player m_Owner;

    // Sound FX
    public AudioSource m_attackFX;
    public AudioSource m_bombFX;

    private const int DEFAULT_DAMAGE = 50;
    private const int IRONSTORM_DAMAGE = 100;
    private const int BOMB_DAMAGE = 150;

	// Use this for initialization
	void Start () {
       
	}

    void OnTriggerEnter2D(Collider2D objHit) {
        int attackType = m_Owner.GetAttackType();

        // ==== Type Chart ==== //
        // 0 = Default          //
        // 1 = Bomb             //
        // 2 = IronStorm        //
        // -------------------- //
        switch (attackType)
        {
            case(0):
                m_attackFX.Play();
                Attack(objHit, DEFAULT_DAMAGE);
                break;
            case(1):
                m_bombFX.Play();
                Attack(objHit, BOMB_DAMAGE);
                break;
            case(2):
                m_attackFX.Play();
                Attack(objHit, IRONSTORM_DAMAGE);
                break;
        }
        
    }

    void Attack(Collider2D p_objHit, int p_damage)
    {
        
        if (p_objHit.gameObject.tag == "Player")
        {
            // Player to player damage always one shot kill
            p_objHit.gameObject.SendMessage("Killed");
        }
        else if (p_objHit.gameObject.tag == "Enemy")
        {
            p_objHit.GetComponent<Enemy>().Damaged(p_damage, m_Owner.m_team);
        }
    }
}
