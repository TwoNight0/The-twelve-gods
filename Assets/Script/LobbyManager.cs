using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    
    //----Button
    public Button play;
    public Button madeby;

    public GameObject Text_madeby;

    void Start()
    {
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
    
    public void ScoreUpdate()
    {
        if (PlayerPrefs.HasKey("MAxScore"))
        {
            int score = PlayerPrefs.GetInt("MAxScore");
            Text_madeby.GetComponent<TextMeshProUGUI>().text = score.ToString();
        }
        else { return; }
        
    }

    IEnumerator btnMadeByCoroutine()
    {
        Text_madeby.SetActive(true);
        
        yield return new WaitForSeconds(4.0f);

        Text_madeby.SetActive(false);
    }


}
