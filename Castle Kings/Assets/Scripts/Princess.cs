using UnityEngine;
using System.Collections;

public class Princess : MonoBehaviour {

    private Vector3 m_initialPos;

	// Use this for initialization
	void Start () {
        m_initialPos = transform.position;
	}

    void OnTriggerEnter2D(Collider2D objHit)
    {
        if (objHit.gameObject.tag == "Player")
        {
            objHit.gameObject.SendMessage("TakePrincess");
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void Reset()
    {
        RespawnAt(m_initialPos);
    }

    public void RespawnAt(Vector3 p_pos)
    {
        transform.position = p_pos;
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
