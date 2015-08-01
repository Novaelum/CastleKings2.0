using UnityEngine;
using System.Collections;
using InControl;

public class ScreenMaster : MonoBehaviour {

	// Use this for initialization
	void Start () {
       Localizater.GetTranslationFor("Victory");
	}
	
	// Update is called once per frame
	void Update () {
        if (InputManager.Devices[0].AnyButton.IsPressed)
        {
            AudioManager.CrossFade();
            Application.LoadLevel("LevelOne");
        }

        
	}
}
