using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveEnemy : MoveHeinz
{
    public Slider healthBarSlider;
    protected float healthBarDamp = 0;

    public override void Start(){
        spellIndex = Random.Range(1,4);
        base.Start();
        charInput = (EnemyInput)charInput;
        charInput.controllerScript = this;
    }

    public override void HandleDamage(){
        healthBarSlider.value = Mathf.SmoothDamp(healthBarSlider.value, health/totalHealth, ref healthBarDamp, 0.1f);
        if(poisoned){
            if(dpsTime>0){
                health-=dps;
                dpsTime-=Time.deltaTime;
            }else{
                poisoned = false;
            }
        }
		dead = health<0||transform.position.y<-10;
        if((dead)){
            kill();
        }
    }

    public override void kill(){
        knockback = Vector3.Dot(projVec,forwardVec)>0?8:9;
        animator.SetInteger("knockback", Vector3.Dot(projVec,forwardVec)>0?8:9);
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Knockback.dead")||transform.position.y<-10){
            if(currShield!=null){
                Destroy(currShield);
            }
			Destroy(gameObject);
		}

    }
}
