using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private int maxLevel = 11;
   
    //----score
    private int maxScore;
    private int score;

    //----bool
    private bool isOver = false;

    [Header("Object")] 
    public GameObject cellPrefab;
    public GameObject effect;

    [Header("Panel")]
    public GameObject gameover;
    public GameObject setting;
    
    //----Transform
    [HideInInspector] public Transform cellPool;
    [HideInInspector] public Transform recyclePool;


    [Tooltip("Cell ���� ��ġ")]public Transform spawner;

    [Range(1, 50)] public int poolSize;
    [HideInInspector] public Cell lastCell;


    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI bestScoreText;

    public int PubMaxLevel {
        get => maxLevel;
        set => maxLevel = value;
    }

    public int PubScore {
        get => score;
        set => score = value;
    }

    public bool PubOver
    {
        get => isOver;
        set => isOver = value;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        //������ ���� �Ӽ�
        Application.targetFrameRate = 60;

        //Text point �Ҵ�
        scoreText = GameObject.Find("Point").GetComponent<TextMeshProUGUI>();
        bestScoreText = GameObject.Find("PointPre ").GetComponent<TextMeshProUGUI>();

        //Pool �Ҵ�
        cellPool = GameObject.Find("Pool").transform.GetChild(0);
        recyclePool = GameObject.Find("Pool").transform.GetChild(1);
    }

    void Start()
    {
        MngSound.instance.bgmPlayer.Play();
        //PlayerPrefs.SetInt("MaxScore", 0);
        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        createManyCell(poolSize);

        ScoreUpdate();
        NextCell();
    }

    void Update()
    {
        // �̰͵� ������ư�����ؼ� ������ �ٲپ����
        //pc : esc / mobile : �ڷΰ���
        if (Input.GetButtonDown("Cancel"))
        {
            btnSetting();
        }
    }

    #region Cell
    //create cell
    private void createCell()
    {
        GameObject insCell_obj = Instantiate(cellPrefab, recyclePool.transform);
        insCell_obj.SetActive(false);
    }

    private void createManyCell(int count)
    {
        for (int i = 0; i < count; i++) {
            createCell();
        }
    }

    private Cell GetCell()
    {
        if(recyclePool.childCount == 0)
        {
            createManyCell(10);
            GameObject getcell = recyclePool.transform.GetChild(0).gameObject;
            Cell cell = getcell.GetComponent<Cell>();
            return cell;
        }
        else
        {
            GameObject getcell = recyclePool.transform.GetChild(0).gameObject;
            Cell cell = getcell.GetComponent<Cell>();
            return cell;
        }
    }

    #endregion 
    public void TouchDown()
    {
        if (lastCell == null)
        {
            return;
        }
        lastCell.Drag();

    }

    public void TouchUp()
    {
        if (lastCell == null)
        {
            return;
        }
        lastCell.Drop();
        lastCell = null;
    }

    public void ReStart()
    {
        //���ھ� �ǵ�����
        score = 0;
        ScoreUpdate();

        isOver = false;

        //���ӿ����гβ���
        gameover.SetActive(false);

        //���Ű��
        MngSound.instance.bgmPlayer.Play();

        NextCell();
    }

    /// <summary>
    /// GameOver
    /// ��� �ݶ��̴��� ��ų�, ���� ���� �����̻� ������ ���ӿ���
    /// ���ӿ����Ǹ� ������ ���߰�, ���к�ݰ� �Բ� ������ ��ư�� Ȱ��ȭ
    /// </summary>
    public void GameOver()
    {
        if (!isOver)
        {
            return;
        }
      
        //�г� Ȱ��ȭ
        gameover.SetActive(true);
        isOver = true;
        StartCoroutine(GameOverRoutine());
        StopAllCoroutines();
    }
    IEnumerator GameOverRoutine()
    {
        MngSound.instance.bgmPlayer.Stop();
        MngSound.instance.SfxPlay(MngSound.sfx.Over);

        //1.��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� ���� ��������
        Cell[] cells = FindObjectsOfType<Cell>();

        //2. 1���� ����� �ϳ��� �����ؼ� �����
        for (int index = 0; index < cells.Length; index++)
        {
            cells[index].rigid.simulated = false;
            cells[index].transform.parent = recyclePool;
            cells[index].Hide();
        }

        yield return new WaitForSeconds(0.5f);

        //�����
        if (PlayerPrefs.HasKey("MaxScore")) {
            maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        }
        else
        {
            maxScore = score;
        }
        PlayerPrefs.SetInt("MaxScore", maxScore);
        PlayerPrefs.Save();

        Debug.Log(PlayerPrefs.GetInt("MaxScore"));

        yield return new WaitForSeconds(2.0f);
    }

    #region btn method
    public void btnGameOver()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void btnSetting()
    {
        setting.SetActive(true);

        //����
        Time.timeScale = 0.0f;
    }

    public void btnResume()
    {
        setting.SetActive(false);

        //��Ȱ��ȭ
        Time.timeScale = 1.0f;
    }


    #endregion

    public void ScoreUpdate()
    {
        scoreText.text = score.ToString();

        if (PlayerPrefs.HasKey("MaxScore"))
        {
            int tmpScore = PlayerPrefs.GetInt("MaxScore");
            //Debug.Log("Score : " + score);
            //Debug.Log(PlayerPrefs.GetInt("MaxScore"));

            if(tmpScore < score)
            {
                bestScoreText.color = Color.magenta;
            }
            
            bestScoreText.text = score.ToString();
        }
        else { return; }
        
    }

    public void NextCell()
    {
        //���ӿ������Ǹ� ���������� ��������
        if (isOver)
        {
            return;
        }
        lastCell = GetCell();

        //�ʱ�ȭ
        lastCell.level = Random.Range(0, 4); //�������� ���� x 
        lastCell.transform.position = spawner.transform.position;
        lastCell.gameObject.SetActive(true);

        //�θ� ����
        lastCell.transform.parent = cellPool;

        //ũ�� ����
        lastCell.SizeUp(lastCell.level);

        //�̹��� ����
        lastCell.SetImg(lastCell.level);

        StartCoroutine(WaitNext());
    }

    IEnumerator WaitNext()
    {
        while (lastCell != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.8f);
        NextCell();
    }

}
