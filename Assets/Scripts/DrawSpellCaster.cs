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
    void Start()
    {
        playerScript = GameObject.Find("paris").GetComponent<MoveHeinz>();
        transform.position = playerScript.cameraT.position+2*playerScript.cameraT.forward;
        cursor = transform.Find("Cursor");
        cursor.localScale = Vector3.zero;
        trail = cursor.Find("Trail").gameObject.GetComponent<TrailRenderer>();
        planeRend = GetComponent<Renderer>();
        planeRend.material.color = new Color(0,0,0,0);
    }

    // Update is called once per frame
    void LateUpdate()
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
        bonkTimer+=0.1f;
    }

    void deadUpdate(){
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
}
