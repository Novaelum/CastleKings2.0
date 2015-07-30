using UnityEngine;
using System.Collections;


// Given to every class that can pickup object (ex. PowerUps)
public interface IPickUpper {
    void OnPickUp(PowerUps.PowerUpsType p_type);
}
