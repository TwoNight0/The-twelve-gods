using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MngSound : MonoBehaviour
{
    public static MngSound instance;

    [Header("----[Audio]")]
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;

    [Header("BGM Controller")]
    public Button btnMute;
    public Slider bgmScaleSlider;

    public bool isMute = false;

    public enum sfx { LevelUp, Next, Attach, Button, Over, Victory };
    [HideInInspector] public int sfxCursor;

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

    private void Start()
    {
        bgmScaleSlider.value = bgmPlayer.volume;
        bgmScaleSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
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
            case sfx.Over:
                sfxPlayer[sfxCursor].clip = sfxClip[2];
                break;
            case sfx.Victory:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            //case sfx.Button:
            //    sfxPlayer[sfxCursor].clip = sfxClip[5];
            //    break;
            
        }
        sfxPlayer[sfxCursor].Play();

        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

    public void BtnMute()
    {
        isMute = !isMute;
        if (isMute)
        {
            //���ҰŻ���
            btnMute.image.sprite = Resources.Load<Sprite>("ICON/" + "ICMute");
            bgmPlayer.mute = true;
        }
        else
        {
            //���Ұ� ���� �ƴҶ�
            btnMute.image.sprite = Resources.Load<Sprite>("ICON/" + "ICBGM");
            bgmPlayer.mute = false;
        }
    }

    void OnVolumeSliderChanged(float value)
    {
        // Slider ���� ����� ������ ȣ��Ǵ� �޼���
        bgmPlayer.volume = value;
    }

}
