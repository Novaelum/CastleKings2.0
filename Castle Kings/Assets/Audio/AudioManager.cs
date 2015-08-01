using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour {

    static private AudioManager g_instance;

    public AudioMixerSnapshot m_snapshotA;
    public AudioMixerSnapshot m_snapshotB;
    private bool m_isA;
	
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
	void Start () {  
        if (g_instance == null) {
            m_isA = true;
            g_instance = this;
        }
        else
        {
            Debug.Log("AudioManager already existing... Destroying duplicate");
            GameObject.Destroy(this.gameObject);
        }
	}

    public static void CrossFade()
    {
        g_instance.ApplyCrossFade();
    }

    private void ApplyCrossFade()
    {
        m_isA = !m_isA;
        AudioMixerSnapshot snapshot = m_isA ? m_snapshotA : m_snapshotB;
        snapshot.TransitionTo(2f);
    }
}
