using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using System;

public class Cam
{
    //public WebCamTexture texture;
    private Point pre_center=new Point(0,0);
    const int directionPoints = 20;
    const int ksize = 8;
    private List<Point> numbers = new List<Point>();
    //byte[] bs;
    private int width;
    private int height;
    private Texture2D rt;
    private Tuple<Mat,Vector3,bool> detect_component = new Tuple<Mat,Vector3,bool>(new Mat(),new Vector3(),false);
    private Mat src1;
    private Mat src2;
    private Mat cv1;
    private Mat cv2;
    private Mat binary;
    private Mat kernel;
    private Mat hierarchly = new Mat(); 
    
    
    private List<string> direction=new List<string>() {" "," "};

    public Cam(int height_,int width_)
    {   
        //rawImage = RawImage.GetComponent<RawImage>();
        height = height_;
        width = width_;
        rt=new Texture2D(height,width,TextureFormat.RGBA32 ,false);
        src1 = new Mat(width,height,CvType.CV_8UC4);
        src2 = new Mat(width,height,CvType.CV_8UC4);
        cv1 = new Mat(width,height,CvType.CV_8UC4);
        cv2 = new Mat(width,height,CvType.CV_8UC4);
        binary = new Mat(width,height,CvType.CV_8UC4);
        kernel = new Mat(ksize,ksize,CvType.CV_8UC4);
        
        //TextureFormat.Alpha8
    }
    // Update is called once per frame
    public Tuple<Texture2D,Vector3,bool> Renew(Texture2D s1,Texture2D s2)
    {   
        detect_component=Detect(s1,s2);
        //Debug.Log(detect_component.Item2);
        //Debug.Log(detect_component.Item3);
        Utils.matToTexture2D(detect_component.Item1,rt);
        //rawImage.texture =rt;
        return Tuple.Create(rt,detect_component.Item2,detect_component.Item3);
    }
    //Tuple<Mat,Vector3,bool>
    private Tuple<Mat,Vector3,bool> Detect(Texture2D t1, Texture2D t2)
    {   
        //Mat mask = new Mat(height,width,CvType.CV_8UC4);
        bool movecheck=false;
        Vector3 velocity=Vector3.zero;
        List< MatOfPoint > contours = new List<MatOfPoint>();

        Utils.texture2DToMat(t1,src1);
        Utils.texture2DToMat(t2,src2);
        //src1 = Mat.FromImageData(t1.EncodeToPNG(), ImreadModes.Color);
        //src2 = Mat.FromImageData(t2.EncodeToPNG(), ImreadModes.Color);
        Imgproc.cvtColor(src1, cv1, Imgproc.COLOR_RGB2GRAY);
        Imgproc.cvtColor(src2, cv2, Imgproc.COLOR_RGB2GRAY);
        
        //Core.absdiff(cv1,cv2,d);
        Core.absdiff(cv1,cv2,binary);
        //return Tuple.Create(d,velocity,movecheck);
        Imgproc.GaussianBlur(binary,binary,new Size(7,7),0);
        
        Imgproc.threshold(binary, binary, 60, 255, Imgproc.THRESH_BINARY);
        
        
        Imgproc.erode(binary,binary,kernel);
        Imgproc.dilate(binary,binary,kernel);
        //return Tuple.Create(mask,velocity,movecheck);
        
        Imgproc.findContours(binary,contours,hierarchly,Imgproc.RETR_CCOMP, Imgproc.CHAIN_APPROX_SIMPLE);

        if (contours.Count>0){
            double max=-1;
            double area;
            int max_index=-1;
            Point center = new Point();

            for (int i =0;i<contours.Count;i++)
            {
                area=Imgproc.contourArea(contours[i]);
                //Debug.Log(area);
                if (area>max)
                {
                    max=area;
                    max_index=i;
                }
            }
            //Debug.Log(max);
            OpenCVForUnity.Rect box=new OpenCVForUnity.Rect();
            box = Imgproc.boundingRect(contours[max_index]);
            
            if (max>(width*height/10))
            {
                Imgproc.rectangle(binary,box,new Scalar (255,0,0),4);
                center.x = (int)(box.x+(box.width/2));
                center.y = (int)(box.y+(box.height/3));
                pre_center.x=(int)(box.x+(box.width/2));
                pre_center.y=(int)(box.y+(box.height/3));
                //Debug.Log("ok");
                //Debug.Log(width);
                //Debug.Log(height);
                numbers.Add(center);
                
                Debug.Log(center);
            }
            else
            {   
                if (pre_center!=null)
                {
                    //Debug.Log(pre_center.x);
                    //Debug.Log(pre_center.y);
                    numbers.Add(pre_center);
                }
                //Debug.Log(pre_center);
            }
        }
        else
        {   
            if (pre_center!=null)
            {
                //Debug.Log(pre_center.x);
                //Debug.Log(pre_center.y);
                numbers.Add(pre_center);
            }
            
            //Debug.Log(pre_center);
        }

        if (numbers.Count>directionPoints)
        {
            double dX;
            double dY;
            dX = (int)((numbers[numbers.Count-1].x - numbers[0].x)/(width/3));
            dY = (int)((numbers[numbers.Count-1].y - numbers[0].y)/(height/3));
            //Debug.Log(dX);
            //Debug.Log(dY);
                
            if (Math.Sign(dX)==-1){
                direction[0]="left";
            }
            else if (Math.Sign(dX)==1){
                direction[0]="right";
            }
            else{
                direction[0]=" ";
            }
            if (Math.Sign(dY)==1){
                direction[1]="bottom";
            }
            else if(Math.Sign(dY)==-1){
                direction[1]="top";
            }
            else{
                direction[1]=" ";
            }
            if (direction[0]!=" " && direction[1]!= " "){
                velocity.x=(float)(numbers[numbers.Count-1].x - numbers[0].x);
                velocity.y=(float)(numbers[numbers.Count-1].y - numbers[0].y);
                movecheck=true;
            }
            else if (direction[0]!=" " && direction[1]==" "){
                velocity.x=(float)(numbers[numbers.Count-1].x - numbers[0].x);
                movecheck=true;
            }
            else if (direction[0]==" " && direction[1]!=" "){
                velocity.y=(float)(numbers[numbers.Count-1].y - numbers[0].y);
                movecheck=true;
            }
            else{
                velocity.x=0;
                velocity.y=0;
            }
            numbers.RemoveAt(0);
        }
        //Imgproc.putText(src2,direction[0],new Point(200,250),Core.FONT_HERSHEY_SIMPLEX, 2,new Scalar (255,0,0),3);
        //Imgproc.putText(src2,direction[1],new Point(200,300),Core.FONT_HERSHEY_SIMPLEX, 2,new Scalar (255,0,0),3);
        //Imgproc.putText(src2,velocity.x.ToString(),new Point(200,150),Core.FONT_HERSHEY_SIMPLEX, 2,new Scalar (255,0,0),3);
        //Imgproc.putText(src2,velocity.y.ToString(),new Point(200,200),Core.FONT_HERSHEY_SIMPLEX, 2,new Scalar (255,0,0),3);
        
        //return src2;

        //Debug.Log(velocity);
        //Debug.Log(movecheck);
        return Tuple.Create(binary,velocity,movecheck);
    }
    
}
