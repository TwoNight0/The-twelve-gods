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

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CursorMove();
    }

    //�΋H������ ���� ���
    private void OnCollisionEnter2D(Collision2D collision)
    {
        MngSound.instance.SfxPlay(MngSound.sfx.Attach);
        if (collision.gameObject.tag == "Cell")
        {
            Cell other = collision.gameObject.GetComponent<Cell>();
            if (level == other.level && !isMerge && !other.isMerge && level < GameManager.instance.PubMaxLevel)
            {
                //��ü�� 
                isMerge = true;

                float myX = transform.position.x;
                float myY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                Vector3 pos = new Vector3(((myX + otherX) / 2), ((myY + otherY) / 2), 0);

                transform.DOMove(pos, 0.01f);

                //������ ��������
                other.Hide();

                //������ŬǮ�� �̵�
                other.transform.parent = GameManager.instance.recyclePool;

                //�ùķ���Ƽ�� ����
                other.rigid.simulated = false;

                //���� ������
                LevelUp();

                //��ü ��
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
        // cell �Ӽ� �ʱ�ȭ
        level = 0;
        isDrag = false;
        isMerge = false;

        // cell Ʈ������ �ʱ�ȭ
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 0);

        // cell ���� �ʱ�ȭ
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        colider.enabled = false;
    }

    void CursorMove()
    {
        if (isDrag)
        {
            //���콺 ��ġ�� ��ũ������ �������������� �ٲ� Ŀ�� ����
            Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            float leftBorder = -2.42f + transform.localScale.x / 2;
            float rightBorder = 2.42f - transform.localScale.x / 2;

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

        colider.enabled = true;
        //���� ��� �ٽ� ����
        rigid.simulated = true;
    }

    /// <summary>
    /// �������� ����� recycle pool�� �̵� 
    /// �̵��ϱ��� �ؾ��� ��, ��Ȱ��ȭ �� �����Ǻ���
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
        //������ �� ���صǴ� �����ӵ� ����
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
        //��ƼŬ �ý����� �̰ɷ� �����Ŵ

        ParticleSystem particle = GameManager.instance.effect.GetComponent<ParticleSystem>();
        particle.Play();
    }
    
}