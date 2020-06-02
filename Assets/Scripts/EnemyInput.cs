using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : CharInput
{

    public GameObject centerPoint;
    public GameObject player;
    public MoveHeinz playerScript;
    public Vector3 vecToPlayer;
    public Vector3 vecToPlayerPrev;
    public float sightRadius = 30f;

    public override void Start(){    
        player = GameObject.Find("paris");
        playerScript = player.GetComponent<MoveHeinz>();
        cameraT = centerPoint.transform;
    }

    public override void CollectInputs(){
        inputDir= Vector2.zero;
        vecToPlayerPrev = vecToPlayer;
        leftMouseDownPrev = leftMouseDown;
        leftMouseDown = false;
        switchAttackModePrev = switchAttackMode;
        switchAttackMode = false;
        vecToPlayer = player.transform.position-transform.position;
        cameraT.forward = vecToPlayer;
        if(controllerScript.attackMode){
            if(controllerScript.hit.distance!=0&&controllerScript.hit.collider.gameObject.tag == "Player"){
                leftMouseDown = true;
            }
            //Vector3 projectedInputDir = Vector3.ProjectOnPlane(vecToPlayer,Vector3.up);
            inputDir = Vector2.up;//Vector2Extension.Rotate(new Vector2(projectedInputDir.x,projectedInputDir.z),cameraT.eulerAngles.y);
        }
        if(vecToPlayer.magnitude<sightRadius){
            controllerScript.attackMode = true;
        }
    }

}
