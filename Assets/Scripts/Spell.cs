﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(300)]
public class Spell : MonoBehaviour
{
    public MoveHeinz player;
    public float elapsedTime;
    public float coolDownTime;
    public bool going = true;
    public bool buff = false;//spell that boosts player stats or not
    public float damage = 0;
    public string opposing;
    public string origin;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("paris").GetComponent<MoveHeinz>();
        elapsedTime = 0;
        StartStuff();
    }

    public virtual void SetPlayer(MoveHeinz boi){
        player = boi;
        origin = boi.gameObject.tag;
        opposing = (origin == "Player")?"Enemy":"Player";
    }

    public virtual void PreStartStuff(){
        coolDownTime = 0;
    }

    public virtual void StartStuff(){

    }

    // Update is called once per frame
    public virtual void LateUpdate()
    {
        
    }

    public virtual void UseEffect(){
        
    }

    public virtual void StopEffect(){

    }

    public virtual void UseEffectEnemy(GameObject enemy){
        
    }

    public virtual bool NewEffectValid(MoveHeinz other, float timeSinceUse){
        return (other.currSpell == null||!other.currSpell.GetComponent<Spell>().going)&&timeSinceUse>coolDownTime;
    }

    public virtual bool EffectValid(MoveHeinz other, float timeSinceUse){
        return timeSinceUse>coolDownTime;
    }
}
