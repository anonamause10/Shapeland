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
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(dying){
            kill();
            return;
        }
        transform.position = playerScript.wandTip.transform.position;
        transform.forward = playerScript.transform.forward;
        density = Mathf.Lerp(density,0.4f,2*Time.deltaTime);
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

    void OnTriggerEnter(Collider other){

    }

    public void kill(){
        dying = true;
        density = Mathf.Lerp(density,0,2*Time.deltaTime);
        rend.material.SetFloat("_Density", density);
        if(dieTime<0){
            Destroy(gameObject);
        }
        dieTime-=Time.deltaTime;
    }


}
