using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;

    
    [System.Serializable]
    public class Sound
    {
        public SoundName name;
        public List<AudioClip> clips = new List<AudioClip>();
        [Range(0, 1)] public float volume = 1;
        [Range(-3, 3)] public float pitch = 1;
    }

    [ArrayElementTitle("name")]
    public List<Sound> sounds = new List<Sound>();
    ActionsMap actionsMap;

    private void OnEnable() => actionsMap.Gameplay.Enable();
    private void OnDisable() => actionsMap.Gameplay.Disable();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        actionsMap = new ActionsMap();

        actionsMap.Gameplay.Debug.performed += ctx => DebugSound();
    }

    public void PlaySoundLoop(SoundName soundName, AudioSource source)
    {
        int index = GetIndex(soundName);
        if (index == sounds.Count)
        {
            Debug.LogError("There is no sound with this name " + soundName + " on SoundsManger, please verify your typing.");
            return;
        }
        source.pitch = sounds[index].pitch;
        source.volume = sounds[index].volume;
        source.clip = sounds[index].clips[Random.Range(0, sounds[index].clips.Count)];
        source.loop = true;
    }

    public void PlaySoundOneShot(SoundName soundName, AudioSource source)
    {
        int index = GetIndex(soundName);
        if (index == sounds.Count)
        {
            Debug.LogError("There is no sound with this name " + soundName + " on SoundsManger, please verify your typing.");
            return;
        }
        source.pitch = sounds[index].pitch;
        source.PlayOneShot(sounds[index].clips[Random.Range(0, sounds[index].clips.Count)], sounds[index].volume);
    }

    int GetIndex(SoundName name)
    {
        foreach (Sound item in sounds)
        {
            if (name == item.name)
                return sounds.IndexOf(item);
        }
        return sounds.Count;
    }

    public enum SoundName
    {
        TO_DEFINE, StomachEmpty, LungsEmpty, StomachFull, LungsFull, LeverInteraction, HeartDamage, HeartLow, Walk, Jump
    }

    void DebugSound()
    {
        GetComponent<AudioSource>().Play();
    }
}
