using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private float _soundInterval;
    [SerializeField] private float _soundIntervalCeramic;
    [SerializeField] private float _soundIntervalBomb;
    private float lastAttackTime = 0.0f;
    private float lastAttackTimeCeramic = 0.0f;
    private float lastAttackTimeBomb = 0.0f;

    private AudioSource _soundSource;
    private AudioSource _soundSourceForCeramics;
    private AudioSource _soundSourceForBombs;
    private AudioSource _musicSource;

    private List<AudioClip> _soundsToPlay;
    private List<AudioClip> _soundsToPlayTemp;

    private void Awake()
    {
        _soundSource = GetComponent<AudioSource>();
        _soundSourceForCeramics = GetComponent<AudioSource>();
        _soundSourceForBombs = GetComponent<AudioSource>();
        _soundsToPlay = new List<AudioClip>();
        _soundsToPlayTemp = new List<AudioClip>(_soundsToPlay);

        _musicSource = transform.GetChild(0).GetComponent<AudioSource>(); // return audio component of the first child

        // keep this object when we go to new scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // destroy duplicate gameobjects
        else if (Instance != null && Instance != this)
            Destroy(gameObject);

        // assign initial volumes
        //ChangeMusicVolume(0);
        //ChangeSoundVolume(0);
    }

    private void Update()
    {
        if(_soundsToPlay.Count > 0)
        {
            foreach (var sound in _soundsToPlay)
            {
                switch (sound.name)
                {
                    case "ceramicPop":
                        {
                            if (Time.time - lastAttackTimeCeramic >= _soundIntervalCeramic)
                            {
                                lastAttackTimeCeramic = Time.time;
                                _soundSourceForCeramics.PlayOneShot(sound);
                            }
                        }
                        break;
                    case "bombSound":
                        {
                            if (Time.time - lastAttackTimeBomb >= _soundIntervalBomb)
                            {
                                lastAttackTimeBomb = Time.time;
                                _soundSourceForBombs.PlayOneShot(sound);
                            }
                        }
                        break;
                    default:
                        {
                            if (Time.time - lastAttackTime >= _soundInterval)
                            {
                                lastAttackTime = Time.time;
                                _soundSource.PlayOneShot(sound);
                            }
                        }
                        break;
                }
            }
            _soundsToPlay.Clear();
        }
    }

    public void PlaySound(AudioClip sound)
    {
        _soundsToPlay.Add(sound);
    }
}