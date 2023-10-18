using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public enum SoundEffectType
{
    win,
    badEvent,
    goodEvent,
    walk,
    snake,
    ladder,
    roll,
    click
}
public class SoundManager : MonoBehaviour
{
    public List<AudioSource> sources= new List<AudioSource>();
    [Header("Audios")]
    public AudioClip winAudio;
    public AudioClip badEventAudio;
    public AudioClip goodEventAudio;
    public AudioClip walkAudio;
    public AudioClip goDownAudio;
    public AudioClip goUpAudio;
    public AudioClip diceAudio;
    public AudioClip clickAudio;
    public List<AudioClip> clips;
    private void OnEnable()
    {
        clips = new AudioClip[] {
        winAudio,
        badEventAudio,
        goodEventAudio,
        walkAudio,
        goDownAudio,
        goUpAudio,
        diceAudio,
        clickAudio,
    }.ToList();
        foreach (var clip in clips)
        {
            var source=this.AddComponent<AudioSource>();
            source.clip= clip;
            sources.Add(source);
        }
    }
    public void Play(SoundEffectType type){
        int index =(int)type;
        sources[index].Play();
    }
}
