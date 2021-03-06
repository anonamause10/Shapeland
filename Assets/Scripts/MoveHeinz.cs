﻿using UnityEngine;
using System;
using System.Collections;

[DefaultExecutionOrder(100)]
public class MoveHeinz : MonoBehaviour {
	//public vars
	//testcomment
	public CharInput charInput;

	public float walkSpeed = 15;
	public float runSpeed = 30;
	protected int gravity = 20;
	public float flySpeed = 100;
	//some move stuff
	protected float turnSmoothTime = 0.2f;
	protected float turnSmoothVelocity;
	protected float turnSmoothVelocityY;
	public bool walking = false;

	public float totalHealth = 20;
	public float health;
	protected float dps = 0;
    protected float dpsTime = 0;
    protected bool poisoned = false;
	public bool dead = false;

	//move and jump stuff
	protected float speedSmoothTime = 0.1f;
	protected float force = 40f;
	public float speedMult = 1.0f;
	protected float speedMultDamp = 0;
	protected float targetSpeed;
	public float currentSpeed;
	protected float speedSmoothVelocity = 1f;
	protected float moveMode;
	protected float moveModeVelocity = 1f;
	protected int knockback = 0;
	protected float knockbackdamp;
	protected float knockbackspeed = 20;
	protected Vector2 knockbackdirection;
	protected Vector3 projVec;//vector representing projectiles direction from transform.position
	protected Vector3 forwardVec;
    public Vector3 moveVec;
	public Vector3 accelVec;
	public Vector3 forceVelVec;
	protected Vector2 inputDir;
	protected Vector2 inputSmoothDamp;
	public Transform centerPoint;
	protected float jumpTimeCounter;
	protected float jumpTime=1;
	protected bool isJumping;
	public bool flying = false;
	public bool outFly = false;
	protected int jumpmode;

	//attack stuff
	public bool isAttacking = false;
	public bool attackingPrev;
	protected float attackFrameCounter;
	protected Transform arm;
	protected Transform forearm;
	protected Transform hand;
	public GameObject wandTip;
	public bool attackMode = false;
	public bool attackValidPrev;
	public bool valid;
	public event Action OnDrawPadSwitch;
	protected float armturnvel = 1f;//dampener for next variable
	protected float armdeg = 0;//arm rotation on y axis
	protected float armprev = 0;//previous value of armdeg
	protected float armturnYvel = 1f;//dampener for next variable
	protected float armdegY = 0;//arm rotation on x axis
	protected int armTurnFrameCounter;
	public RaycastHit hit;
	public GameObject spell;
	public GameObject currSpell;
	public GameObject drawPadObj;
	private Vector3 drawMouseTracker;
	public int spellIndex;
	protected String[] spells;
	public float[] timeSinceUse;
	protected bool shielding = false;
	public GameObject currShield;
	protected GameObject shield;
	protected int layermasknum;
	public bool drawPad = false;
	public float timeSpeed =1.0f;
	protected float timeSpeedDamp;

	//controller stuff
	protected CharacterController controller;
	protected Animator animator;
	protected Rigidbody rbody;
	public Transform cameraT;

	public virtual void Start () {
		charInput = GetComponent<CharInput>();
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator> ();
		rbody = GetComponent<Rigidbody>();
		arm = transform.Find("Armature").Find("Bone").Find("Bone.007");
		forearm = arm.Find("Bone.008");
		hand = forearm.Find("Bone.009");
		cameraT = charInput.cameraT;
		spells = new String[]{"FlameSpell", "DeathSpell","BaseBoltSpell","BaseBoltHeavySpell","FireBoltCharge","FreezeSpell"};
		timeSinceUse = new float[spells.Length];
		for (int i = 0; i < timeSinceUse.Length; i++)
		{
			timeSinceUse[i] = 5000;
		}
		spell = (GameObject)Resources.Load("Prefabs/" + spells[spellIndex]);
		spell.GetComponent<Spell>().PreStartStuff();
		shield = (GameObject)Resources.Load("Prefabs/DefaultShield");
		health = totalHealth;
		
		layermasknum = ~((1<<(gameObject.layer))|(1<<(gameObject.layer+3)));
		Cursor.lockState = CursorLockMode.Locked;
		//animator.speed = 0.5f;
	}

	public virtual void LateUpdate () {
		Physics.Raycast(cameraT.position, cameraT.forward, out hit, Mathf.Infinity, layermasknum);
		HandleDamage();
		HandleCooldownTimers();
		attackingPrev = isAttacking;
		charInput.CollectInputs();
		animator.speed = speedMult/Mathf.Pow(Vector2Extension.averageComponentLength(transform.localScale),0.2f);
		Knockback();
		if(flying){
			fly();
		}else{
			jump();
			//if(knockback==0){
				move();
			//}else{
			//	KnockbackMove();
			//}
		}		
		attack();

		if(gameObject.tag=="Player"){
			HandleTimeScale(timeSpeed);
		}

		
		launchAttack();
		
		//Debug.DrawRay(transform.position,forceVelVec*100,Color.red);
	}

	public virtual void move(){
		//walking
		speedMult = Mathf.SmoothDamp(speedMult, 1, ref speedMultDamp, 15f);
		if(knockback==0&&!dead){
			Vector2 input = charInput.inputDir;
			inputDir = Vector2.SmoothDamp(inputDir, input.normalized, ref inputSmoothDamp, !controller.isGrounded?turnSmoothTime:turnSmoothTime*1f*((currentSpeed<5?5:currentSpeed)/runSpeed));
			if (inputDir != Vector2.zero || animator.GetInteger("attack")==2||attackMode) {
				float targetRotation = Mathf.Atan2 (inputDir.x*(((attackMode&&charInput.inputDir.y<0)?-1:1)), (attackMode)?Mathf.Abs(inputDir.y):inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
				if((input.magnitude==0&&controller.isGrounded&&attackMode)||drawPad){
					targetRotation = cameraT.eulerAngles.y;
				}
				transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, (controller.isGrounded?turnSmoothTime:turnSmoothTime*1f*((currentSpeed<5?5:currentSpeed)/runSpeed)));
			}
			walking = charInput.walking;


			targetSpeed = ((walking) ? walkSpeed : runSpeed) * inputDir.magnitude;
			currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, controller.isGrounded?speedSmoothTime:5f);
		}

		if(controller.isGrounded){
			moveVec = speedMult*((((attackMode&&!outFly)?(Quaternion.AngleAxis(cameraT.eulerAngles.y, Vector3.up)*new Vector3(inputDir.x,0,inputDir.y)):transform.forward) * currentSpeed))+(transform.up*moveVec.y)+forceVelVec/Vector2Extension.averageComponentLength(transform.localScale);
		}
		
		//movement handling
	
		accelVec.y = -gravity;
        moveVec+=accelVec*Time.deltaTime;
		if(controller.isGrounded&&moveVec.y<-30f){
			moveVec.y = -30f;
		}
		controller.Move((moveVec) * Time.deltaTime);
		forceVelVec = Vector3.Lerp(forceVelVec,Vector3.zero,5*Time.deltaTime);
		float animationSpeedPercent = ((!walking) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
		float targetMoveMode = attackMode?1:0;
		moveMode = Mathf.SmoothDamp(moveMode,targetMoveMode,ref moveModeVelocity,0.1f);
		animator.SetInteger("knockback", knockback);
		animator.SetFloat("moveMode", moveMode);
		animator.SetFloat("movespeed", animationSpeedPercent);
		animator.SetFloat("moveX", inputDir.x*animationSpeedPercent);
		animator.SetFloat("moveY", inputDir.y*animationSpeedPercent);
		//Debug.DrawRay(transform.position+(3*Vector3.up),(new Vector3(moveVec.x,0,moveVec.z))*100, Color.blue);
	}

	/*public virtual void KnockbackMove(){
		moveVec = (transform.up*moveVec.y)+(dead&&controller.isGrounded?0:1)*-1*knockbackspeed*(knockback!=0? new Vector3(knockbackdirection.x,0,knockbackdirection.y):Vector3.zero);
		accelVec.y = -gravity;
        moveVec+=accelVec*Time.deltaTime;
		controller.Move(moveVec* Time.deltaTime);
		animator.SetInteger("knockback", knockback);
	}*/

	public virtual void jump(){
		//jumping
		if(controller.isGrounded&&charInput.jumping){
			moveVec.y = 0;
			isJumping = true;
			jumpTimeCounter = jumpTime;
			moveVec += transform.up*3.0f;
		}
        if(charInput.jumping&&isJumping){
			if(jumpTimeCounter>0){
				moveVec+=transform.up*force*jumpTimeCounter*Time.deltaTime;
				jumpTimeCounter -= Time.deltaTime;
			}else{
				isJumping = false;
			}
        }
		if(charInput.getJumpUp()){
			isJumping = false;
		}
		if(controller.isGrounded){
			jumpmode = 0;
			outFly = false;
		}else{
			if(Time.timeScale>0.95f){
				jumpmode = (jumpTimeCounter>0.75)?1:2;
			}
		}
		animator.SetInteger("jump",jumpmode);
		if((charInput.getJumpDown())&&!controller.isGrounded&&Time.timeScale>0.95f){
			flying = true;
			isJumping = true;
		}
	}

	public virtual void fly(){
		outFly = false;

		if (currentSpeed > 5 || charInput.leftMouseDown) {
			transform.forward = Vector3.SmoothDamp(transform.forward, Quaternion.AngleAxis(-25f*currentSpeed/flySpeed, transform.right)*cameraT.forward, ref moveVec, 0.03f);
		}

		float targetSpeed = ((charInput.inputDir.y+1)/2f)*flySpeed;
		
		currentSpeed = Mathf.SmoothDamp (currentSpeed, charInput.inputDir.y==0?currentSpeed:targetSpeed, ref speedSmoothVelocity, 1f);
        moveVec = (transform.forward * currentSpeed);

		if(charInput.getJumpDown()&&Time.timeScale>0.95f){
			flying = false;
			transform.eulerAngles = Vector3.up*transform.eulerAngles.y;
			outFly = true;
		}
		controller.Move(moveVec* Time.deltaTime);
		animator.SetInteger ("jump", 2);
	}

	public virtual void attack(){
		isAttacking = false;
		if(charInput.getSwitchSpellDown()){
			spellIndex = (spellIndex+1)%spells.Length;
			spell = (GameObject)Resources.Load("Prefabs/" + spells[spellIndex]);
			spell.GetComponent<Spell>().PreStartStuff();
		}
		if(charInput.spawnBoi){
			Instantiate((GameObject)Resources.Load("Prefabs/Enemies/Enemy"),(hit.distance!=0?hit.point:(cameraT.position+cameraT.forward*100)),Quaternion.identity);
		}
		if(charInput.shield&&!drawPad){
			shielding = true;
			if(currShield==null){
				currShield = Instantiate(shield,wandTip.transform.position,Quaternion.identity);
				currShield.GetComponent<Shield>().SetPlayer(this);
			}
			animator.SetInteger("shield",1);
		}else{
			shielding = false;
			if(currShield!=null){
				currShield.GetComponent<Shield>().kill();
			}
			animator.SetInteger("shield",0);
		}
		if(charInput.getDrawPadDown()){
			timeSpeed = timeSpeed==0?1:0;
			drawPad = !drawPad;
			if(OnDrawPadSwitch!=null){
				OnDrawPadSwitch();
			}
			if(drawPad){
				drawPadObj = Instantiate((GameObject)Resources.Load("Prefabs/DrawSpellCaster"),transform.position+3*transform.up, Quaternion.LookRotation(cameraT.forward));
				drawMouseTracker = new Vector3(Screen.width/2,Screen.height/2,Camera.main.nearClipPlane);
			}else{
				if(drawPadObj!=null){
					drawPadObj.GetComponent<DrawSpellCaster>().kill();
				}
			}
		}
		//attacking
		valid = attackValid();
		if((charInput.leftMouseDown&&valid)){
			isAttacking = true;
			animator.SetInteger("attack",2);
			attackFrameCounter=-1;
		}
		if(!charInput.leftMouseDown&&charInput.leftMouseDownPrev||!valid){
			animator.SetInteger("attack",0);
			if((!charInput.leftMouseDown&&charInput.leftMouseDownPrev&&valid)||(!valid&&attackValidPrev)){
				attackFrameCounter = 7;
			}
			isAttacking = false;
		}
		if(!isAttacking&&attackFrameCounter>0){
			attackFrameCounter-=1;
		}
		if(charInput.getSwitchAttackDown()){
			attackMode = !attackMode;
			animator.SetBool("scope", attackMode);
		}
		if((isAttacking||attackFrameCounter>0||attackMode||shielding)){
			if(!drawPad){
				armdegY = Mathf.SmoothDampAngle(armdegY,!flying?(cameraT.eulerAngles.x+(attackMode?-20*(targetSpeed/runSpeed):0)):0, ref armturnYvel, 0.1f);
				arm.RotateAround(arm.position,transform.right,armdegY);
			}else{
				armdegY += (-charInput.mouseY);
				arm.RotateAround(arm.position,transform.right,armdegY);	
			}
		}
		if(drawPad){
			drawMouseTracker.x += 20*charInput.mouseX;
			rotateArm(Camera.main.ScreenToWorldPoint(new Vector3(drawMouseTracker.x,charInput.mousePosition.y,Camera.main.nearClipPlane))-cameraT.position,transform.forward,false);
		}else if(attackMode||isAttacking){
			rotateArm(cameraT.forward, transform.forward,true);
		}
		attackValidPrev = valid;
	}

	public virtual void ApplyForce(Vector3 force){
		forceVelVec = new Vector3(force.x,Mathf.Abs(force.y),force.z);
		//print(forceVelVec);
	}

	public virtual void SetKnockbackDirection(Vector3 projPos,float magnitude){
		projVec = Vector3.ProjectOnPlane(projPos-transform.position,Vector3.up);
		forwardVec = Vector3.ProjectOnPlane(transform.forward,Vector3.up);
		float dotProduct = Vector3.Dot(projVec,forwardVec);
		int num = dotProduct>0?UnityEngine.Random.Range(1,4):4;
		if(num!=-1&&knockback==0){
			knockback = num;
			knockbackspeed = magnitude;
			knockbackdamp = 0;
		}
		if(magnitude<50){
			transform.forward = (dotProduct>0?1:-1)*projVec;
		}
		ApplyForce(magnitude*(centerPoint.position-projPos));
	}

	public virtual void Knockback(){
		knockbackdirection = new Vector2(transform.forward.x,transform.forward.z);
		if(knockback!=0&&controller.isGrounded){
			if(knockback>7){
				return;
			}
			inputDir = Vector2.zero;
			if(knockback<5){
				knockbackspeed = Mathf.SmoothDamp(knockbackspeed,0,ref knockbackdamp,knockback==4?0.2f:0.1f);
				if(knockback<4){
					knockbackdirection*=-1;
				}else{
					
				}
			}else{
				knockbackspeed = Mathf.SmoothDamp(knockbackspeed,0,ref knockbackdamp,0.3f);
				if(knockback<6){
					knockbackdirection*=-1;
				}else{
				
				}
			}
			knockbackdirection *= knockbackspeed;
		}
		if(animator.IsInTransition(0)){
			knockback = 0;
		}
	}

	public virtual void launchAttack(){
		if(isAttacking){
			if(spell.GetComponent<Spell>().NewEffectValid(this,timeSinceUse[spellIndex])){
				currSpell = Instantiate(spell,wandTip.transform.position,Quaternion.identity) as GameObject;
				currSpell.GetComponent<Spell>().SetPlayer(this);
			}
			//spellLine.SetPosition(0,wandTip.transform.position);
			//spellLine.SetPosition(1,cameraT.position+cameraT.forward*100);
		}
	}

	public virtual void rotateArm(Vector3 forward, Vector3 inputDir, bool damp){
		//   1  0  7
		//    \ | /
		//     \|/
		//   2--0--6
		//     /|\
		//    / | \
		//   3  4  5
		int quad = 0;
		forward = Vector3.ProjectOnPlane(forward, Vector3.up);	
		float degree = Vector3.SignedAngle(inputDir, forward, Vector3.up);
		degree = degree<0?360+degree:degree;
		if((!(isAttacking||attackFrameCounter>0)||flying)&&Time.timeScale>0.95f){
			degree = 0;
		}
		quad = (int)degree/45;
		armdeg = damp?Mathf.SmoothDampAngle(armdeg, degree, ref armturnvel, 0.1f):degree;
		armdeg = armdeg<0?armdeg%-360:armdeg%360;
		if(quad>4&&armdeg>180){
			armdeg = -(360-armdeg);
		}
		if(quad<3.9f){
			arm.RotateAround(arm.position,transform.up,armdeg);
			hand.RotateAround(hand.position,transform.up,(0.4f*(armdeg)));
		}else{
			arm.RotateAround(arm.position,transform.up,(0.1f*(armdeg)));
			forearm.RotateAround(forearm.position,transform.up,(0.3f*(armdeg)));
			hand.RotateAround(hand.position,transform.up,(0.3f*(armdeg)));
		}

		armprev = (degree%360);

	}

	public virtual void HandleCooldownTimers(){
		for (int i = 0; i < timeSinceUse.Length; i++){
			if(i==spellIndex){
				if(attackingPrev&&!isAttacking){
					timeSinceUse[i] = 0;
				}
			}
			timeSinceUse[i]+=Time.deltaTime;
		}
	}

	public virtual bool attackValid(){
		Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
		Vector3 camforward = Vector3.ProjectOnPlane(cameraT.forward, Vector3.up);
		float degree = Vector3.SignedAngle(camforward, forward, Vector3.up);
		degree = degree<0?360+degree:degree;
		bool spellvalidity = attackingPrev?spell.GetComponent<Spell>().EffectValid(this,timeSinceUse[spellIndex]):spell.GetComponent<Spell>().NewEffectValid(this,timeSinceUse[spellIndex]);
		return !(degree>135&&degree<225)&&spell.GetComponent<Spell>().EffectValid(this,timeSinceUse[spellIndex])&&knockback==0&&!shielding&&Time.timeScale>0.95f;
	}

	public virtual bool MouseDown(){
		return charInput.leftMouseDown&&!charInput.leftMouseDownPrev;
	}

	public virtual bool AttackReady(){
		return !isAttacking;// && spell.NewEffectValid();
	}

	public virtual void HandleTimeScale(float timeScale){

		Time.timeScale = Mathf.SmoothDamp(Time.timeScale,timeScale,ref timeSpeedDamp,0.005f);//Mathf.Lerp(Time.timeScale,timeScale,50*Time.deltaTime);

	}

	public virtual void HandleDamage(){
		if(poisoned){
            if(dpsTime>0){
                health-=dps*Time.deltaTime;
                dpsTime-=Time.deltaTime;
            }else{
                poisoned = false;
            }
        }
		dead = health<0;
        if((dead)){
            kill();
        }
	}

	public virtual void poison(float amount,float time){
        dps = (0.04f*Mathf.Atan(200*(amount+dps)))/Time.deltaTime;
        dpsTime = 2.5f*Mathf.Atan(2000*(dpsTime+time));
        poisoned = true;
    }

	public virtual void kill(){
		if(currShield!=null){
            Destroy(currShield);
        }
		Destroy(gameObject);
	}

}