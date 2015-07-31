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
        StartCoroutine(WaitForAndDo(Random.Range(6, 12), "SpawnPU"));
    }

    IEnumerator WaitForAndDo(float p_time, string p_TODO)
    {
        yield return new WaitForSeconds(p_time);
        SendMessage(p_TODO);
    }
}
