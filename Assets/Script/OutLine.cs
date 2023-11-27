using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLine : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //isOver = true
        GameManager.instance.PubOver = true;
        GameManager.instance.GameOver();
    }
}
