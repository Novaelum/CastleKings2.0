using UnityEngine;
using System.Collections;

public abstract class Enemy : Character {

    protected struct TargetInfos
    {
        public Player target;
        public int priorityValue;
    };

    // Called when one of Enemy childs dies
    protected static void Died(GameMaster.Teams p_attackerTeam) {
        GameMaster.g_scoreDelegate.EnemyKilled(p_attackerTeam);
    }

    public abstract void Damaged(int p_damage, GameMaster.Teams p_attackerTeam);
    public abstract void Init(Vector3 p_pos);
}
