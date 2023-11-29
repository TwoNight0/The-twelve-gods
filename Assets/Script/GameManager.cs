using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    //---- level
    private int maxLevel = 11;
    private int limitLevel = 4;

    //----score
    private int maxScore;
    private int score;

    //cell num
    private int nextNum = -1;

    //----bool
    private bool isOver = false;
    //private bool isTouching = false;

    public float boarder = 2.4f;

    //----Vector2
    private Vector2 touchStartPos;
    private Vector2 touchDelta;

    [Header("Object")] 
    public GameObject cellPrefab;
    public GameObject effect;
    public GameObject afterCell;

    [Header("Panel")]
    public GameObject gameover;
    public GameObject setting;
    public GameObject victory;

    //----Transform

    [HideInInspector] public Transform cellPool;
    [HideInInspector] public Transform recyclePool;

    [Header("spawn point")]
    [Tooltip("Cell 생성 위치")]public Transform spawner;

    [Range(1, 50)] public int poolSize;
    [HideInInspector] public Cell lastCell;

    public LineRenderer lineRenderer;

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

        //프레임 설정 속성
        Application.targetFrameRate = 60;

        //Text point 할당
        scoreText = GameObject.Find("Point").GetComponent<TextMeshProUGUI>();
        bestScoreText = GameObject.Find("PointPre ").GetComponent<TextMeshProUGUI>();

        //Pool 할당
        cellPool = GameObject.Find("Pool").transform.GetChild(0);
        recyclePool = GameObject.Find("Pool").transform.GetChild(1);
    }

    void Start()
    {

        resolutionSetting();
        MngSound.instance.bgmPlayer.Play();
        //PlayerPrefs.SetInt("MaxScore", 0);
        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        createManyCell(poolSize);

        ScoreUpdate();
        NextCell();

        Lineinit();

        afterCell.SetActive(true);
    }

    void Update()
    {
        // 이것도 설정버튼으로해서 나가게 바꾸어야함
        //pc : esc / mobile : 뒤로가기
        if (Input.GetButtonDown("Cancel"))
        {
            btnSetting();
        }
        DrawDownRay();
        Touchtracker();
    }

    private void GameWin()
    {
        Cell[] cells = FindObjectsOfType<Cell>();

        //2. 1번의 목록을 하나씩 접근해서 지우기
        for (int index = 0; index < cells.Length; index++)
        {
            if(cells[index].level == maxLevel)
            {
                victory.SetActive(true);
                StartCoroutine(GameOverRoutine(MngSound.sfx.Victory));
            }
        }
    }

    public void resolutionSetting()
    {
        // 해상도를 픽셀로 고정
        //Screen.SetResolution(1920, 1080, true);
        //Debug.Log("re");
        // 해상도를 비율로 고정
        float targetRatio = 9.0f / 16.0f;
        float ratio = (float)Screen.width / (float)Screen.height;
        float scaleHeight = ratio / targetRatio;
        float fixedWidth = (float)Screen.width / scaleHeight;

        Screen.SetResolution((int)fixedWidth, Screen.height, true);
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

    #region line

    private void Lineinit()
    {

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;
      

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    private void LinePosUpdate(Vector3 pos2)
    {
        if(lastCell != null)
        {
            lineRenderer.SetPosition(0, lastCell.transform.position);
            lineRenderer.SetPosition(1, pos2);
        }
        else
        {
            return;
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

    public void Touchtracker() {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("터치 시작");
                // 터치 시작 위치 기록
                touchStartPos = touch.position;
                TouchDown();
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Debug.Log("터치 이동");
                // 터치 이동량 계산
                touchDelta = touch.position - touchStartPos;

                // 위치 제한 로직 추가
                float newX = Mathf.Clamp(lastCell.transform.position.x + touchDelta.x * Time.deltaTime * 0.3f, -2.35f, 2.35f);

                // 오브젝트 이동
                lastCell.transform.position = new Vector3(newX, 2.25f,0);

                // 오브젝트 이동
                //lastCell.transform.Translate( new Vector3(touchDelta.x * Time.deltaTime * 0.3f, 0, 0), Space.World);
                Vector3 pos = lastCell.transform.position;
                pos.x = pos.x + (float)(0.2 * (lastCell.level + 1) + 0.2);
                GameManager.instance.afterCell.transform.position = pos;

                // 터치 시작점 업데이트
                touchStartPos = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                //isTouching = false;
                Debug.Log("터치 종료");
                TouchUp();
            }
        }

        
    }

    public void moveWindow()
    {
        //마우스 위치를 스크린에서 월드포지션으로 바꿔 커서 역할
        Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (mousePos.x < -boarder)
        {
            mousePos.x = -boarder;
        }
        else if (mousePos.x > boarder)
        {
            mousePos.x = boarder;
        }
        mousePos.y = 2.25f;
        mousePos.z = 0f;

        transform.position = Vector3.Lerp(transform.position, mousePos, 1.5f);
    }


    public void ReStart()
    {
        //스코어 되돌리기
        score = 0;
        ScoreUpdate();

        isOver = false;

        //게임오버패널끄기
        gameover.SetActive(false);

        //브금키기
        MngSound.instance.bgmPlayer.Play();

        NextCell();
    }

    /// <summary>
    /// GameOver
    /// 장외 콜라이더에 닿거나, 빨간 선에 몇초이상 닿을시 게임오버
    /// 게임오버되면 게임을 멈추고, 실패브금과 함께 나가기 버튼이 활성화
    /// </summary>
    public void GameOver()
    {
        if (!isOver)
        {
            return;
        }
      
        //패널 활성화
        gameover.SetActive(true);
        isOver = true;
        StartCoroutine(GameOverRoutine(MngSound.sfx.Over));
    }

    IEnumerator GameOverRoutine(MngSound.sfx sfx)
    {
        MngSound.instance.bgmPlayer.Stop();
        MngSound.instance.SfxPlay(sfx);

        //1.장면 안에 활성화 되어있는 모든 동글 가져오기
        Cell[] cells = FindObjectsOfType<Cell>();

        //2. 1번의 목록을 하나씩 접근해서 지우기
        for (int index = 0; index < cells.Length; index++)
        {
            cells[index].rigid.simulated = false;
            cells[index].transform.parent = recyclePool;
            cells[index].Hide();
        }
        yield return null;

        //저장용
        if (PlayerPrefs.HasKey("MaxScore")) {
            maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        }
        else
        {
            maxScore = score;
        }
        PlayerPrefs.SetInt("MaxScore", maxScore);
        PlayerPrefs.Save();

        Debug.Log( "저장완료"+ PlayerPrefs.GetInt("MaxScore"));

        yield return new WaitForSeconds(1.0f);
        StopAllCoroutines();
    }

    #region btn method
    public void btnGameOver()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void btnSetting()
    {
        setting.SetActive(true);

        //멈춤
        Time.timeScale = 0.0f;
    }

    public void btnResume()
    {
        setting.SetActive(false);

        //재활성화
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
                bestScoreText.text = score.ToString();
            }
            else
            {
                bestScoreText.text = tmpScore.ToString();
            }

        }
        else { return; }
        
    }

    public void NextCell()
    {
        //승리 조건 검색
        GameWin();
        //게임오버가되면 다음동글을 주지않음
        if (isOver)
        {
            return;
        }
        lastCell = GetCell();

        //초기화

        
        if (nextNum == -1) {
            lastCell.level = 0;
        }
        else
        {
            //next 를 main 으로 넘김 
            lastCell.level = nextNum;
        }
        nextNum = Random.Range(0, limitLevel);
        //예비이미지 전달 및 적용
        afterCell.GetComponent<SpriteRenderer>().sprite = SetafterImg(nextNum);

        //예비이미지 크기조절
        SizeUp(afterCell.transform, 0);

        //cell 위치 지정 및 실체화
        lastCell.transform.position = spawner.transform.position;

        //예비이미지도 위치 변경
        Vector3 pos = new Vector3((float)(0.2 * (lastCell.level + 1) + 0.2), 2.25f, 0);
        afterCell.transform.DOMove(pos, 0.2f);

        //Cell 활성화
        lastCell.gameObject.SetActive(true);

        //라인렌더러 활성화
        lineRenderer.enabled = true;

        //부모 변경
        lastCell.transform.parent = cellPool;

        //크기 변경
        lastCell.SizeUp(lastCell.level);

        //이미지 변경
        lastCell.SetImg(lastCell.level);

        StartCoroutine(WaitNext());
    }

    #region after method

    public void SizeUp(Transform trans, int level)
    {
        float scale = (float)(0.2 * (level + 1) + 0.2);
        trans.DOScale(new Vector3(scale, scale, 1), 0.25f);
    }

    public Sprite SetafterImg(int level)
    {
        string s = level.ToString("D2");
        return Resources.Load<Sprite>("character/" + s);
    }

    #endregion

    IEnumerator WaitNext()
    {
        while (lastCell != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.8f);
        NextCell();
    }

    public void DrawDownRay()
    {

        if(lastCell != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(lastCell.transform.position, Vector2.down);

            if(hit.collider != null)
            {
                LinePosUpdate(hit.point);
            }
            else
            {
                return;
            }
        }
    }

}
