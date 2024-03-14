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
    private AudioClip finalBossBgmClip;  // ������ ������ �������

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
        bgmPlayer.clip = finalBossBgmClip;  // ��� ������ ������ ������ ��� �������� ����
        bgmPlayer.Play();  // ��� ���� ���
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
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.clip = bgmClip;
        bgmPlayer.volume = _bgmVolume;  // �̰����� �ʱ� ���� ����

        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // ȿ���� �÷��̾� �ʱ�ȭ
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
