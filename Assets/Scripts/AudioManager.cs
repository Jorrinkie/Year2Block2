using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------------Audio Source---------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("----------------Audio Clip---------------")]
    public AudioClip FireSFX;
    public AudioClip MusicBWF;
    public AudioClip MusicMinimalistic;

    private void Start()
    {
        musicSource.clip = MusicMinimalistic; // Set the first music clip
        musicSource.Play(); // Play the first music clip

        SFXSource.clip = FireSFX; // Set the SFX clip
        SFXSource.Play(); // Play the SFX clip
    }
}
