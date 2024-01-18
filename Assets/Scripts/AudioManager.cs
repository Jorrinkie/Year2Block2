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

    [Header("----------------Settings---------------")]
    [SerializeField] bool playMusicMinimalistic = true; // Set to false if you want to play MusicBWF instead
    [SerializeField] bool playNoMusic = false; // Set to true if you don't want any music

    private void Start()
    {
        // Check if no music is selected
        if (!playNoMusic)
        {
            // Set up music loop based on the boolean variable
            if (playMusicMinimalistic)
            {
                musicSource.clip = MusicMinimalistic;
            }
            else
            {
                musicSource.clip = MusicBWF;
            }

            musicSource.loop = true;
            musicSource.Play();
        }

        // Set up SFX loop
        SFXSource.clip = FireSFX;
        SFXSource.loop = true;
        SFXSource.Play();
    }
}
