using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using static SoundType;
using Unity.VisualScripting;

[DefaultExecutionOrder(-100)]
public class AudioController : MonoBehaviour
{
    public AudioSource BaseLoopSource
    {
        get
        {
            return loopSources[0];
        }
    }

    //public static AudioController Instance = null;


    [Range(0, 1)] public float SFXVolume;
    [Range(0, 1)] public float AmbientVolume;

    [Range(0, 1)] public float MusicVolume;

    [SerializeField]
    private List<AudioSource> loopSources = new List<AudioSource>();

    public List<AudioSource> SFXActive = new List<AudioSource>();


    private List<AudioClip> sfx = new List<AudioClip>();
    private List<AudioClip> loops = new List<AudioClip>();

    private Dictionary<string, int> lastPlayedSounds = new Dictionary<string, int>();
    private Dictionary<string, float> limitedFrequencySounds = new Dictionary<string, float>();

    private int[] DEFAULT_LOOPSOURCE_INDICES = new int[] {0};
    private const string SOUNDID_REPEAT_DELIMITER = "#";

    public bool underwater;

    public AudioReverbZone audioReverbZone;

    private void Awake()
    {
        G.audio = this;

        SFXVolume = 1f;
        AmbientVolume = 1f;
        MusicVolume = 1f;

        //if (Instance)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        //Instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        foreach (object o in Resources.LoadAll("Audio/SFX"))
        {
            sfx.Add((AudioClip)o);
        }
        foreach (object o in Resources.LoadAll("Audio/Loops"))
        {
            loops.Add((AudioClip)o);
        }
    }


    // just in case
    public AudioSource GetLoopSource(int index)
    {
        return loopSources[index];
    }

    public AudioClip GetLoopClip(string loopId)
    {
        return loops.Find(x => x.name.ToLowerInvariant() == loopId.ToLowerInvariant());
    }
    //
    
    //Main
    public AudioSource PlaySound(string soundId, float volume, Vector3 position, float skipToTime = 0f, AudioParams.Pitch pitch = null,AudioParams.Randomization randomization = null,
        AudioParams.Distortion distortion = null,AudioParams.Repetition repetition = null,bool looping = false)
    {
        if (repetition != null)
        {
            if (RepetitionIsTooFrequent(soundId, repetition.minRepetitionFrequency,repetition.entryId))
            {
                return null;
            }
        }

        string randomVariationId = soundId;
        if (randomization != null)
        {
            randomVariationId = GetRandomVariationOfSound(soundId, randomization.noRepeating);
        }

        var source = CreateAudioSourceForSound(randomVariationId, position,looping);
        if (source != null)
        {
            float correctVolume = NormalizeVolume(volume, SoundType.SFX);
            source.volume = correctVolume;
            source.time = source.clip.length * skipToTime;

            if (pitch != null)
                source.pitch = pitch.pitch;

            if (distortion != null)
            {
                if (distortion.muffled)
                {
                    MuffleSource(source, distortion.cutOffFrequency, distortion.lowpassResonance);
                }
            }
            else
            {
                if (underwater)
                {
                    MuffleSource(source, 1000, 1);
                }
            }

            SFXActive.Add(source);
            return source;
        }
        return null;
    }

    private string GetRandomVariationOfSound(string soundPrefix, bool noRepeating)
    {
        string soundId = "";

        if (!string.IsNullOrEmpty(soundPrefix))
        {
            List<AudioClip> variations = sfx.FindAll(x => x != null && x.name.ToLowerInvariant().StartsWith(soundPrefix.ToLowerInvariant() + SOUNDID_REPEAT_DELIMITER));
            if (variations.Count > 0)
            {
                int index = Random.Range(0, variations.Count) + 1;
                if (noRepeating)
                {
                    if (!lastPlayedSounds.ContainsKey(soundPrefix))
                    {
                        lastPlayedSounds.Add(soundPrefix, index);
                    }
                    else
                    {
                        int breakOutCounter = 0;
                        const int BREAK_OUT_THRESHOLD = 100;
                        while (lastPlayedSounds[soundPrefix] == index && breakOutCounter < BREAK_OUT_THRESHOLD)
                        {
                            index = Random.Range(0, variations.Count) + 1;
                            breakOutCounter++;
                        }

                        if (breakOutCounter >= BREAK_OUT_THRESHOLD - 1)
                        {
                            Debug.Log("Broke out of infinite loop! AudioController.PlayRandomSound.");
                        }

                        lastPlayedSounds[soundPrefix] = index;
                    }
                }

                soundId = soundPrefix + SOUNDID_REPEAT_DELIMITER + index;
            }
            else
            {
                soundId = soundPrefix;
            }
        }

        return soundId;
    }

    public AudioClip GetAudioClip(string soundId)
    {
        return sfx.Find(x => x.name.ToLowerInvariant() == soundId.ToLowerInvariant());
    }

    private AudioSource CreateAudioSourceForSound(string soundId, Vector3 position, bool looping)
    {
        if (!string.IsNullOrEmpty(soundId))
        {
            AudioClip sound = GetAudioClip(soundId);

            if (sound != null)
            {
                return InstantiateAudioObject(sound, position, looping);
            }
        }

        return null;
    }

    private AudioSource InstantiateAudioObject(AudioClip clip, Vector3 pos, bool looping)
    {
        GameObject tempGO = new GameObject("Audio_" + clip.name);
        tempGO.transform.position = pos;

        DontDestroyOnLoad(tempGO);

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;

        aSource.Play();
        if (looping)
        {
            aSource.loop = true;
        }
        else
        {
            Destroy(tempGO, clip.length * 3f);
        }
        return aSource;
    }
    private void MuffleSource(AudioSource source, float cutoff = 300f, float lowpassResonance = 1f)
    {
        var filter = source.gameObject.AddComponent<AudioLowPassFilter>();
        filter.cutoffFrequency = cutoff;
    }

    private bool RepetitionIsTooFrequent(string soundId, float frequencyMin, string entrySuffix = "")
    {
        float time = Time.unscaledTime;
        string soundKey = soundId + entrySuffix;

        if (limitedFrequencySounds.ContainsKey(soundKey))
        {
            if (time - frequencyMin > limitedFrequencySounds[soundKey])
            {
                limitedFrequencySounds[soundKey] = time;
                return false;
            }
        }
        else
        {
            limitedFrequencySounds.Add(soundKey, time);
            return false;
        }

        return true;
    }

    private void UnMuffleSource(AudioSource source)
    {
        var lowPassFilter = source.GetComponent<AudioLowPassFilter>();
        if (lowPassFilter != null)
        {
            Destroy(lowPassFilter);
        }
    }

    //Things related to lops 
    public void MuffleLoop(float cutoff, int loopIndex = 0)
    {
        MuffleSource(loopSources[loopIndex], cutoff);
    }

    public void UnMuffleLoop(int loopIndex = 0)
    {
        UnMuffleSource(loopSources[loopIndex]);
    }


    public void SetLoopVolumeImmediate(float volume, int sourceIndex = 0)
    {
        loopSources[sourceIndex].volume = volume;
    }

    public void SetLoopVolume(float volume, float duration, int sourceIndex = 0, bool cancelOtherFades = true)
    {
        if (cancelOtherFades)
            CancelFades();
        loopSources[sourceIndex].DOFade(volume, duration);
    }

    public void SetLoopPaused(bool paused)
    {
        foreach (AudioSource loopSource in loopSources)
        {
            if (paused)
            {
                loopSource.Pause();
            }
            else
            {
                loopSource.UnPause();
            }
        }
    }

    public void StopAllLoops()
    {
        CancelFades();
        foreach (AudioSource loopSource in loopSources)
        {
            loopSource.Stop();
        }
    }

    public void StopLoop(int sourceIndex = 0)
    {
        loopSources[sourceIndex].Stop();
    }


    public void FadeOutLoop(float fadeDuration, params int[] sourceIndices)
    {
        //CancelFades();

        if (sourceIndices == null || sourceIndices.Length == 0)
        {
            sourceIndices = DEFAULT_LOOPSOURCE_INDICES;
        }

        for (int i = 0; i < sourceIndices.Length; i++)
        {
            loopSources[sourceIndices[i]].DOFade(0f, fadeDuration);
        }
    }

    public void FadeInLoop(float fadeDuration, float toVolume, params int[] sourceIndices)
    {
        //CancelFades();

        if (sourceIndices == null || sourceIndices.Length == 0)
        {
            sourceIndices = DEFAULT_LOOPSOURCE_INDICES;
        }

        for (int i = 0; i < sourceIndices.Length; i++)
        {
            loopSources[sourceIndices[i]].DOFade(toVolume, fadeDuration);
        }
    }
    private void CancelFades()
    {
        foreach (var source in loopSources)
        {
            source.DOKill();
        }
    }

    public void SetLoopAndPlay(string loopName,float volume,SoundType type = Music, int sourceIndex = 0, bool looping = true, bool cancelFades = true)
    {
        if (cancelFades)
        {
            CancelFades();
        }
        TrySetLoop(loopName, sourceIndex);
        RestartLoop(sourceIndex);

        loopSources[sourceIndex].volume = NormalizeVolume(volume,type);
        loopSources[sourceIndex].loop = looping;
    }

    public void RestartLoop(int sourceIndex,float volume = 1f)
    {
        loopSources[sourceIndex].Stop();
        loopSources[sourceIndex].time = 0f;
        loopSources[sourceIndex].volume = volume;
        loopSources[sourceIndex].pitch = 1f;
        loopSources[sourceIndex].Play();
    }

    private void TrySetLoop(string loopName, int sourceIndex = 0)
    {
        AudioClip loop = GetLoop(loopName);

        if (loop != null)
        {
            loopSources[sourceIndex].clip = loop;
            loopSources[sourceIndex].pitch = 1f;
        }
    }

    private AudioClip GetLoop(string loopName)
    {
        return loops.Find(x => x.name == loopName);
    }

    private float NormalizeVolume(float volume,SoundType type)
    {
        if(type == Music)
           return Mathf.Clamp01(volume) * MusicVolume;
        if(type == SFX)
            return Mathf.Clamp01(volume) * SFXVolume;
        if(type == Ambient)
            return Mathf.Clamp01(volume) * AmbientVolume;

        return volume;
    }

}

public enum SoundType
{
    SFX,
    Music,
    Ambient
}
