using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

  //  public Sound FindSoundSource(string name)
  //  {
  //      Sound s = Array.Find(sounds, sound => sound.name == name);
  //      return s;
  //  }

   public void EnableAudio(string name)
   {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.enabled = true;
   }

   public void DisableAudio(string name)
   {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.enabled = false;
   }

   public void PlayAudio(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
   }
   public void StopAudio(string name)
   {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
   }
   public void PauseAudio(string name)
   {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Pause();
   }

 //   public void fadeAudio(Sound s,float targetVolume,float timer,float fadeInTime)
 //   { 
 //       s.source.volume = Mathf.Lerp(s.source.volume, targetVolume, timer / fadeInTime);
 //   }





}
