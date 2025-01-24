using UnityEngine;

public class AmbientNoise : MonoBehaviour
{
    
    void Start()
    {
        GetComponent<AudioSource>().volume = AudioManager.Instance.sfxSource.volume * 0.5f;
    }
}
