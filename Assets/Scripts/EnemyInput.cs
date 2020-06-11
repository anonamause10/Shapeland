using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : CharInput
{

    public GameObject centerPoint;
    public GameObject player;
    public MoveHeinz playerScript;
    public MoveHeinz thisScript;
    public Vector3 vecToPlayer;
    public Vector3 vecToPlayerPrev;
    public float sightRadius = 30f;
    public float minPlayerRadius = 10f;

    public override void Start(){    
        player = GameObject.Find("paris");
        playerScript = player.GetComponent<MoveHeinz>();
        cameraT = centerPoint.transform;
        thisScript = GetComponent<MoveHeinz>();
    }

    public override void CollectInputs(){
        inputDir= Vector2.zero;
        vecToPlayerPrev = vecToPlayer;
        leftMouseDownPrev = leftMouseDown;
        leftMouseDown = false;
        walking = false;
        switchAttackModePrev = switchAttackMode;
        switchAttackMode = false;
        vecToPlayer = playerScript.centerPoint.position-centerPoint.transform.position;
        if(!!!controllerScript.dead){
            cameraT.forward = vecToPlayer;
        }
        if(controllerScript.attackMode&&!controllerScript.dead){
            if(controllerScript.hit.distance!=0&&hitLayerIsEnemy(controllerScript.hit.collider.gameObject.layer)){
                leftMouseDown = true;
            }
            print(thisScript.centerPoint.position);
            Debug.DrawRay(centerPoint.transform.position, vecToPlayer*10000);
            //Vector3 projectedInputDir = Vector3.ProjectOnPlane(vecToPlayer,Vector3.up);
            if(Vector2Extension.XzPlaneMagnitude(vecToPlayer)>minPlayerRadius+0.5f){
                inputDir = Vector2.up;
            }else{
                inputDir = Vector2.Lerp(Vector2.down,Vector2.zero,Vector2Extension.XzPlaneMagnitude(vecToPlayer)/minPlayerRadius);
            }
            //Vector2Extension.Rotate(new Vector2(projectedInputDir.x,projectedInputDir.z),cameraT.eulerAngles.y);
        }
        if(Vector2Extension.XzPlaneMagnitude(vecToPlayer)<0.5*sightRadius){
            walking = true;
        }
        if(vecToPlayer.magnitude<sightRadius){
            controllerScript.attackMode = true;
        }
    }

    public bool hitLayerIsEnemy(int otherlayer){
        int enemyLayer = gameObject.layer==8?9:8;
        return otherlayer == enemyLayer||otherlayer == enemyLayer+3;
    }

}
