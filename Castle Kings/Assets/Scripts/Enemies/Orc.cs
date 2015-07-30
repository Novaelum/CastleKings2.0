using UnityEngine;
using System.Collections.Generic;

public class Orc : Character {

    struct TargetInfos
    {
        public Transform targetPos;
        public int priorityValue;
    };

    private int m_health;
    Sides m_currentSide;
    Sides m_lastSide;

    private TargetInfos[] m_targetsList = new TargetInfos[8];
    private Rigidbody2D m_rbody2D;
    private Animator m_animator;
    private float m_speed;
    private bool m_isAttacking;

	// Use this for initialization
	void Start () {
        m_currentSide = Sides.FRONT;
        m_lastSide = Sides.FRONT;
        m_rbody2D = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_health = 100;
        m_speed = 3f;
        InitTargetsList();
        SelectTarget();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_currentSide != m_lastSide)
        {
            m_lastSide = m_currentSide;
            SetAnimations("BackWalk", "RightWalk", "FrontWalk", "LeftWalk");
        }

        if (m_isAttacking)
        {
            //SetSide(Mathf.Atan(Mathf.Abs((m_targetsList[0].targetPos.position.y - transform.position.y) / (m_targetsList[0].targetPos.position.x - transform.position.x))) * 180 / Mathf.PI);
            SetSide(Mathf.Atan2((m_targetsList[0].targetPos.position.y - transform.position.y), (m_targetsList[0].targetPos.position.x - transform.position.x)) * 180 / Mathf.PI);
            AttackTarget();
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
            m_targetsList[i].targetPos = p.transform;
            m_targetsList[i].priorityValue = 0;
            i++;
        }
    }

    private void SelectTarget()
    {
        SetPriorities();
        SortTargetsList();
        m_isAttacking = true;
    }

    private void SetPriorities()
    {

    }

    // Generate a number base on the target's health
    private int HealthPriority(Player p_cTarget) {
        return 0;
    }

    // Generate a number base on the target's range from self position
    private int RangePriority(Player p_cTarget)
    {
        return 0;
    }



    private void AttackTarget() 
    {
        transform.position = Vector2.MoveTowards(transform.position, m_targetsList[0].targetPos.position, Time.deltaTime * m_speed);
    }

    // Sort the target list by priorityValue (bigger number gets first)
    private void SortTargetsList() {
        for (int i = 0; m_targetsList[i].targetPos != null; i++)
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
}
