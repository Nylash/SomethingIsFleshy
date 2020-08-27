using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public List<AudioClip> clips = new List<AudioClip>();
        [Range(0, 1)] public float volume = 1;
        [Range(-3, 3)] public float pitch = 1;
    }

    public List<Sound> sounds = new List<Sound>();

    public void PlaySoundLoop(string soundName, AudioSource source)
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

    public void PlaySoundOneShot(string soundName, AudioSource source)
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

    int GetIndex(string name)
    {
        foreach (Sound item in sounds)
        {
            if (name == item.name)
                return sounds.IndexOf(item);
        }
        return sounds.Count;
    }
}
