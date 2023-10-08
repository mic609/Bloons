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
    private AudioClip _sound;

    private void Awake()
    {
        _soundSource = GetComponent<AudioSource>();
        _soundSourceForCeramics = GetComponent<AudioSource>();
        _soundSourceForBombs = GetComponent<AudioSource>();

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
        if(_sound != null)
        {
            switch (_sound.name)
            {
                case "ceramicPop":
                    {
                        if (Time.time - lastAttackTimeCeramic >= _soundIntervalCeramic)
                        {
                            lastAttackTimeCeramic = Time.time;
                            _soundSourceForCeramics.PlayOneShot(_sound);
                            _sound = null;
                        }
                    }
                    break;
                case "bombSound":
                    {
                        if (Time.time - lastAttackTimeBomb >= _soundIntervalBomb)
                        {
                            lastAttackTimeBomb = Time.time;
                            _soundSourceForBombs.PlayOneShot(_sound);
                            _sound = null;
                        }
                    }
                    break;
                default:
                    {
                        if (Time.time - lastAttackTime >= _soundInterval)
                        {
                            lastAttackTime = Time.time;
                            _soundSource.PlayOneShot(_sound);
                            _sound = null;
                        }
                    }
                break;
            }
        }
    }

    public void PlaySound(AudioClip sound)
    {
        _sound = sound;
    }
}