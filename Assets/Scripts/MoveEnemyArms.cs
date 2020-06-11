using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemyArms : MonoBehaviour
{
    public float xOffset=1;
    public float yOffset;
    public float zOffset;
    private Transform leftArm;
    private Transform rightArm;
    // Start is called before the first frame update
    void Start()
    {
        rightArm = transform.Find("Armature").Find("Bone").Find("Bone.007");
        leftArm = transform.Find("Armature").Find("Bone").Find("Bone.001");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 lOffset;
        Vector3 rOffset;
        lOffset = transform.localScale.x*xOffset*Vector3.Cross(transform.forward, transform.up).normalized;
        rOffset = transform.localScale.x*xOffset*Vector3.Cross(transform.up, transform.forward).normalized;
        lOffset -= transform.localScale.y*transform.up*yOffset;
        rOffset -= transform.localScale.y*transform.up*yOffset;
        lOffset += transform.localScale.z*transform.forward*zOffset;
        rOffset += transform.localScale.z*transform.forward*zOffset;
        leftArm.transform.position += lOffset;
        rightArm.transform.position += rOffset;
    }
}
