using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAS;
    [SerializeField] AudioFile[] audioFiles;
    AudioSource audioSource;
    public static AudioManager i;

    public void OnStart()
    {
        audioSource = GetComponent<AudioSource>();
        i = this;
    }

    public void PlayOneShot(int index)
    {
        AudioClip clip;
        try
        {
            clip = audioFiles[index].audioClip;
        }
        catch (System.Exception)
        {

            throw;
        }

        if (!clip) { return; }
        audioSource.PlayOneShot(clip);
    }

    public void RePlayBGM()
    {
        if (bgmAS) bgmAS.Play();
    }

}

[System.Serializable]
public class AudioFile
{
    public AudioClip audioClip;
    public string soundName;
}