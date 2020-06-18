using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDataHolder 
{
    public Texture2D texture;
    private int size;
    private Vector4 bounds;//left, right, bottom, top
    private int numPoints;

    public DrawDataHolder(int squareSize){
        size = squareSize;
        texture = new Texture2D(size,size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                texture.SetPixel(i,j,Color.black);
            }
        }
        texture.Apply();
        bounds = new Vector4(size-1,0,size-1,0);
    }

    public void SetPoint(int x, int y, float val){
        Color col = val*Color.white; col.a = 1;
        texture.SetPixel(x,y,col);
        texture.Apply();
        numPoints++;

        if(x<bounds.x) bounds.x = x;
        if(x>bounds.y) bounds.y = x;
        if(y<bounds.z) bounds.z = y;
        if(y>bounds.w) bounds.w = y;
    }

    

    public Texture2D resizedTexture(int targetSize, int pad){
        if(bounds == new Vector4(size-1,0,size-1,0)){
            return new Texture2D(targetSize, targetSize);
        }
        Texture2D result = new Texture2D(targetSize,targetSize);
        Texture2D temp = new Texture2D((int)(bounds.y-bounds.x)+1,(int)(bounds.w-bounds.z)+1);
        for (int i = (int)bounds.x; i <= (int)bounds.y; i++)
        {
            for (int j = (int)bounds.z; j <= (int)bounds.w; j++)
            {
                temp.SetPixel(i-(int)bounds.x,j-(int)bounds.z,texture.GetPixel(i,j));
            }
        }
        temp.Apply();

        for (int i = 0; i < targetSize; i++)
        {
            for (int j = 0; j < targetSize; j++)
            {
                result.SetPixel(i,j,temp.GetPixelBilinear(((float)i)/(targetSize-1),((float)j)/(targetSize-1)));
            }
        }
        result.Apply();

        return result;
    }


}
