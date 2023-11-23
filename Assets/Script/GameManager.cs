using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("----[Core]")]
    public bool isOver;

    public int MaxScore;
    public int score;

    public int maxLevel = 12;

    [Range(1, 50)] public int poolSize;

    [Header("----[Audio]")]
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;

    public enum sfx { LevelUp, Next, Attach, Button, Over };
    public int sfxCursor;

    [Header("----[UI]")]
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        //프레임 설정 속성
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        bgmPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //pc : esc / mobile : 뒤로가기
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    //create cell
    //get cell
    //public void TouchDown()
    //{
    //    if (lastDongle == null)
    //    {
    //        return;
    //    }
    //    lastDongle.Drag();

    //}

    //public void TouchUp()
    //{
    //    if (lastDongle == null)
    //    {
    //        return;
    //    }
    //    lastDongle.Drop();
    //    lastDongle = null;
    //}


}
