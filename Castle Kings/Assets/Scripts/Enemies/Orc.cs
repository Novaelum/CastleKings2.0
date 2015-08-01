using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Orc : Enemy {

    private int m_health;
    Sides m_currentSide;
    Sides m_lastSide;

    // Components Refs
    private TargetInfos[] m_targetsList = new TargetInfos[GameMaster.MAX_PLAYERS];
    private Animator m_animator;

    // Sound FX
    public AudioSource m_hitFX;

    private float m_speed;
    private bool m_isAttacking;
    private bool m_hasAttacked;
    private bool m_isHealing;

    // Timers
    private const float TIMER_SELECTION = 5f;
    private const float TIMER_HEALTH = 8f;
    private const float TIMER_ATTACK = 2f;

    private float m_tarSelectTimer;
    private float m_healthTimer;
    private float m_attackTimer;
   
    private const int DAMAGE_OUTPUT = 10;

	// Use this for initialization
	void Start () {
        m_healthTimer = 0;
        m_isHealing = true;
        m_hasAttacked = false;
        m_currentSide = Sides.FRONT;
        m_lastSide = Sides.FRONT;
        
        m_animator = GetComponent<Animator>();

        m_health = 100;
        m_speed = 1.5f;
        InitTargetsList();
        SelectTarget();
	}

    override public void Init(Vector3 p_pos)
    {
        transform.position = p_pos;
    }
	
	// Update is called once per frame
	void Update () {
        SideUpdate();
        TimerUpdate();
        AttackUpdate();
        if (m_isHealing)
        {
            m_health = m_health < 100 ? m_health += 4 : 100;
        }
	}

    private void SideUpdate() {
        if (m_currentSide != m_lastSide)
        {
            m_lastSide = m_currentSide;
            SetAnimations("BackWalk", "RightWalk", "FrontWalk", "LeftWalk");
        }
    }

    // Updates all the timers
    private void TimerUpdate()
    {
        // Target Selection (loops)
        m_tarSelectTimer += Time.deltaTime;
        if (m_tarSelectTimer >= TIMER_SELECTION)
        {
            SelectTarget();
        }

        // Attack cooldown
        if (m_hasAttacked)
        {
            m_attackTimer += Time.deltaTime;
            if (m_attackTimer >= TIMER_ATTACK)
            {
                m_hasAttacked = false;
                m_attackTimer = 0;
            }
        }

        // Healing cooldown
        if (!m_isHealing)
        {
            m_healthTimer += Time.deltaTime;
            if (m_healthTimer >= TIMER_HEALTH)
            {
                m_isHealing = true;
                m_healthTimer = 0;
            }
        }
    }

    // Updates the sides of the orc and makes him move base on the current target (m_targetList[0].target)
    private void AttackUpdate()
    {
        if (m_isAttacking)
        {
            //SetSide(Mathf.Atan(Mathf.Abs((m_targetsList[0].targetPos.position.y - transform.position.y) / (m_targetsList[0].targetPos.position.x - transform.position.x))) * 180 / Mathf.PI);
            SetSide(Mathf.Atan2((m_targetsList[0].target.transform.position.y - transform.position.y), (m_targetsList[0].target.transform.position.x - transform.position.x)) * 180 / Mathf.PI);
        }
        else
        {
         //   SetSide(Mathf.Atan2((m_targetsList[0].target.transform.position.y - transform.position.y), ((m_targetsList[0].target.transform.position.x - transform.position.x)) * 180 / Mathf.PI) * -1);
        }
        Move();
    }

    public void SetSide(float p_angle)
    {
        if (p_angle > -45 && p_angle <= 45)
        {
            m_currentSide = Sides.RIGHT;
        }
        else if (p_angle > 45 && p_angle <= 135)
        {
            m_currentSide = Sides.BACK;
        }
        else if (p_angle > 135 || p_angle <= -135)
        {
            m_currentSide = Sides.LEFT;
        }
        else
        {
            m_currentSide = Sides.FRONT;
        }
    }

    // Init the corrent walking animation or idle (frame 0) base on the currentSide
    private void SetAnimations(string p_forBack, string p_forRight, string p_forFront, string p_forLeft)
    {
        switch (m_currentSide)
        {
            case Sides.BACK:
                m_animator.Play(p_forBack, -1, 0);
                break;

            case Sides.RIGHT:
                m_animator.Play(p_forRight, -1, 0);
                break;

            case Sides.FRONT:
                m_animator.Play(p_forFront, -1, 0);
                break;

            case Sides.LEFT:
                m_animator.Play(p_forLeft, -1, 0);
                break;
        }
    }

    // Fill m_targetsList with every player object present in the current scene (max 8)
    private void InitTargetsList()
    {
        int i = 0;
        foreach (Player p in GameObject.FindObjectsOfType(typeof(Player)))
        {
            m_targetsList[i].target = p;
            m_targetsList[i].priorityValue = 0;
            i++;
        }
    }

    private void SelectTarget()
    {
        SetPriorities();
        SortTargetsList();
        if (m_health > 30)
        {
            m_isAttacking = true;
        }
        else
        {
            m_isAttacking = false;
        }
    }

    private void SetPriorities()
    {
        for (int i = 0; m_targetsList[i].target != null; i++)
        {
            m_targetsList[i].priorityValue = (HealthPriority(m_targetsList[i].target) + RangePriority(m_targetsList[i].target) + CarriesPrincessPriority(m_targetsList[i].target));
        }
    }

    // Generate a number base on the target's health
    private int HealthPriority(Player p_cTarget) {
        int tHealth = p_cTarget.GetHealth();
        if (tHealth >= 85)
        {
            return 0;
        }
        else if (tHealth < 85 && tHealth >= 70)
        {
            return 2;
        }
        else if (tHealth < 70 && tHealth >= 55)
        {
            return 4;
        }
        else if (tHealth < 55 && tHealth >= 40)
        {
            return 6;
        }
        else if (tHealth < 40 && tHealth >= 25)
        {
            return 8;
        }
        else if (tHealth < 25 && tHealth >= 10)
        {
            return 10;
        }
        else
        {
            return 12;
        }
    }

    // Generate a number base on the target's range from self position
    private int RangePriority(Player p_cTarget)
    {
        float dist = Mathf.Sqrt((Mathf.Pow((p_cTarget.transform.position.x - transform.position.x), 2)) + (Mathf.Pow((p_cTarget.transform.position.y - transform.position.y), 2)));
        if (dist >= 30)
        {
            return 0;
        }
        else if (dist < 30 && dist >= 25)
        {
            return 1;
        }
        else if (dist < 25 && dist >= 20)
        {
            return 2;
        }
        else if (dist < 20 && dist >= 15)
        {
            return 3;
        }
        else if (dist < 15 && dist >= 10)
        {
            return 4;
        }
        else if (dist < 10 && dist >= 5)
        {
            return 10;
        }
        else
        {
            return 12;
        }
    }

    private int CarriesPrincessPriority(Player p_cTarget)
    {
        if (p_cTarget.IsCarryingPrincess())
        {
            return 15;
        }
        else
        {
            return 0;
        }
    }

    private void Move() 
    {
        Vector3 targetPos = m_targetsList[0].target.transform.position;
        if (m_isAttacking)
        {
            // Check if the target is farther than the specified range
            if (Mathf.Sqrt((Mathf.Pow((targetPos.x - transform.position.x), 2)) + (Mathf.Pow((targetPos.y - transform.position.y), 2))) > 0.5f)
                transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * m_speed);
        }
        else
            transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * m_speed) * -1;
    }

    void AttackCDComplete()
    {
        m_hasAttacked = false;
    }

    // Sort the target list by priorityValue (bigger number gets first)
    private void SortTargetsList() {
        for (int i = 0; m_targetsList[i].target != null; i++)
        {
            int j = i;
            while (j > 0 && m_targetsList[j - 1].priorityValue < m_targetsList[j].priorityValue)
            {
                TargetInfos temp = m_targetsList[j];
                m_targetsList[j] = m_targetsList[j - 1];
                m_targetsList[j - 1] = temp;
                j--;
            }
        }
    }

    override public void Damaged(int p_damage, GameMaster.Teams p_attackerTeam)
    {
        m_hitFX.Play();

        m_healthTimer = 0;
        if (m_isHealing) {
            m_isHealing = false;
        }
            
        StartCoroutine(Attacked());
        m_health -= p_damage;
        CheckDeath(p_attackerTeam);
    }

    private void CheckDeath(GameMaster.Teams p_attackerTeam)
    {
        if (m_health <= 0)
        {
            Enemy.Died(p_attackerTeam);
            Spawner.g_enemyCount--;
            GameObject.Destroy(this.gameObject);
        }
    }
    
    // Collision handling
  
  void OnCollisionEnter2D(Collision2D objHit)
  {
     // If objHit is a player, the orc will not collide with it (trigger true)
      if (objHit.gameObject.tag == "Player")
      {
          if (!m_hasAttacked)
          {
              m_hasAttacked = true;
              objHit.gameObject.GetComponent<Player>().TakesDamage(DAMAGE_OUTPUT);
          }
      }
  }
}
