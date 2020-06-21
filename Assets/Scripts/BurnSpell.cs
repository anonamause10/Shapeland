using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnSpell : Spell
{
    public ParticleSystem beamLine;
    private List<ParticleCollisionEvent> collisionEvents;
    

    public override void StartStuff(){
        damage = 0.2f;
        beamLine = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    public override void LateUpdate()
    {
        if(player.isAttacking&&going){
            UseEffect();
        }else if(!player.isAttacking){
            StopEffect();
        }

    }

    public override void UseEffect(){

        transform.position = player.wandTip.transform.position;
        transform.forward = (player.hit.distance!=0?player.hit.point:player.cameraT.position+player.cameraT.forward*100)-player.wandTip.transform.position;
        
		if(!beamLine.isEmitting){
			beamLine.Play();
		}
    }

    public override void StopEffect(){
        going = false;
        if(!beamLine.isStopped){
			beamLine.Stop();
		}
        if(!beamLine.IsAlive()){
            Destroy(gameObject);
        }
    }

    public override void UseEffectEnemy(GameObject enemy){
        enemy.GetComponent<MoveHeinz>().health-=damage;
    }

    public void OnParticleCollision(GameObject other)
    {
        if(other.tag == opposing&&other.GetComponent<MoveHeinz>()!=null){
			UseEffectEnemy(other);
            return;
		}
        /*
        if(other.tag == "Effect"){
            return;
        }
        
        int numEvents = beamLine.GetCollisionEvents(other, collisionEvents);
        print(numEvents);
        
        if(collisionEvents[0].colliderComponent.gameObject.tag=="Effect"){
            return;
        }
        if (other.tag == "Terrain")
        {
            Collider[] colls = Physics.OverlapSphere(collisionEvents[0].intersection,0.8f);
            foreach (Collider coll in colls)
            {
                if(coll.tag=="Effect"){
                    return;
                }    
            }
            Instantiate((GameObject)Resources.Load("Prefabs/MediumFlames"),collisionEvents[0].intersection,Quaternion.Euler(-90,0,0));
        }*/
        
    }
    
 
}
