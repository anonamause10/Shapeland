using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : Boom
{
    
    private ParticleSystem ps;
    private Renderer rend;

    public override void StartStuff(){
        ps = GetComponent<ParticleSystem>();
    }
    
    public override void Update()
    {
        if(aliveTime<3&&!ps.isStopped){
            ps.Stop();
        }
        if(dying){
            aliveTime-=Time.deltaTime;
        }
        if(aliveTime<0){
            Destroy(gameObject);
        }
    }

    public void OnTriggerStay(Collider other){
        if(other.gameObject.tag=="Effect"){
            Destroy(gameObject);
        }
    }
}
