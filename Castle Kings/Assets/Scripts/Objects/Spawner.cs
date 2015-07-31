using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    static public int g_enemyCount = 0;

    public Enemy m_prefab;

    private float m_timerRange;
    private float m_timer;
    

	// Use this for initialization
	void Start () {
        m_timer = 0;
        m_timerRange = Random.Range(8, 20);
        GetComponent<SpriteRenderer>().enabled = false; // Makes sure the spawner is invisible to the player
	}

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_timerRange)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (g_enemyCount < 12)
        {
            GameObject spawned = (GameObject)Object.Instantiate(m_prefab.gameObject);
            spawned.GetComponent<Enemy>().Init(transform.position);
            g_enemyCount++;
        }
        m_timer = 0;
        m_timerRange = Random.Range(8, 20);
    }
}
