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

        //프레임 설정 속성
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
        // 이것도 설정버튼으로해서 나가게 바꾸어야함
        //pc : esc / mobile : 뒤로가기
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
    /// 장외 콜라이더에 닿거나, 빨간 선에 몇초이상 닿을시 게임오버
    /// 게임오버되면 게임을 멈추고, 실패브금과 함께 나가기 버튼이 활성화
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
        //1.장면 안에 활성화 되어있는 모든 동글 가져오기
        Cell[] cells = FindObjectsOfType<Cell>();


        for (int index = 0; index < cells.Length; index++)
        {
            cells[index].rigid.simulated = false;
        }


        //2. 1번의 목록을 하나씩 접근해서 지우기
        for (int index = 0; index < cells.Length; index++)
        {
            cells[index].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        //저장용
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
        //게임오버가되면 다음동글을 주지않음
        if (isOver)
        {
            return;
        }
        lastCell = GetCell();

        //초기화
        lastCell.level = Random.Range(0, 7); //마지막값 포함 x 
        lastCell.transform.position = spawner.transform.position;
        lastCell.gameObject.SetActive(true);

        //부모 변경
        lastCell.transform.parent = cellPool;

        //크기 변경
        lastCell.SizeUp(lastCell.level);

        //이미지 변경
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
