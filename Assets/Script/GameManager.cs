using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private int maxLevel = 12;
   
    //----score
    private int MaxScore;
    private int score;

    //----bool
    private bool isOver;

    //----Object
    public GameObject cellPrefab;
    public GameObject effect;

    [HideInInspector] public Transform cellPool;
    [HideInInspector] public Transform recyclePool;
    public Transform spawner;

    private GameObject recyclePool_obj;


    [Range(1, 50)] public int poolSize;
    public Cell lastCell;


    private TextMeshProUGUI scoreText;

    public int PubMaxLevel {
        get => maxLevel;
        set => maxLevel = value;
    }

    public int PubScore {
        get => score;
        set => score = value;
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
        scoreText = GameObject.Find("Point").GetComponent<TextMeshProUGUI>();

        cellPool = GameObject.Find("Pool").transform.GetChild(0);
        recyclePool = GameObject.Find("Pool").transform.GetChild(1);

    }

    void Start()
    {
        MngSound.instance.bgmPlayer.Play();
        
        createManyCell(poolSize);
        NextCell();
    }

    void Update()
    {
        // �̰͵� ������ư�����ؼ� ������ �ٲپ����
        //pc : esc / mobile : �ڷΰ���
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
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


    /// <summary>
    /// GameOver
    /// ��� �ݶ��̴��� ��ų�, ���� ���� �����̻� ������ ���ӿ���
    /// ���ӿ����Ǹ� ������ ���߰�, ���к�ݰ� �Բ� ������ ��ư�� Ȱ��ȭ
    /// </summary>
    public void GameOver()
    {
        if (isOver)
        {
            return;
        }
        isOver = true;
        StartCoroutine(GameOverRoutine());
    }
    IEnumerator GameOverRoutine()
    {
        //1.��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� ���� ��������
        Cell[] cells = FindObjectsOfType<Cell>();


        for (int index = 0; index < cells.Length; index++)
        {
            cells[index].rigid.simulated = false;
        }


        //2. 1���� ����� �ϳ��� �����ؼ� �����
        for (int index = 0; index < cells.Length; index++)
        {
            cells[index].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        //�����
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MAxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);

        MngSound.instance.bgmPlayer.Stop();
        MngSound.instance.SfxPlay(MngSound.sfx.Over);
    }

    public void ScoreUpdate()
    {
        scoreText.text = score.ToString();
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
        lastCell.level = Random.Range(0, 7); //�������� ���� x 
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

        yield return new WaitForSeconds(1.0f);
        NextCell();
    }

}
