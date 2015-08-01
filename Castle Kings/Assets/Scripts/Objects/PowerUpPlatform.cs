using UnityEngine;
using System.Collections;

public class PowerUpPlatform : MonoBehaviour {

    public PowerUps m_powerUp;

	// Use this for initialization
	void Start () {
        SetTimer();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void SpawnPU() {
        GameObject powerUp = (GameObject)Object.Instantiate(m_powerUp.gameObject);
        powerUp.GetComponent<PowerUps>().Init(Random.Range(0, 15), transform.position, this);
    }

    public void SetTimer()
    {
        Invoke("SpawnPU", Random.Range(6, 12));
    }
}
