using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
    protected enum Sides
    {
        BACK,
        RIGHT,
        FRONT,
        LEFT
    };

    protected IEnumerator Attacked()
    {
        SpriteRenderer sprRen = GetComponent<SpriteRenderer>();
        sprRen.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        sprRen.color = Color.white;
    }
}
