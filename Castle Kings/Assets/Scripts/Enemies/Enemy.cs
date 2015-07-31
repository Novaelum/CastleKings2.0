using UnityEngine;
using System.Collections;

public abstract class Enemy : Character {
    public abstract void Damaged(int p_damage);
    public abstract void Init(Vector3 p_pos);
}
