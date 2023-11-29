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
        //Debug.Log("��Lobby");
        ScoreUpdate();
    }
    public void resolutionSetting()
    {
        // �ػ󵵸� �ȼ��� ����
        //Screen.SetResolution(1920, 1080, true);
        Debug.Log("re");
        // �ػ󵵸� ������ ����
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
        Debug.Log("�����մϴ�");
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
