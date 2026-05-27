using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : Singleton<AudioController>   
{
    // Start is called before the first frame update
    [SerializeField] AudioSource _audio;
    [SerializeField] List<AudioClip> _listClip = new List<AudioClip>();
    List<AudioSource> _sounds = new List<AudioSource>();
    
    public void PlaySound(string nameSound)
    {
        AudioClip audioClip = null;
        foreach(AudioClip a in _listClip)
        {
            if(a.name.ToLower().Equals(nameSound.ToLower()))
            {
                audioClip = a;
                break;
            }
        }
        if(audioClip == null)
        {
            Debug.Log("sound " + nameSound + "not exist");
        }
        AudioSource sourceSound = null; 
        foreach(AudioSource a in _sounds)
        {
            if (a.gameObject.activeSelf)
                continue;
            sourceSound = a;
        }
        if (sourceSound == null)
        {
            sourceSound = Instantiate(_audio, this.transform.position,
                Quaternion.identity, this.transform); 
            _sounds.Add(sourceSound);
        }
        sourceSound.clip = audioClip;
        sourceSound.gameObject.SetActive(true);
        sourceSound.Play();
    }
}
