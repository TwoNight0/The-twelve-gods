using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    
    //----Button
    [Header("Button")]
    public Button play;
    public Button madeby;

    [Header("GameObj")]
    public GameObject Text_madeby;
    public GameObject Text_BestScore;

    [Header("AudioClip")]
    public AudioClip btnClick;
    public AudioClip btnClean;
    public AudioSource sfxPlayer;

    void Start()
    {
        resolutionSetting();
        //Debug.Log("씬Lobby");
        ScoreUpdate();
    }
    public void resolutionSetting()
    {
        // 해상도를 픽셀로 고정
        //Screen.SetResolution(1920, 1080, true);
        Debug.Log("re");
        // 해상도를 비율로 고정
        float targetRatio = 9.0f / 16.0f;
        float ratio = (float)Screen.width / (float)Screen.height;
        float scaleHeight = ratio / targetRatio;
        float fixedWidth = (float)Screen.width / scaleHeight;

        Screen.SetResolution((int)fixedWidth, Screen.height, true);
    }
    public void btnPlay()
    {
        SceneManager.LoadScene("Game");
    }
    public void btnMadeBy()
    {
        StartCoroutine(btnMadeByCoroutine());
    }
    public void btnExit()
    {
        Debug.Log("종료합니다");
        Application.Quit();
        
    }

    public void btnClickSound()
    {
        sfxPlayer.clip = btnClick;
        sfxPlayer.Play();
    }

    public void btnCleanSound()
    {
        sfxPlayer.clip = btnClean;
        sfxPlayer.Play();
    }

    public void ScoreUpdate()
    {
        if (PlayerPrefs.HasKey("MaxScore"))
        {
            int tmpScore = PlayerPrefs.GetInt("MaxScore");
            Debug.Log("Score : " + tmpScore);
            Debug.Log(PlayerPrefs.GetInt("MaxScore"));
            Text_BestScore.GetComponent<TextMeshProUGUI>().text = tmpScore.ToString();
        }
        else { return; }
        
    }

    IEnumerator btnMadeByCoroutine()
    {
        Text_madeby.SetActive(true);
        
        yield return new WaitForSeconds(2.5f);

        Text_madeby.SetActive(false);
    }


}
