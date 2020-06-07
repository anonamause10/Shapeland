using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(250)]
public class Shield : MonoBehaviour
{
    private float elapsedTime;
    private bool dying = false;
    private float dieTime = 0.5f;
    private float density;
    public MoveHeinz playerScript;
    private Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetPlayer(MoveHeinz player){
        playerScript = player;
        gameObject.layer = player.gameObject.layer+3;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //print(gameObject.layer);
        if(dying){
            kill();
            return;
        }
        transform.position = playerScript.wandTip.transform.position;
        transform.forward = playerScript.transform.forward;
        transform.RotateAround(transform.position,transform.forward,180);
        transform.RotateAround(transform.position,-1*transform.right,playerScript.cameraT.eulerAngles.x);
        density = Mathf.Lerp(density,0.35f,5*Time.deltaTime);
        rend.material.SetFloat("_Density", density);
        elapsedTime+=Time.deltaTime;
    }

    public Vector3 DoParryEffect(Spell other){
        other.SetPlayer(playerScript);
        if(elapsedTime<0.5){
            return transform.forward;
        }
        return Vector3.zero;
    }

    void OnCollisionStay(Collision other){
        if(other.gameObject.tag == (playerScript.tag=="Enemy"?"Player":"Enemy")){
            other.gameObject.GetComponent<MoveHeinz>().health-=10*Time.deltaTime;
            if(other.gameObject.GetComponent<Rigidbody>().isKinematic){
                other.gameObject.GetComponent<MoveHeinz>().SetKnockbackDirection(transform.position,5);
            }else{
                other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.forward*100);
            }
        }
    }

    public void kill(){
        if(!dying){
            GetComponent<Collider>().isTrigger = true;
        }
        dying = true;
        density = Mathf.Lerp(density,0,2*Time.deltaTime);
        rend.material.SetFloat("_Density", density);
        if(dieTime<0){
            Destroy(gameObject);
        }
        dieTime-=Time.deltaTime;
    }


}
