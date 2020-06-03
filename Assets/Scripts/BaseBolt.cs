using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBolt : Spell
{
    public float velocity=50;
    public float timer = 5f;
    private GameObject explosion;
    private GameObject release;
    public bool attackingDone;
    public bool isReflected = false;

    public override void StartStuff(){
        //Physics.IgnoreCollision(player.gameObject.GetComponent<Collider>(), GetComponent<Collider>(), bool ignore = true);
        transform.forward = (player.hit.distance!=0?player.hit.point:player.cameraT.position+player.cameraT.forward*300)-player.wandTip.transform.position;
        
        float randX = Random.Range(-0.3f, 0.3f);
        float randY = Random.Range(-0.3f, 0.3f);

        transform.forward = Quaternion.AngleAxis(randX, transform.right) * transform.forward;
        transform.forward = Quaternion.AngleAxis(randY, transform.up) * transform.forward;
        
        damage = 2f;

        explosion = (GameObject)Resources.Load("Prefabs/BaseBoltRelease");
        release = (GameObject)Resources.Load("Prefabs/BaseBoltRelease");
        Instantiate(explosion, transform.position, Quaternion.LookRotation(transform.forward));
    }

    public override void LateUpdate()
    {
        UseEffect();
        if(!player.isAttacking){
            player.currSpell = null;
        }

    }

    public override void UseEffect(){

        transform.Translate(velocity*Time.deltaTime*transform.forward,Space.World);  
        timer -= Time.deltaTime;
        if(timer<0){
            StopEffect();
        }  
    }

    public override void StopEffect(){
        Destroy(gameObject);
    }

    public override void UseEffectEnemy(GameObject enemy){
        enemy.GetComponent<MoveHeinz>().health-=damage;
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Shield"){
            Shield shield = other.gameObject.GetComponent<Shield>();
            Vector3 shieldEffect = shield.DoParryEffect(this);
            if(shieldEffect!=Vector3.zero){
                transform.forward = shieldEffect;
                return;
            }
        }
        Instantiate(release, other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position), Quaternion.LookRotation(Vector3.up)); 
        if(other.gameObject.tag == origin||other.gameObject.tag == "Spell"){
            return;
        }
        if(other.gameObject.tag == opposing){
            UseEffectEnemy(other.gameObject);
        }
        StopEffect();
    }


 
}
