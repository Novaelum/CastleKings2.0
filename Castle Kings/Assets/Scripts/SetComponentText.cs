using UnityEngine;
using UnityEngine.UI;

public class SetComponentText : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = Localizater.GetTranslationFor("StartMessage");
	}
}
