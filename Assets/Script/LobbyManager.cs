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
        //Debug.Log("씬Lobby");
        ScoreUpdate();
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
