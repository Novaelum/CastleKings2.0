using UnityEngine;
using System.Collections;

public class Player : Character, IPickUpper {
    // Owner (base)
    public TeamBase m_owner;

    // Private const
    // Speed
    private const float SPEED_MAXIMUM = 0.1f;
    private const float SPEED_DEFAULT = 0.05f;
    private const float SPEED_MINIMUM = 0.025f;
    private float SPEED_DASH = 2f;
    [HideInInspector] public GameMaster.Teams m_team;

    // Stats
    private const int MAX_HEALTH = 100;
    private const int MAX_MANA = 100;

    // Manacost
    private const int MANACOST_DASH = 50;

    // Attack type
    private const int ATTACK_DEFAULT = 0;
    private const int ATTACK_BOMB = 1;
    private const int ATTACK_IRONSTORM = 2;

    // Components
    Rigidbody2D m_rbody2D;
    Animator m_animator;
    SpriteRenderer m_sprRen;
    ParticleSystem m_particles;

    // Sides
    Sides m_currentSide;
    Sides m_lastSide;

    // States
    private bool m_isMoving; // Whether or not movements keys are active
    private bool m_isAttacking;
    private bool m_isDashing;
    private bool m_carryPrincess;
    private bool m_isImmune;
    private bool m_isPowerUpActive;
    private bool m_isDead;

    // Variables
    private float m_speed;
    private int m_mana;
    private int m_health;
    private int m_attackType;
    PowerUps.PowerUpsType m_cPowerUp;
    [HideInInspector] public int m_ID;

    // Timer
    private float m_powerUpCD;
    private const float TIMER_IRONSTORM = 1f;
    private const float TIMER_BOMB = 1f;
    private const float TIMER_DOUBLE_SPEED = 5f;

    private const float TIMER_DEATH = 1f;

    private float m_powerUpTimer;
    private float m_deathTimer;

    // Getters
    public int GetHealth()              { return m_health; }
    public int GetAttackType()          { return m_attackType; }
    public bool IsImmune()              { return m_isImmune; }
    public bool IsCarryingPrincess()    { return m_carryPrincess; }

	// Use this for initialization
	void Start () {
        m_animator = GetComponent<Animator>();
        m_rbody2D = GetComponent<Rigidbody2D>();
        m_sprRen = GetComponent<SpriteRenderer>();
        m_particles = GetComponent<ParticleSystem>();
        m_team = m_owner.m_team;
        m_carryPrincess = false;

        Spawn();
	}

    private void Spawn()
    {
        if (m_carryPrincess)
        {
            m_carryPrincess = false;
            Princess prin = (Princess)GameObject.FindObjectOfType(typeof(Princess));
            prin.RespawnAt(transform.position);
        }

        m_isDead = false;
        m_deathTimer = 0;

        m_powerUpCD = 0;
        m_isMoving = false;
        m_powerUpTimer = 0;
        m_isAttacking = false;

        m_currentSide = Sides.FRONT;
        m_lastSide = Sides.FRONT;
        m_cPowerUp = PowerUps.PowerUpsType.NONE;

        m_particles.Stop();

        m_speed = SPEED_DEFAULT;
        m_health = MAX_HEALTH;
        m_mana = MAX_MANA;
        m_attackType = ATTACK_DEFAULT;

        Debug.Log("SpawnRoutine for " + m_team.ToString());
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        transform.position = m_owner.transform.position;
        m_isImmune = true;
        yield return new WaitForSeconds(3f);
        m_isImmune = false;
        yield return null;
    }
	
	// Update is called once per frame
	void Update () {
        CheckSideChange();
        if (m_isPowerUpActive)
            PowerUpActive();
        if (m_isDead)
            RespawnTimer();
	}

    private void CheckSideChange()
    {
        if (m_currentSide != m_lastSide)
        {
            m_lastSide = m_currentSide;
            if (!m_isAttacking)
            {
                SetAnimations("BackWalk", "RightWalk", "FrontWalk", "LeftWalk");
            }
            if (m_mana != 100)
                m_mana = m_mana < 100 ? m_mana += 3 : 100;
        }
    }

    private void PowerUpActive()
    {
        m_powerUpTimer += Time.deltaTime;
        if (m_powerUpTimer >= m_powerUpCD)
        {
            switch (m_cPowerUp)
            {
                // IronStorm
                case (PowerUps.PowerUpsType.IRONSTORM):
                    AttackEnded();
                    break;
                // Double Speed 
                case (PowerUps.PowerUpsType.DOUBLE_SPEED):
                    if (!m_carryPrincess)
                        m_speed = SPEED_DEFAULT;
                    else
                        m_speed = SPEED_MINIMUM;
                    break;
                // Bomber man
                case (PowerUps.PowerUpsType.BOMBER_MAN):
                    AttackEnded();
                    break;
                // Castle King
                //  case (PowerUps.PowerUpsType.CASTLE_KING):
                //      break;
            }
            m_cPowerUp = PowerUps.PowerUpsType.NONE;
            m_powerUpTimer = 0;
            m_attackType = ATTACK_DEFAULT;
            m_isPowerUpActive = false;
        }
    }

    private void RespawnTimer()
    {
        m_deathTimer += Time.deltaTime;
        if (m_deathTimer >= TIMER_DEATH)
            Spawn();
    }

    // Init the corrent walking animation or idle (frame 0) base on the currentSide
    private void SetAnimations(string p_forBack, string p_forRight, string p_forFront, string p_forLeft) {
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


    public void SetSide(float p_angle) {
        if (!m_isAttacking)
        {
            if (p_angle > -45 && p_angle <= 45)
            {
                m_currentSide = Sides.BACK;
            }
            else if (p_angle > 45 && p_angle <= 135)
            {
                m_currentSide = Sides.RIGHT;
            }
            else if (p_angle > 135 || p_angle <= -135)
            {
                m_currentSide = Sides.FRONT;
            }
            else
            {
                m_currentSide = Sides.LEFT;
            }
        }
    }

    public void Move(float p_X, float p_Y, float p_speed = 0) {
        if (!m_isMoving)
        {
            m_animator.speed = 1;
            m_isMoving = true;
        }

        if (!m_isDead)
        {
            Vector2 moveDirection = new Vector2(p_X, p_Y);
            if (p_speed == 0)
            {
                moveDirection *= m_speed;
                m_rbody2D.transform.Translate(moveDirection);
            }
            else
            {
                // TODO: Find a way to check for walls to avoid teleporting over
                m_mana -= MANACOST_DASH;
                m_isDashing = true;
                moveDirection *= p_speed;
                m_rbody2D.transform.Translate(moveDirection);
                Invoke("DashEnded", 0.1f);
            }
        }
    }

    public void StopMoving() {
        if (!m_isDead)
        {
            SetAnimations("BackWalk", "RightWalk", "FrontWalk", "LeftWalk"); // Reset animation to get back to the first animation frame (idle)
            m_animator.speed = 0;
            m_isMoving = false;
        }  
    }

    public void Attack() {
        if (!m_isAttacking && !m_isDead)
        {
            m_isAttacking = true;
            SetAnimations("BackAttack", "RightAttack", "FrontAttack", "LeftAttack");
            m_animator.speed = 1; // In case the player was in idle
            Invoke("AttackEnded", 0.4f);
        }
    }

    public void UsePowerUp()
    {
        Debug.Log(m_cPowerUp.ToString());
        if (m_cPowerUp != PowerUps.PowerUpsType.NONE && !m_isPowerUpActive)
        {
            m_isPowerUpActive = true;
            switch (m_cPowerUp)
            {
                case(PowerUps.PowerUpsType.IRONSTORM):
                    if (!m_isAttacking)
                        IronStorm();
                    break;
                case (PowerUps.PowerUpsType.DOUBLE_SPEED):
                    DoubleSpeed();
                    break;
                case (PowerUps.PowerUpsType.BOMBER_MAN):
                    if (!m_isAttacking)
                        BomberMan();
                    break;
              //  case (PowerUps.PowerUpsType.CASTLE_KING):
              //      CastleKing();
              //      break;
            }
        }
    }

    // Power Ups
    // IronStorm
    private void IronStorm()
    {
        m_powerUpCD = TIMER_IRONSTORM;
        m_attackType = ATTACK_IRONSTORM;
        m_animator.Play("IronStorm", -1, 0);
        m_isAttacking = true;
    }

    // Double Speed
    private void DoubleSpeed()
    {
        m_powerUpCD = TIMER_DOUBLE_SPEED;
        if (!m_carryPrincess)
            m_speed = SPEED_MAXIMUM;
        else
            m_speed = SPEED_DEFAULT;
    }

    // BomberMan
    private void BomberMan()
    {
        m_powerUpCD = TIMER_BOMB;
        m_attackType = ATTACK_BOMB;
        m_animator.Play("Bomb", -1, 0);
    }

    // CastleKing
    // private void CastleKing()
    // {
    // 
    // }

    public void Dash(float p_X, float p_Y)
    {
        if (m_mana >= MANACOST_DASH && !m_isDashing && !m_isAttacking)
        {
            // Check if the player is moving or not
            // p_X && p_Y != 0 (Player is moving)
            if (p_X > 0 && p_Y > 0)
            {
                Move(p_X, p_Y, SPEED_DASH);
            }
            // p_X && p_Y == 0 (Player not moving)
            else
            {
                // Determine the direction to dash from the player orientation (currentSide)
                switch (m_currentSide)
                {
                    case Sides.BACK:
                        Move(0, 1, SPEED_DASH);
                        break;

                    case Sides.RIGHT:
                        Move(1, 0, SPEED_DASH);
                        break;

                    case Sides.FRONT:
                        Move(0, -1, SPEED_DASH);
                        break;

                    case Sides.LEFT:
                        Move(-1, 0, SPEED_DASH);
                        break;
                }
            }
        }
        Debug.Log(m_mana);
    }

    private void AttackEnded()
    {
        m_isAttacking = false;
        if (!m_isMoving)
            m_animator.speed = 0;
    }

    void Killed()
    {
        if (!m_isImmune)
        {
            m_isImmune = true;
            m_isDead = true;
            m_animator.Play("Death", -1, 0);
            if (m_carryPrincess)
                m_particles.Stop();
        }
    }

    public void TakesDamage(int p_damage)
    {
        if (!m_isImmune)
        {
            m_health -= p_damage;
            StartCoroutine(Attacked());
           // Debug.Log("Hey!" + m_health);
            if (m_health <= 0)
            {
                Killed();
            }
        }
    }

    void TakePrincess()
    {
        m_carryPrincess = true;
        m_speed = SPEED_MINIMUM;
        Debug.Log("I shall bring you to safety my dear!");
        m_particles.Play();
    }

    void Scored()
    {
        m_particles.Stop();
        m_carryPrincess = false;
        m_speed = SPEED_DEFAULT;
    }

    private void DashEnded()
    {
        m_isDashing = false;
        m_rbody2D.velocity = Vector3.zero;
        m_rbody2D.angularVelocity = 0;
        Attack();
    }

    // Interface call
    public void OnPickUp(PowerUps.PowerUpsType p_type)
    {
        m_cPowerUp = p_type;
        Debug.Log(m_cPowerUp.ToString());
    }

    void OnEnterCollision2D(Collision2D objHit)
    {
        Debug.Log((objHit.gameObject.tag.ToString()));
    }
}
