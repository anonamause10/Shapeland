using System.Collections;
using System.Collections.Generic;
using Barracuda;
using UnityEngine;

public class DrawSpellCaster : MonoBehaviour
{
    // Start is called before the first frame update
    public MoveHeinz playerScript;
    public Transform cursor;
    public TrailRenderer trail;
    public float sensitivity = 1f;
    public float bonkTimer;
    private float timeMult;
    private Renderer planeRend;
    private bool dead = false;
    private float[,] array;
    private Texture2D display; 
    private Vector2 prevPoint;
    private bool prevPress;
    private Vector2 currPoint;
    public NNModel modelSource;
    
    void Start()
    {
        array = new float[28,28];
        display = new Texture2D(array.GetLength(1),array.GetLength(0));
        playerScript = GameObject.Find("paris").GetComponent<MoveHeinz>();
        transform.position = playerScript.cameraT.position+2*playerScript.cameraT.forward;
        cursor = transform.Find("Cursor");
        cursor.localScale = Vector3.zero;
        trail = cursor.Find("Trail").gameObject.GetComponent<TrailRenderer>();
        planeRend = GetComponent<Renderer>();
        planeRend.material.color = new Color(0,0,0,0);

        int xArr = Mathf.Clamp((int)Mathf.Round(((cursor.localPosition.x +0.8f)/1.6f)*28),0,27);
        int yArr = Mathf.Clamp((int)Mathf.Round(((cursor.localPosition.y +0.8f)/1.6f)*28),0,27);
        currPoint = new Vector2(xArr,yArr);
    }

    // Update is called once per frame
    void Update()
    {
        if(!dead){
            aliveUpdate();
        }else{
            deadUpdate();
        }
    }

    void aliveUpdate(){
        prevPress = trail.emitting;
        if (prevPress) prevPoint.Set(currPoint.x,currPoint.y);
        timeMult = Mathf.Atan(bonkTimer);
        cursor.localScale = 0.1f*timeMult*Vector3.one;
        planeRend.material.color = new Color(0,0,0,timeMult/Mathf.PI);


        trail.emitting = playerScript.charInput.leftMouseDown&&Mathf.Abs(cursor.localPosition.x)<0.8f&&Mathf.Abs(cursor.localPosition.y)<0.8f;
        transform.position = playerScript.cameraT.position+2*playerScript.cameraT.forward;
        
        Vector3 cursorMove = new Vector3(playerScript.charInput.mouseX,playerScript.charInput.mouseY,0);
        if(Mathf.Abs(cursor.localPosition.x)>1.5f&&Mathf.Sign(playerScript.charInput.mouseX)==Mathf.Sign(cursor.localPosition.x)){
            cursorMove.x = 0;
        }
        if(Mathf.Abs(cursor.localPosition.y)>1.0f&&Mathf.Sign(playerScript.charInput.mouseY)==Mathf.Sign(cursor.localPosition.y)){
            cursorMove.y = 0;
        }
        cursor.Translate(sensitivity*cursorMove);


        int xArr = Mathf.Clamp((int)Mathf.Round(((cursor.localPosition.x +0.8f)/1.6f)*28),0,27);
        int yArr = Mathf.Clamp((int)Mathf.Round(((cursor.localPosition.y +0.8f)/1.6f)*28),0,27);
        currPoint.Set(xArr,yArr);
        print(currPoint);
        if(trail.emitting){
            array[(int)currPoint.x,(int)currPoint.y] = 1;
        }

        
        if(trail.emitting&&prevPress){
            if(!touchingPoints(currPoint,prevPoint)){
                fillInPoints((int)prevPoint.x,(int)prevPoint.y,(int)currPoint.x,(int)currPoint.y);
            }
        }
        


        bonkTimer+=0.1f;
        //print(cursor.localPosition);
    }

    void deadUpdate(){
        timeMult = Mathf.Atan(bonkTimer);
        cursor.localScale = 0.1f*timeMult*Vector3.one;
        planeRend.material.color = new Color(0,0,0,timeMult/Mathf.PI);

        trail.material.SetFloat("_Density",(Mathf.PI/2)-timeMult);

        float padSize = (Mathf.PI/2)-timeMult+1;
        transform.localScale = padSize*Vector3.one;

        transform.position = playerScript.cameraT.position+2*playerScript.cameraT.forward;
        
        bonkTimer-=0.2f;
        if(bonkTimer<0){
            Destroy(gameObject);
        }
    }

    public bool touchingPoints(Vector2 a, Vector2 b){
        Vector2 c = b-a;
        return Mathf.Abs(c.x)<=1&&Mathf.Abs(c.y)<=1;
    }

    public void fillInPoints(int x,int y,int x2, int y2) {
        int w = x2 - x ;
        int h = y2 - y ;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0 ;
        if (w<0) dx1 = -1 ; else if (w>0) dx1 = 1 ;
        if (h<0) dy1 = -1 ; else if (h>0) dy1 = 1 ;
        if (w<0) dx2 = -1 ; else if (w>0) dx2 = 1 ;
        int longest = Mathf.Abs(w) ;
        int shortest = Mathf.Abs(h) ;
        if (!(longest>shortest)) {
            longest = Mathf.Abs(h) ;
            shortest = Mathf.Abs(w) ;
            if (h<0) dy2 = -1 ; else if (h>0) dy2 = 1 ;
            dx2 = 0 ;            
        }
        int numerator = longest >> 1 ;
        for (int i=0;i<=longest;i++) {
            array[x,y] = 1;
            numerator += shortest ;
            if (!(numerator<longest)) {
                numerator -= longest ;
                x += dx1 ;
                y += dy1 ;
            } else {
                x += dx2 ;
                y += dy2 ;
            }
        }
    }

    public void kill(){
        dead = true;
        bonkTimer = 5;
        trail.emitting = false;
    }

    public float[,] clearArray(float[,] arr){
        float[,] temp = arr;
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                temp[i,j] = 0;
            }
        }
        return temp;
    }

    void OnGUI(){
        for (int i = 0; i < 28; i++)
        {
            for (int j = 0; j < 28; j++)
            {
                Color col = array[i,j]*Color.white;
                col.a = 1;
                display.SetPixel(i,j,col);
            }
        }
        display.Apply();
        GUI.DrawTexture(new Rect(10,10,28,28),display);
    }
}