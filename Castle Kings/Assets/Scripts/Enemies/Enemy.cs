using UnityEngine;
using System.Collections;

public abstract class Enemy : Character {

    protected struct TargetInfos
    {
        public Player target;
        public int priorityValue;
    };

    public abstract void Damaged(int p_damage);
    public abstract void Init(Vector3 p_pos);
}
