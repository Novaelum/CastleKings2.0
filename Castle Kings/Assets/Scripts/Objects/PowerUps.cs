using UnityEngine;
using System.Collections;

public class PowerUps : MonoBehaviour {

    public enum PowerUpsType
    {
        NONE,
        IRONSTORM,
        DOUBLE_SPEED,
        BOMBER_MAN,
        CASTLE_KING
    }

    private Animator m_animator;
    private PowerUpPlatform m_owner;
    private PowerUpsType m_type;

	// Initialization
	public void Init(int p_type, Vector3 p_pos, PowerUpPlatform p_owner) {
        Debug.Log(p_type);
        m_animator = GetComponent<Animator>();
        transform.position = p_pos;
        InitPower(p_type);
        m_owner = p_owner;
	}

    private void InitPower(int p_type) {
        if (p_type < 5) {
            m_type = PowerUpsType.IRONSTORM;
            m_animator.Play("PowerUp", -1, 0);
        } else if (p_type >=5 && p_type < 10) {
            m_type = PowerUpsType.BOMBER_MAN;
            m_animator.Play("PowerUp", -1, 0.3f);
        } else {
            m_type = PowerUpsType.DOUBLE_SPEED;
            m_animator.Play("PowerUp", -1, 0.5f);
        }
    }
	
	// Update is called once per frame
	void Update () {
	   
	}

    void OnTriggerEnter2D(Collider2D hitObj)
    {
        IPickUpper obj = hitObj.GetComponent<IPickUpper>();
        if (obj != null)
        {
            obj.OnPickUp(m_type);
            m_owner.SetTimer();
            Destroy(this.gameObject);
        }
    }
}
