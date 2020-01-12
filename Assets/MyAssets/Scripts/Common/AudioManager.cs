using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 【Unity】AudioSourceの同時再生による音割れを防ぐ方法
/// http://hecres.hatenablog.com/entry/2018/03/10/181127
/// Unityのオーディオミキサーを使って音声の変換や音量を変更する
/// https://gametukurikata.com/se/audiomixer
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAS;
    [SerializeField] AudioFile[] audioFiles;
    AudioSource audioSource;
    public static AudioManager i;

    public void OnStart()
    {
        audioSource = GetComponent<AudioSource>();
        if (i == null) i = this;
        DontDestroyOnLoad(i);
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