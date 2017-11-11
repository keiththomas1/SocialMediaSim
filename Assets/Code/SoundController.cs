using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioClip[] clickSounds;
    public AudioClip[] likeSounds;
    public AudioClip backSound;

    private GlobalVars globalVars;
    private AudioSource source;
    private float clickSoundLevel;
    private float likeSoundLevel;
    private float backSoundLevel;

	// Use this for initialization
	void Start () {
        globalVars = GlobalVars.Instance;
        source = GetComponent<AudioSource>();
        clickSoundLevel = 0.1f;
        likeSoundLevel = 0.5f;
        backSoundLevel = 0.5f;

        GameObject.DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.name)
                {
                    case "GameVolumeOnButton":
                        globalVars.MusicLevel = 1.0f;
                        break;
                    case "GameVolumeOffButton":
                        globalVars.MusicLevel = 0.0f;
                        break;
                    case "SoundEffectsOnButton":
                        globalVars.SoundEffectsLevel = 1.0f;
                        break;
                    case "SoundEffectsOffButton":
                        globalVars.SoundEffectsLevel = 0.0f;
                        break;
                }
            }
        }
    }

    public void PlayClickSound(int index = -1)
    {
        if (index == -1)
        {
            source.PlayOneShot(ChooseRandomClickSound(), clickSoundLevel * globalVars.SoundEffectsLevel);
        }
        else
        {
            if (index >= 0 && index < clickSounds.Length)
            {
                source.PlayOneShot(clickSounds[index], clickSoundLevel * globalVars.SoundEffectsLevel);
            }
        }
    }

    public void PlayLikeSound()
    {
        source.PlayOneShot(ChooseRandomLikeSound(), likeSoundLevel * globalVars.SoundEffectsLevel);
    }

    public void PlayBackSound()
    {
        source.PlayOneShot(backSound, backSoundLevel * globalVars.SoundEffectsLevel);
    }

    private AudioClip ChooseRandomClickSound()
    {
        var randomClip = Random.Range(0, clickSounds.Length);
        return clickSounds[randomClip];
    }

    private AudioClip ChooseRandomLikeSound()
    {
        var randomClip = Random.Range(0, likeSounds.Length);
        return likeSounds[randomClip];
    }
}

