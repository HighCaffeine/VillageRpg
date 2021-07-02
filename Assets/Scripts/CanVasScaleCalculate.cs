using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScaleCalculate : MonoBehaviour
{
    public float widthRatio;
    public float heightRatio;

    public CanvasScaler canvasScaler;

    public List<Transform> needToScaleCalculateTransformList;

    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        ScaleCalculate(Screen.width, Screen.height);

        if (widthRatio > heightRatio)
        {
            canvasScaler.matchWidthOrHeight = heightRatio / widthRatio;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = widthRatio / heightRatio;
        }


        //for (int i = 0; i < needToScaleCalculateTransformList.Count; i++)
        //{
        //    needToScaleCalculateTransformList[i].localScale = new Vector3(multiWidthValue * needToScaleCalculateTransformList[i].localScale.x,
        //                                                                needToScaleCalculateTransformList[i].localScale.y);
        //}
    }

    private void ScaleCalculate(float width, float height)
    {
        float minPixelValue;
        float maxPixelValue;
        float temp;

        if (width > height)
        {
            minPixelValue = height;
            maxPixelValue = width;
        }
        else
        {
            minPixelValue = width;
            maxPixelValue = height;
        }

        while (maxPixelValue % minPixelValue != 0)
        {
            temp = maxPixelValue % minPixelValue;
            maxPixelValue = minPixelValue;
            minPixelValue = temp;
        }

        widthRatio = width / minPixelValue;
        heightRatio = height / minPixelValue;
    }
}
