using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Orc : Enemy, ISpawnable {

    struct TargetInfos
    {
        public Player target;
        public int priorityValue;
    };

    private int m_health;
    Sides m_currentSide;
    Sides m_lastSide;

    private TargetInfos[] m_targetsList = new TargetInfos[8];
    private Animator m_animator;
    private Collider2D m_collider;

    private float m_speed;
    private bool m_isAttacking;
    private bool m_hasAttacked;
    private bool m_isHealing;
    private float m_healthTimer;

    private const int HEALTH_COOLDOWN = 8;
    private const int DAMAGE_OUTPUT = 10;

	// Use this for initialization
	void Start () {
        m_healthTimer = 0;
        m_isHealing = true;
        m_hasAttacked = false;
        m_currentSide = Sides.FRONT;
        m_lastSide = Sides.FRONT;
        
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider2D>();


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
        if (m_currentSide != m_lastSide)
        {
            m_lastSide = m_currentSide;
            SetAnimations("BackWalk", "RightWalk", "FrontWalk", "LeftWalk");
        }

        if (!m_isAttacking)
        {
            //SetSide(Mathf.Atan(Mathf.Abs((m_targetsList[0].targetPos.position.y - transform.position.y) / (m_targetsList[0].targetPos.position.x - transform.position.x))) * 180 / Mathf.PI);
            SetSide(Mathf.Atan2((m_targetsList[0].target.transform.position.y - transform.position.y), (m_targetsList[0].target.transform.position.x - transform.position.x)) * 180 / Mathf.PI);
        }
        else
        {
            SetSide(Mathf.Atan2((m_targetsList[0].target.transform.position.y - transform.position.y), ((m_targetsList[0].target.transform.position.x - transform.position.x)) * 180 / Mathf.PI) * -1);
        }
        Move();

        if (m_isHealing)
        {
            m_health = m_health < 100 ? m_health += 4 : 100;
        }
        
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
        
        StartCoroutine(Cooldown(2f, "SelectTarget"));
    }

    private void SetPriorities()
    {
        for (int i = 0; m_targetsList[i].target != null; i++)
        {
            m_targetsList[i].priorityValue = HealthPriority(m_targetsList[i].target) + RangePriority(m_targetsList[i].target);
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
            return 1;
        }
        else if (tHealth < 70 && tHealth >= 55)
        {
            return 2;
        }
        else if (tHealth < 55 && tHealth >= 40)
        {
            return 3;
        }
        else if (tHealth < 40 && tHealth >= 25)
        {
            return 4;
        }
        else if (tHealth < 25 && tHealth >= 10)
        {
            return 5;
        }
        else
        {
            return 8;
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
            return 5;
        }
        else
        {
            return 6;
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

    override public void Damaged(int p_damage)
    {
        m_healthTimer = 0;
        if (m_isHealing) {
            m_isHealing = false;
            StartCoroutine(HealthTimer());
        }
            
      //  StartCoroutine(Attacked());
        m_health -= p_damage;
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (m_health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator Cooldown(float p_time, string p_TODO)
    {
        yield return new WaitForSeconds(p_time);
        SendMessage(p_TODO);
    }
    
    IEnumerator HealthTimer()
    {
        for (; m_healthTimer != HEALTH_COOLDOWN; m_healthTimer++)
        {
            yield return new WaitForSeconds(1);
        }
        m_isHealing = true;
        yield return null;
    }


    // Collision handling

    void OnTriggerStay2D(Collider2D objHit)
  {
      // If not a player, the orc will collide with it (trigger false)
      if (objHit.tag == "Player")
      {
          if (!m_hasAttacked)
          {
              m_hasAttacked = true;
              objHit.GetComponent<Player>().TakesDamage(DAMAGE_OUTPUT);
              StartCoroutine(Cooldown(1f, "AttackCDComplete"));
          }
            
          
      }
      else
      {
          m_collider.isTrigger = false;
      }
  }
  
  void OnCollisionEnter2D(Collision2D objHit)
  {
     // If objHit is a player, the orc will not collide with it (trigger true)
     if (objHit.gameObject.tag == "Player")
     {
         m_collider.isTrigger = true;
     }
  }
}
