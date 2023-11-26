using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MngSound : MonoBehaviour
{
    public static MngSound instance;

    [Header("----[Audio]")]
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;

    public enum sfx { LevelUp, Next, Attach, Button, Over, Victory };
    public int sfxCursor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SfxPlay(sfx type)
    {
        switch (type)
        {
            case sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[0];
                break;
            case sfx.Attach:
                sfxPlayer[sfxCursor].clip = sfxClip[1];
                break;
            case sfx.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[2];
                break;
            case sfx.Over:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            case sfx.Victory:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            //case sfx.Button:
            //    sfxPlayer[sfxCursor].clip = sfxClip[5];
            //    break;
            
        }
        sfxPlayer[sfxCursor].Play();

        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

}
