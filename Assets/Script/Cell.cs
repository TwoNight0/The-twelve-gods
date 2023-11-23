using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameManager manager;
    public ParticleSystem effect;
    public int level;


    //bool
    public bool isDrag;
    public bool isMerge;
    public bool isAttach;

    public Rigidbody2D rigid;
    CircleCollider2D circle;
    Animator anim;
    SpriteRenderer spriteRenderer;
    float deadTime;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //level setting

    void CursorMove() {
        if (isDrag)
        {
            //���콺 ��ġ�� ��ũ������ �������������� �ٲ� Ŀ�� ����
            Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            float leftBorder = -4.2f + transform.localScale.x / 2;
            float rightBorder = 4.2f - transform.localScale.x / 2;

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }

            mousePos.y = 5.5f;
            mousePos.z = 0f;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
        }
    }


    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        //���� ��� �ٽ� ����
        rigid.simulated = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(AttachRouutine());
    }

    IEnumerator AttachRouutine()
    {
        if (isAttach)
        {
            yield break;
        }
        isAttach = true;
        //manager.SfxPlay(GameManager.sfx.Attach);
        yield return new WaitForSeconds(0.2f);
        isAttach = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Dongle")
        {
            //Dongle other = collision.gameObject.GetComponent<Dongle>();

            //if (level == other.level && !isMerge && !other.isMerge && level < 11)
            //{
            //    float myX = transform.position.x;
            //    float myY = transform.position.y;
            //    //float otherX = other.transform.position.x;
            //    //float otherY = other.transform.position.y;

            //    // 1. ���� �Ʒ�
            //    // 2. ������ ������ ��, ���� �����ʿ� ���� ��
            //    if (myY < otherY || (myY == otherY && myX > otherX))
            //    {
            //        //������ ��������
            //        other.Hide(transform.position);
            //        //���� ������
            //        LevelUp();
            //    }
            //}

        }
    }

    /// <summary>
    /// �������� ����� recycle pool�� �̵� 
    /// �̵��ϱ��� �ؾ��� ��, ��Ȱ��ȭ �� �����Ǻ���
    /// </summary>
    /// <param name="targetPos"></param>
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        //����ȿ�� ���ֱ�
        rigid.simulated = false;
        circle.enabled = false;

        if (targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }

        StartCoroutine(HideRoutine(targetPos));
    }

    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
        while (frameCount < 20)
        {
            frameCount++;
            if (targetPos != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            }
            else if (targetPos == Vector3.up * 100)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }

            yield return null;
        }
        manager.score += (int)Mathf.Pow(2, level);

        isMerge = false;
        gameObject.SetActive(false);

    }

    private void LevelUp()
    {
        isMerge = true;

        //������ �� ���صǴ� �����ӵ� ����
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        anim.SetInteger("Level", level + 1);
        EffectPlay();
        //manager.SfxPlay(GameManager.sfx.LevelUp);

        yield return new WaitForSeconds(0.3f);
        level++;

        //���� ū ����ȯ
        manager.maxLevel = Mathf.Max(level, manager.maxLevel);
        isMerge = false;
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
                //manager.GameOver();
            }
        }
    }

    void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        //��ƼŬ �ý����� �̰ɷ� �����Ŵ
        effect.Play();
    }
}
