using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;
public class Shoot
{
    //static prefab_controll prefab_controll;
    public GameObject ball;  //**
    private GameObject ballClone;
    static prefab_controll Newball;
    //public GameObject meter;
    //public GameObject arrow;
    public GameObject gameOver;  //**
    public GameObject camera;
    private Vector3 throwSpeed = new Vector3(0, 15, 3); //This value is a sure basket
    public Vector3 ballPos; //**
    public TextMeshPro  availableShotsGO;
    private int availableShots = 5;
    private float arrowSpeed = 0.3f; //Difficulty
    private bool right = true;

    public Shoot(GameObject ball_,GameObject gameOver_,Vector3 ballPos_){
        availableShotsGO = GameObject.Find("score").GetComponent<TextMeshPro>();
        camera = GameObject.Find("ARCamera");
        ball = ball_;
        gameOver = gameOver_;
        ballPos = ballPos_;
    }
    
    /*public void move_cursor()
    {
        if (arrow.transform.position.x < 4.7f && right)
        {
            arrow.transform.position += new Vector3(arrowSpeed, 0, 0);
        }
        if (arrow.transform.position.x >= 4.7f)
        {
            right = false;
        }
        if (right == false)
        {
            arrow.transform.position -= new Vector3(arrowSpeed, 0, 0);
        }
        if ( arrow.transform.position.x <= -4.7f)
        {
            right = true;
        }
    }*/
    public void shoot_ball(Vector3 velocity)
    {
        availableShots--;
        availableShotsGO.text = availableShots.ToString();

        //ballClone = GameObject.Instantiate(ball, ballPos, camera.transform.rotation) as GameObject;
        //ballClone = prefab_controll();
        ballClone = GameObject.Find("ball");
        //ballClone = (Shoot)ballClone.GetComponent("Shoot");
        ballClone.GetComponent(Shoot).ballPos;
        throwSpeed.y = throwSpeed.y + velocity.y/90;
        throwSpeed.z = throwSpeed.z + velocity.z/100;

        ballClone.GetComponent<Rigidbody>().AddForce(throwSpeed, ForceMode.Impulse);
        //camera.GetComponent<AudioSource>().Play();
        
    }
    public Tuple<bool,bool> remove_ball()
    {
        if (ballClone != null && ballClone.transform.position.y < -16)
        {
            GameObject.Destroy(ballClone);
            throwSpeed = new Vector3(0, 15, 3);//Reset perfect shot variable

            /* Check if out of shots */
            
            if (availableShots == 0)
            {
                //arrow.GetComponent<Renderer>().enabled = false;
                //GameObject.Instantiate(gameOver, new Vector3(0.31f, 0.2f, 0), camera.transform.rotation);
                return Tuple.Create(true,true);
            }
            return Tuple.Create(true,false);
        }
        return Tuple.Create(false,false);
    }
    
}