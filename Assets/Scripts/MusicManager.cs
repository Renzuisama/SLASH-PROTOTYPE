using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Background Music Playlist (Non-final levels)")]
    [SerializeField] private AudioClip[] bgmTracks;

    [Header("Final Level Music")]
    [SerializeField] private AudioClip finalLevelBgm;
    [SerializeField] private bool loopFinalLevelBgm = true;

    [Header("Auto Final-Level Detection (Optional)")]
    [Tooltip("If enabled, the manager switches to Final Level BGM when the loaded scene buildIndex matches Final Level Build Index.")]
    [SerializeField] private bool autoDetectFinalLevelBySceneBuildIndex = false;
    [SerializeField] private int finalLevelBuildIndex = -1;

    [Header("General Settings")]
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool persistAcrossScenes = true;

    [Header("Fade Settings")]
    [SerializeField] private float fadeOutDuration = 1.5f;
    [SerializeField] private float fadeInDuration = 2f;

    private AudioSource audioSource;

    private int currentTrackIndex = 0;
    private bool inFinalMode = false;

    private Coroutine playlistCoroutine;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Basic singleton to avoid duplicates if you persist across scenes.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (persistAcrossScenes)
            DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false; // playlist handled manually; final can set loop true
        audioSource.volume = Mathf.Clamp01(volume);
    }

    private void OnEnable()
    {
        if (autoDetectFinalLevelBySceneBuildIndex)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (autoDetectFinalLevelBySceneBuildIndex)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!playOnStart) return;

        if (bgmTracks == null || bgmTracks.Length == 0)
        {
            Debug.LogWarning("MusicManager: No BGM tracks assigned!");
            return;
        }

        StartPlaylist();
    }

    // ----------------------------
    // Public API
    // ----------------------------

    /// <summary>
    /// Call this when you enter the final level.
    /// </summary>
    public void PlayFinalLevelBgm(bool withFade = true)
    {
        if (finalLevelBgm == null)
        {
            Debug.LogWarning("MusicManager: Final Level BGM is not assigned!");
            return;
        }

        inFinalMode = true;

        StopPlaylistRoutine();

        if (withFade)
        {
            StartFadeToClip(finalLevelBgm, loopFinalLevelBgm);
        }
        else
        {
            StopFadeRoutine();
            audioSource.Stop();
            audioSource.clip = finalLevelBgm;
            audioSource.loop = loopFinalLevelBgm;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Call this when you leave the final level (or restart back to normal levels).
    /// </summary>
    public void ResumePlaylist(bool restartFromFirstTrack = false, bool withFade = true)
    {
        if (bgmTracks == null || bgmTracks.Length == 0) return;

        inFinalMode = false;

        if (restartFromFirstTrack)
            currentTrackIndex = 0;

        StopPlaylistRoutine();

        if (withFade)
        {
            StartFadeToClip(bgmTracks[currentTrackIndex], false, thenStartPlaylist: true);
        }
        else
        {
            StopFadeRoutine();
            StartPlaylist();
        }
    }

    public void StopMusic()
    {
        StopFadeRoutine();
        StopPlaylistRoutine();
        audioSource.Stop();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }

    /// <summary>
    /// Plays a specific playlist track (non-final mode) immediately.
    /// </summary>
    public void PlaySpecificTrack(int trackIndex, bool withFade = true)
    {
        if (bgmTracks == null || bgmTracks.Length == 0) return;

        if (trackIndex < 0 || trackIndex >= bgmTracks.Length)
        {
            Debug.LogWarning($"MusicManager: Invalid track index: {trackIndex}");
            return;
        }

        inFinalMode = false;
        currentTrackIndex = trackIndex;

        StopPlaylistRoutine();

        if (withFade)
        {
            StartFadeToClip(bgmTracks[currentTrackIndex], false, thenStartPlaylist: true);
        }
        else
        {
            StopFadeRoutine();
            StartPlaylist();
        }
    }

    // ----------------------------
    // Internals
    // ----------------------------

    private void StartPlaylist()
    {
        if (bgmTracks == null || bgmTracks.Length == 0) return;

        StopFadeRoutine();
        StopPlaylistRoutine();

        audioSource.loop = false;
        playlistCoroutine = StartCoroutine(PlaylistLoop());
    }

    private IEnumerator PlaylistLoop()
    {
        while (!inFinalMode)
        {
            AudioClip clip = bgmTracks[currentTrackIndex];
            if (clip == null)
            {
                Debug.LogWarning($"MusicManager: BGM track {currentTrackIndex} is null. Skipping.");
                currentTrackIndex = (currentTrackIndex + 1) % bgmTracks.Length;
                continue;
            }

            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.volume = volume;
            audioSource.Play();

            // Wait until audio actually stops (unscaled, so pausing timeScale won't break it)
            yield return new WaitUntil(() => !audioSource.isPlaying || inFinalMode);

            if (inFinalMode) yield break;

            currentTrackIndex = (currentTrackIndex + 1) % bgmTracks.Length;
        }
    }

    private void StartFadeToClip(AudioClip newClip, bool loop, bool thenStartPlaylist = false)
    {
        StopFadeRoutine();
        fadeCoroutine = StartCoroutine(FadeToClipRoutine(newClip, loop, thenStartPlaylist));
    }

    private IEnumerator FadeToClipRoutine(AudioClip newClip, bool loop, bool thenStartPlaylist)
    {
        // Fade out (use unscaled time so it still fades even if Time.timeScale = 0)
        float startVol = audioSource.volume;
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, t / Mathf.Max(0.0001f, fadeOutDuration));
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();

        // Switch clip
        audioSource.clip = newClip;
        audioSource.loop = loop;
        audioSource.Play();

        // Fade in
        t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, volume, t / Mathf.Max(0.0001f, fadeInDuration));
            yield return null;
        }

        audioSource.volume = volume;
        fadeCoroutine = null;

        // If we faded to a playlist track, resume cycling from that index
        if (thenStartPlaylist && !inFinalMode)
        {
            StopPlaylistRoutine();
            playlistCoroutine = StartCoroutine(PlaylistLoop());
        }
    }

    private void StopPlaylistRoutine()
    {
        if (playlistCoroutine != null)
        {
            StopCoroutine(playlistCoroutine);
            playlistCoroutine = null;
        }
    }

    private void StopFadeRoutine()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!autoDetectFinalLevelBySceneBuildIndex) return;
        if (finalLevelBuildIndex < 0) return;

        if (scene.buildIndex == finalLevelBuildIndex)
        {
            PlayFinalLevelBgm(withFade: true);
        }
        else
        {
            // Only resume if we were in final mode; prevents restarting music on every scene load.
            if (inFinalMode)
                ResumePlaylist(restartFromFirstTrack: false, withFade: true);
        }
    }
}
