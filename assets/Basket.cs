using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class Basket : MonoBehaviour
{
    
    public AudioClip basket;
    public int  score_int;
    public TextMeshPro score;

        void start()
    {
        
        score_int=0;
    }

    void OnCollisionEnter()
    {
        GetComponent<AudioSource>().Play();
    }

    void OnTriggerEnter()
    {   //score=Object.Find("/Text (TMP)");
        score=GameObject.Find("score").GetComponent<TextMeshPro>();
        score_int++;
        score.text=score_int.ToString();


        AudioSource.PlayClipAtPoint(basket, transform.position);
    }
}