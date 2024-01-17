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
        // Set up music loop
        musicSource.clip = MusicMinimalistic;
        musicSource.loop = true;
        musicSource.Play();

        // Set up SFX loop
        SFXSource.clip = FireSFX;
        SFXSource.loop = true;
        SFXSource.Play();
    }
}
