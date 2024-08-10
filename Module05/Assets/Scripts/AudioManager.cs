using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static public AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip[] playlist;
    private AudioSource audioSource;
    private int currentSong = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = playlist[0];
            audioSource.Play();
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = playlist[++currentSong % playlist.Length];
            audioSource.Play();
        }
    }
}
