using UnityEngine;
[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public string name;

    [Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;

    public bool loop = false;
    public bool enableLP = false;

    [HideInInspector]
    public AudioSource source;

    [HideInInspector]
    public AudioLowPassFilter lpFilter;
}
