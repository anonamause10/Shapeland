using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharInput : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 inputDir;
    public MoveHeinz controllerScript;
    public bool leftMouseDown = false;
    public bool leftMouseDownPrev = false;
    public bool walking;
    public bool jumping = false;
    public bool jumpingPrev = false;
    public bool switchAttackMode = false;
    public bool switchAttackModePrev = false;
    public bool switchSpell = false;
    public bool switchSpellPrev = false;
    public bool drawPad = false;
    public bool drawPadPrev = false;
    public bool shield = false;
    public bool shieldPrev = false;
    public bool spawnBoi;
    public int knockback;
    public Transform cameraT;

    public virtual void Start(){
        cameraT = Camera.main.transform;
    }
    
    public virtual void CollectInputs(){
        leftMouseDownPrev = leftMouseDown;
        switchAttackModePrev = switchAttackMode;
        shieldPrev = shield;
        switchAttackMode = false;
        switchSpellPrev = switchSpell;
        drawPadPrev = drawPad;
        drawPad = false;
        jumpingPrev = jumping;
        jumping = Input.GetKey(KeyCode.Space);
        inputDir = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        leftMouseDown = Input.GetMouseButton(0);
        switchSpell = Input.GetKeyDown(KeyCode.Q);
        switchAttackMode = Input.GetMouseButton(2);
        drawPad = Input.GetKey(KeyCode.E);
        shield = Input.GetMouseButton(1);
        knockback = GetPressedNumber();
        walking = Input.GetKey (KeyCode.LeftShift);
        spawnBoi = Input.GetKeyDown(KeyCode.T);
    }

    public bool getJumpDown(){
        return jumping&&!jumpingPrev;
    }

    public bool getJumpUp(){
        return !jumping&&jumpingPrev;
    }

    public bool getSwitchAttackDown(){
        return switchAttackMode&&!switchAttackModePrev;
    }

    public bool getSwitchAttackUp(){
        return switchAttackMode&&switchAttackModePrev;
    }

    public bool getSwitchSpellDown(){
        return switchSpell&&!switchSpellPrev;
    }

    public bool getSwitchSpellUp(){
        return !switchSpell&&switchSpellPrev;
    }

    public bool getDrawPadDown(){
        return drawPad&&!drawPadPrev;
    }

    public bool getDrawPadUp(){
        return !drawPad&&drawPadPrev;
    }

    public int GetPressedNumber() {

    	for (int number = 0; number <= 9; number++) {
    	    if (Input.GetKeyDown(number.ToString()))
    	        return number;
    	}
 
    	return -1;
	}

}
