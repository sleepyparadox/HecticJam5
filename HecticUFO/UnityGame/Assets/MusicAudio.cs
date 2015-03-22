using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

public enum AudioStackRule
{
    Replace,
    Singleton,
    OneShot,
}

public class MusicAudio : MonoBehaviour
{
    public static MusicAudio S;
    public AudioClip CowMoo;
    public AudioClip UFOSuck;
    public AudioClip UFOBlow;
    public AudioClip BabyGrow;
    public AudioClip Thump;

    Dictionary<AudioClip, UnityObject> CurrentSounds = new Dictionary<AudioClip, UnityObject>();

    public void Play(AudioClip clip, Vector3? at, AudioStackRule rule = AudioStackRule.Replace, float volume = 1f)
    {
        volume = Mathf.Clamp01(volume);
        if (rule == AudioStackRule.Singleton && CurrentSounds.ContainsKey(clip))
            return;

        if(CurrentSounds.ContainsKey(clip))
        {
            CurrentSounds[clip].Dispose();
            CurrentSounds.Remove(clip);
        }

        var clipObj = new UnityObject();
        if (at.HasValue)
            clipObj.WorldPosition = at.Value;
        var src = clipObj.GameObject.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.Play();
        src.loop = false;
        clipObj.UnityUpdate += (u) =>
        {
            if(!src.isPlaying)
            {
                if (clipObj.Alive)
                    clipObj.Dispose();
                if (CurrentSounds.ContainsKey(clip)
                    && CurrentSounds[clip] == clipObj)
                    CurrentSounds.Remove(clip);
            }
        };

        if(rule != AudioStackRule.OneShot)
            CurrentSounds[clip] = clipObj;
    }

    public MusicAudio()
    {
        S = this;
    }
}
