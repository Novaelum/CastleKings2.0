using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    static int g_enemyCount;

    public MonoBehaviour m_prefab;

	// Use this for initialization
	void Start () {
        GetComponent<SpriteRenderer>().enabled = false; // Make sure the spawner is invisible to the player
        SpawnEnemy();
	}

    private void SpawnEnemy()
    {
        if (g_enemyCount < 12)
        {
            GameObject spawned = (GameObject)Object.Instantiate(m_prefab.gameObject);
            spawned.GetComponent<Enemy>().Init(transform.position);
            g_enemyCount++;
        }
        StartCoroutine(SpawnerCooldown());
    }

    IEnumerator SpawnerCooldown()
    {
        yield return new WaitForSeconds(10f);
        SpawnEnemy();
    }


    // Count the number of enemies present on the map, called by GameMaster
    static public void CountEnemies()
    {
        g_enemyCount = 0;
        foreach (Enemy en in GameObject.FindObjectsOfType(typeof(Enemy)))
        {
            g_enemyCount++;
        }
    }
}
