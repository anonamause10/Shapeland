 using UnityEngine;
 
 public static class Vector2Extension {
     
    public static Vector2 Rotate(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static float XzPlaneMagnitude(Vector3 v){
        return Mathf.Sqrt(v.x*v.x+v.z*v.z);
    }

    public static float averageComponentLength(Vector3 v){
        return (v.x+v.y+v.z)/3;
    }
 }