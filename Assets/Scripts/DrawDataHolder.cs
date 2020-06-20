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
        if(x+1<size) texture.SetPixel(x+1,y,col);
        if(y+1<size) texture.SetPixel(x,y+1,col);
        if(x+1<size&&y+1<size) texture.SetPixel(x+1,y+1,col);
        texture.Apply();
        numPoints++;

        if(x<bounds.x) bounds.x = x;
        if((x+1<size?x+1:x)>bounds.y) bounds.y = x+1<size?x+1:x;
        if(y<bounds.z) bounds.z = y;
        if((y+1<size?y+1:y)>bounds.w) bounds.w = y+1<size?y+1:y;
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
                if(i<pad||i>targetSize-pad-1||j<pad||j>targetSize-pad-1){
                    result.SetPixel(i,j,Color.black);
                }else{
                    result.SetPixel(i,j,temp.GetPixelBilinear(((float)i-pad)/(targetSize-1-pad),((float)j-pad)/(targetSize-1-pad)));
                }
            }
        }
        result.Apply();

        return result;
    }


}
