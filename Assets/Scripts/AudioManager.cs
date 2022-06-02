using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip startSlowMotionSound;
    [SerializeField] private AudioClip endSlowMotionSound;
    [SerializeField] private AudioClip sliceSound, bloodSound, shootSound, hurtSound, explosionSound, fireworkSound;

    private Dictionary<int, float> AudioSoucePitches = new Dictionary<int, float>();
    // Start is called before the first frame update
    void Update()
    {
        var audioSources = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < audioSources.Length; i++)
        {
            var id = audioSources[i].GetInstanceID();
            if (!AudioSoucePitches.ContainsKey(id))
            {
                AudioSoucePitches.Add(id, audioSources[i].pitch);
            }

            var originalPitch = AudioSoucePitches[id];
            if (Time.timeScale != 1)
            {
                audioSources[i].pitch = originalPitch - 0.3f;
            } else
            {
                audioSources[i].pitch = originalPitch;
            }
        }
    }

    private IEnumerator ExecuteSound(AudioClip sound, float pitch, float volume)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(sound);
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        yield return new WaitForSecondsRealtime(sound.length);
        if (AudioSoucePitches.ContainsKey(audioSource.GetInstanceID()))
        {
            AudioSoucePitches.Remove(audioSource.GetInstanceID());
        }

        Destroy(audioSource);
    }

    public void PlaySound(string name, float pitch = 1.0f, float volume = 1.0f)
    {
        Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>
        {
            { "slice", sliceSound },
            { "blood", bloodSound },
            { "startSlowMotion", startSlowMotionSound },
            { "endSlowMotion", endSlowMotionSound },
            { "shoot", shootSound },
            { "hurt", hurtSound },
            { "explosion", explosionSound },
            { "fireworks", fireworkSound},
        };

        StartCoroutine(ExecuteSound(soundEffects[name], pitch, volume));
    }
}
