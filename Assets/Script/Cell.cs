using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cell : MonoBehaviour
{
    public int level;

    //bool
    public bool isDrag;
    public bool isMerge;
    public bool isAttach;

    [HideInInspector] public Rigidbody2D rigid;
    CircleCollider2D colider;

    SpriteRenderer spriteRenderer;
    float deadTime;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CursorMove();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(AttachRouutine());
        }
        
    }

    IEnumerator AttachRouutine()
    {
        if (isAttach)
        {
            yield break;
        }
        isAttach = true;
        MngSound.instance.SfxPlay(MngSound.sfx.Attach);
        yield return new WaitForSeconds(0.2f);
        isAttach = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime += Time.deltaTime;
            if (deadTime > 2.0f && deadTime < 5.0f)
            {
                spriteRenderer.color = new Color(0.9f, 0.2f, 0.2f);
            }
            if (deadTime > 5.0f)
            {
                GameManager.instance.GameOver();
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Cell")
        {
            Cell other = collision.gameObject.GetComponent<Cell>();

            if (level == other.level && !isMerge && !other.isMerge && level < GameManager.instance.PubMaxLevel)
            {
                float myX = transform.position.x;
                float myY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                Vector3 pos = new Vector3(((myX + otherX) / 2), ((myY + otherY) / 2), 0);

                transform.DOMove(pos, 0.01f);

                //상대방은 숨겨지고
                other.Hide(transform.position);

                //리사이클풀로 이동
                other.transform.parent = GameManager.instance.recyclePool;

                //나는 레벨업
                LevelUp();

            }
        }
    }

    private void OnDisable()
    {
        // cell 속성 초기화
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;

        // cell 트랜스폼 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1,1,0);

        // cell 물리 초기화
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        //콜라이더 다시키기
        colider.enabled = true;
    }

    private void OnEnable()
    {
        rigid = GetComponent<Rigidbody2D>();
        colider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void CursorMove()
    {
        if (isDrag)
        {
            //마우스 위치를 스크린에서 월드포지션으로 바꿔 커서 역할
            Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            float leftBorder = -2.1f + transform.localScale.x / 2;
            float rightBorder = 2.1f - transform.localScale.x / 2;

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }

            mousePos.y = 2.25f;
            mousePos.z = 0f;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.5f);
        }
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        //물리 계산 다시 시작
        rigid.simulated = true;
    }

    /// <summary>
    /// 합쳐지면 숨기고 recycle pool로 이동 
    /// 이동하기전 해야할 것, 비활성화 및 포지션변경
    /// </summary>
    /// <param name="targetPos"></param>
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        rigid.simulated = false;
        colider.enabled = false;
        isMerge = false;
        gameObject.SetActive(false);

    }

    public void ScoreUp()
    {
        GameManager.instance.PubScore += (level+1) * 10;
    }

    private void LevelUp()
    {
        isMerge = true;

        //레벨업 중 방해되는 물리속도 제거
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        ScoreUp();
        GameManager.instance.ScoreUpdate();
        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        EffectPlay();
        MngSound.instance.SfxPlay(MngSound.sfx.LevelUp);

        yield return null;
        level++;

        //둘중 큰 값반환
        GameManager.instance.PubMaxLevel = Mathf.Max(level, GameManager.instance.PubMaxLevel);
        SizeUp(level);
        SetImg(level);
        isMerge = false;
    }

    public void SizeUp(int level)
    {
        float scale = (float)(0.2 * (level+1) + 0.2);
        transform.DOScale(new Vector3(scale, scale, 1), 0.25f);
    }

    public void SetImg(int level)
    {
        string s = level.ToString("D2");
        spriteRenderer.sprite = Resources.Load<Sprite>("character/" + s);
    }


    public void EffectPlay()
    {
        GameManager.instance.effect.transform.position = transform.position;
        GameManager.instance.effect.transform.localScale = transform.localScale;
        //파티클 시스템은 이걸로 실행시킴

        ParticleSystem particle = GameManager.instance.effect.GetComponent<ParticleSystem>();
        particle.Play();
    }
    
}