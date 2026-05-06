using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChild : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource _audio;
    void Awake()
    {
        _audio = this.GetComponent<AudioSource>();
        
    }
    private void OnEnable()
    {
        StartCoroutine(waitSoundDone());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator waitSoundDone()
    {
        yield return new WaitUntil(()=> !_audio.isPlaying);
        this.gameObject.SetActive(false);
    }
}
