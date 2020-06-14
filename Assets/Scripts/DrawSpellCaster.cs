using System.Collections;
using System.Collections.Generic;
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
        timeMult = Mathf.Atan(bonkTimer);
        cursor.localScale = 0.05f*timeMult*Vector3.one;
        planeRend.material.color = new Color(0,0,0,timeMult/Mathf.PI);


        trail.emitting = playerScript.charInput.leftMouseDown&&Mathf.Abs(cursor.localPosition.x)<0.8&&Mathf.Abs(cursor.localPosition.y)<0.8;
        transform.position = playerScript.cameraT.position+2*playerScript.cameraT.forward;
        
        cursor.Translate(sensitivity*(new Vector3(playerScript.charInput.mouseX,playerScript.charInput.mouseY,0)));
        if(trail.emitting){
            int xArr = Mathf.Clamp((int)Mathf.Round(((cursor.localPosition.x +0.8f)/1.6f)*28),0,27);
            int yArr = Mathf.Clamp((int)Mathf.Round(((cursor.localPosition.y +0.8f)/1.6f)*28),0,27);
            array[xArr,yArr] = 1;
        }
        bonkTimer+=0.1f;
        //print(cursor.localPosition);
    }

    void deadUpdate(){
        clearArray(array);
        timeMult = Mathf.Atan(bonkTimer);
        cursor.localScale = 0.05f*timeMult*Vector3.one;
        planeRend.material.color = new Color(0,0,0,timeMult/Mathf.PI);

        trail.material.SetFloat("_Density",(Mathf.PI/2)-timeMult);

        float padSize = (Mathf.PI/2)-timeMult+1;
        transform.localScale = padSize*Vector3.one;

        transform.position = playerScript.cameraT.position+2*playerScript.cameraT.forward;
        
        cursor.Translate(sensitivity*(new Vector3(playerScript.charInput.mouseX,playerScript.charInput.mouseY,0)));
        bonkTimer-=0.2f;
        if(bonkTimer<0){
            Destroy(gameObject);
        }
    }

    public void kill(){
        dead = true;
        bonkTimer = 5;
        trail.emitting = false;
    }

    public void printArray(float[,] arr){
        string str = "";
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                str += arr[i,j];
                if(j==arr.GetLength(1)-1){
                    str += "\n";
                }
            }
        }
        print(str);
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
