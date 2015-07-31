using UnityEngine;
using System.Collections;

public class KillScore {

    public delegate void KillScoreDelegate(GameMaster.Teams p_attackerTeam);
    public KillScoreDelegate enemyKilled;
    public KillScoreDelegate playerKilled;

    public void EnemyKilled(GameMaster.Teams p_attackerTeam)
    {
        if (enemyKilled != null)
            enemyKilled(p_attackerTeam);
    }

    public void PlayerKilled(GameMaster.Teams p_attackerTeam)
    {
        if (playerKilled != null)
            playerKilled(p_attackerTeam);
    }
}
