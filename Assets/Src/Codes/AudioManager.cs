using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    [SerializeField]
    private AudioClip bgmClip;

    [Header("#Final Boss BGM")]
    [SerializeField]
    private AudioClip finalBossBgmClip;  // 마지막 보스용 배경음악

    [SerializeField]
    private float _bgmVolume;

    public float BgmVolume
    {
        get { return _bgmVolume; }
        set
        {
            _bgmVolume = value;
            if (bgmPlayer != null)
            {
                bgmPlayer.volume = _bgmVolume;
            }
        }
    }
    public void PlayFinalBossBgm()
    {
        bgmPlayer.clip = finalBossBgmClip;  // 배경 음악을 마지막 보스용 배경 음악으로 변경
        bgmPlayer.Play();  // 배경 음악 재생
    }

    private AudioSource bgmPlayer;
    private AudioHighPassFilter bgmEffect;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    //public enum Sfx {Whip, FireWhip, FlameBullet, SwordBullet, Boomerang, Thunder, Darkness, Freeze, GemPickup, MagnetPickup, PlayerHit, BoxHit, Blast, Guardian}

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.clip = bgmClip;
        bgmPlayer.volume = _bgmVolume;  // 이곳에서 초기 볼륨 설정

        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    //internal void PlaySfx(Sfx boxHit)
    //{
        //throw new NotImplementedException();
    //}

    //public void PlaySfx(Sfx sfx, float v)
    //{
        //for (int index=0; index < sfxPlayers.Length; index++)
        //{
            //int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            //if (sfxPlayers[loopIndex].isPlaying)
               // continue;

            //channelIndex = loopIndex;
            //sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            //sfxPlayers[loopIndex].Play();
            //break;
        //}
    //}

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }
}
