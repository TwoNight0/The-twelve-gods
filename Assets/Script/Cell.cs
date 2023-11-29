using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cell : MonoBehaviour
{
    public int level;

    //bool
    public bool isDrag = false;
    public bool isMerge = false;

    [HideInInspector] public Rigidbody2D rigid;
    CircleCollider2D colider;

    SpriteRenderer spriteRenderer;

    //부딫혔을때 사운드 재생
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //MngSound.instance.SfxPlay(MngSound.sfx.Attach);
        if (collision.gameObject.tag == "Cell")
        {
            Cell other = collision.gameObject.GetComponent<Cell>();
            if (level == other.level && !isMerge && !other.isMerge && level < GameManager.instance.PubMaxLevel)
            {
                //합체중 
                isMerge = true;

                float myX = transform.position.x;
                float myY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                Vector3 pos = new Vector3(((myX + otherX) / 2), ((myY + otherY) / 2), 0);

                transform.DOMove(pos, 0.01f);

                //상대방은 숨겨지고
                other.Hide();

                //리사이클풀로 이동
                other.transform.parent = GameManager.instance.recyclePool;

                //시뮬레이티드 끄기
                other.rigid.simulated = false;

                //나는 레벨업
                LevelUp();

                //합체 끝
                isMerge = false;

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
                //합체중 
                isMerge = true;

                float myX = transform.position.x;
                float myY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                Vector3 pos = new Vector3(((myX + otherX) / 2), ((myY + otherY) / 2), 0);

                transform.DOMove(pos, 0.01f);

                //상대방은 숨겨지고
                other.Hide();

                //리사이클풀로 이동
                other.transform.parent = GameManager.instance.recyclePool;

                //시뮬레이티드 끄기
                other.rigid.simulated = false;

                //나는 레벨업
                LevelUp();

                //합체 끝
                isMerge = false;

            }
        }
    }

    private void OnEnable()
    {
        rigid = GetComponent<Rigidbody2D>();
        colider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CellClear()
    {
        // cell 속성 초기화
        level = 0;
        isDrag = false;
        isMerge = false;

        // cell 트랜스폼 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 0);

        // cell 물리 초기화
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        colider.enabled = false;
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        GameManager.instance.lineRenderer.enabled = false;
        colider.enabled = true;
        //물리 계산 다시 시작
        rigid.simulated = true;
    }

    /// <summary>
    /// 합쳐지면 숨기고 recycle pool로 이동 
    /// 이동하기전 해야할 것, 비활성화 및 포지션변경
    /// </summary>
    /// <param name="targetPos"></param>
    public void Hide()
    {
        CellClear();
        gameObject.SetActive(false);
    }

    public void ScoreUp()
    {
        GameManager.instance.PubScore += (level+1) * 10;
    }

    private void LevelUp()
    {
        //레벨업 중 방해되는 물리속도 제거
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        ScoreUp();
        GameManager.instance.ScoreUpdate();

        EffectPlay();
        MngSound.instance.SfxPlay(MngSound.sfx.LevelUp);
        if (level < 11)
        {
            level++;
            SizeUp(level);
            SetImg(level);
        }
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