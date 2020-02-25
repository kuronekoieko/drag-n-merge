using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    float timer;
    bool isLimit;

    public void OnStart()
    {
        audioSource = GetComponent<AudioSource>();
        if (i == null) i = this;
    }

    public void PlayOneShot(int index)
    {
        if (isLimit) { return; }
        isLimit = true;

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

        DOVirtual.DelayedCall(0.05f, () =>
        {
            isLimit = false;
        });
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