using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using OpenCVForUnity;
using TMPro;
using System;

public class manager : MonoBehaviour
{
    // Start is called before the first frame update
    static Shoot shoot;
    static Cam cam;
    static WebCamTexture texture;   // camera
    public GameObject ball;
    public GameObject gameOver;
    public Vector3 ballPos;
    private Texture2D s1;
    private Texture2D s2;
    private int width;
    private int height;
    private bool thrown = false;
    //public GameObject RawImage;
    Tuple<Texture2D,Vector3,bool> result;
    Tuple<bool,bool> remove_result = new Tuple<bool,bool>(false,false);
    void Awake()
    { 
        
        /*for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            Debug.Log(WebCamTexture.devices[i].name);
            //如果是前置摄像机
            /*if (WebCamTexture.devices[i].isFrontFacing)
            {   
                texture=new WebCamTexture();
                texture.deviceName = WebCamTexture.devices[i].name;
                Debug.Log(i);
            }
        }*/

        if(texture==null)
        {
            texture=new WebCamTexture();
            texture.deviceName = WebCamTexture.devices[1].name;
        }   
        if (!texture.isPlaying){
            texture.Play();   
        }
        width=texture.width;
        height=texture.height;
        s1=new Texture2D(width,height,TextureFormat.RGBA32 ,false);
        s2=new Texture2D(width,height,TextureFormat.RGBA32 ,false);
        result = new Tuple<Texture2D,Vector3,bool>(new Texture2D(width,height,TextureFormat.Alpha8 ,false),new Vector3(0,0,0),false);
        //temp = new Texture2D(width,height,TextureFormat.RGBA32 ,false);

    }
    void Start()
    {   
        cam=new Cam(width,height);
        shoot=new Shoot(ball,gameOver,ballPos);
        //RawImage = GameObject.Find("RawImage");

        Physics.gravity = new Vector3(0, -20, 0);
        GetTexture2D(s1);
    }

    // Update is called once per frame
    void Update()
    {   
        //s2.Release();
        GetTexture2D(s2);
        result=cam.Renew(s1,s2);
        //RawImage.GetComponent<RawImage>().texture=result.Item1;
        
        //Debug.Log(result.Item1);
        //Debug.Log(result.Item2);
        if (thrown)
        {
            remove_result=shoot.remove_ball();
            if (remove_result.Item1)
            {
                thrown = false;
            }
            
            if (remove_result.Item1&&remove_result.Item2)
            {
                restart();
            }
            
        }
        if (result.Item3==true)
        {   
            if (!thrown)
            {   
                thrown = true;
                shoot.shoot_ball(result.Item2);
                
            }
        }
        //s1.Release();
        GetTexture2D(s1);
    }
    /*void FixedUpdate()
    {
    
    }*/
    void GetTexture2D(Texture2D t)
    {   
        t.SetPixels(texture.GetPixels());
        t.Apply();

    }
    void restart()
    {   
        SceneManager.LoadScene(0);
    }
}
