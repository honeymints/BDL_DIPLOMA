using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    private int numberOfColorsToAdd;
    
    public void StartColorTransferring(Bottle SecondBottle, Bottle FirstBottle)
    {
        FirstBottle.ChoseRotationAndDirection(SecondBottle);
        numberOfColorsToAdd = Mathf.Min(FirstBottle.numberOfTopColorLayers, 4-SecondBottle.numberOfColors);
        for (int i=0;i<numberOfColorsToAdd;i++)
        {
            SecondBottle.bottleColors[SecondBottle.numberOfColors + i] = FirstBottle.TopColor;
        }
        
        SecondBottle.UpdateColors();
        
        FirstBottle.CalculateRotationIndex(4-SecondBottle.numberOfColors);

        FirstBottle.GetComponent<SpriteRenderer>().sortingOrder += 2;
        FirstBottle.bottleMask.sortingOrder += 2;
        
        StartCoroutine(MoveBottle(FirstBottle, SecondBottle));
    }
    
    IEnumerator RotateBottle(Bottle FirstBottle, Bottle SecondBottle)
    {
        float t = 0;
        float lerp;
        float eulerAngle;
        
        float lastEulerAngle=0;
        
        while (t<FirstBottle.rotationTime)
        {
            lerp = t / FirstBottle.rotationTime;
            eulerAngle = Mathf.Lerp(0f, FirstBottle.directionMultiplier*FirstBottle.rotationAngles[FirstBottle.rotationIndex], lerp);
            t += Time.deltaTime*FirstBottle.rotationSpeedCurve.Evaluate(eulerAngle);

            FirstBottle.transform.RotateAround(FirstBottle.chosenRotation.position, Vector3.forward, lastEulerAngle-eulerAngle);
            FirstBottle.bottleMask.material.SetFloat("_ScaleAndRotation", FirstBottle.ScaleAndRotationCurve.Evaluate(eulerAngle));

            if (FirstBottle.fillAmounts[FirstBottle.numberOfColors] > FirstBottle.fillAmountCurve.Evaluate(eulerAngle)+0.005f)
            {
                if (FirstBottle.lineRenderer.enabled == false)
                {
                    FirstBottle.lineRenderer.startColor = FirstBottle.TopColor;
                    FirstBottle.lineRenderer.endColor = FirstBottle.TopColor.linear;
                    FirstBottle.lineRenderer.SetPosition(0, FirstBottle.chosenRotation.position);
                    FirstBottle.lineRenderer.SetPosition(1,FirstBottle.chosenRotation.position-Vector3.up*1.45f);

                    FirstBottle.lineRenderer.enabled = true;
                }
                FirstBottle.bottleMask.material.SetFloat("_FillAmount",FirstBottle.fillAmountCurve.Evaluate(eulerAngle));
                
                SecondBottle.FillUpTheBottle(FirstBottle.fillAmountCurve.Evaluate(lastEulerAngle)-FirstBottle.fillAmountCurve.Evaluate(eulerAngle));
            }

            t += Time.deltaTime * FirstBottle.rotationSpeedCurve.Evaluate(eulerAngle);
            lastEulerAngle = eulerAngle;
            yield return new WaitForEndOfFrame();
        }

        eulerAngle = FirstBottle.directionMultiplier*FirstBottle.rotationAngles[FirstBottle.rotationIndex];
        FirstBottle.bottleMask.material.SetFloat("_ScaleAndRotation", FirstBottle.ScaleAndRotationCurve.Evaluate(eulerAngle));
        FirstBottle.bottleMask.material.SetFloat("_FillAmount",FirstBottle.fillAmountCurve.Evaluate(eulerAngle));

        FirstBottle.numberOfColors -= numberOfColorsToAdd;
        SecondBottle.numberOfColors += numberOfColorsToAdd;

        FirstBottle.lineRenderer.enabled = false;
        
        StartCoroutine(RotateBottleBackToPlace(FirstBottle));
    }

    IEnumerator MoveBottle(Bottle FirstBottle, Bottle SecondBottle)
    {
        FirstBottle.startPosition = FirstBottle.transform.position;

        if (FirstBottle.chosenRotation == FirstBottle.leftRotation)
        {
            FirstBottle.endPosition = SecondBottle.rightRotation.position;
        }
        else
        {
            FirstBottle.endPosition = SecondBottle.leftRotation.position;
        }

        float t = 0;
        
        while(t<=1)
        {
            transform.position = Vector3.Lerp(FirstBottle.startPosition, FirstBottle.endPosition, t);
            t += Time.deltaTime*2;
            yield return new WaitForEndOfFrame();
        }

        FirstBottle.transform.position = FirstBottle.endPosition;
        
        StartCoroutine(RotateBottle(FirstBottle, SecondBottle));
    }

    IEnumerator MoveBottleBack(Bottle FirstBottle)
    {
        FirstBottle.startPosition = FirstBottle.transform.position;
        FirstBottle.endPosition = FirstBottle.originalPosition;
        
        float t = 0;
        while(t<=1)
        {
            FirstBottle.transform.position = Vector3.Lerp(FirstBottle.startPosition, FirstBottle.endPosition, t);
            t += Time.deltaTime*2;
            yield return new WaitForEndOfFrame();
        }

        FirstBottle.transform.position = FirstBottle.endPosition;
        
        FirstBottle.GetComponent<SpriteRenderer>().sortingOrder -= 2;
        FirstBottle.bottleMask.sortingOrder -= 2;
    }
    
    IEnumerator RotateBottleBackToPlace(Bottle FirstBottle)
    {
        float t = 0;
        float lerp;
        float eulerAngle;

        float lastEulerAngle = FirstBottle.directionMultiplier * FirstBottle.rotationAngles[FirstBottle.rotationIndex];
        
        while (t<FirstBottle.rotationTime)
        {
            lerp = t / FirstBottle.rotationTime;
            eulerAngle = Mathf.Lerp(FirstBottle.directionMultiplier * FirstBottle.rotationAngles[FirstBottle.rotationIndex], 0f, lerp);
            t += Time.deltaTime;

            //transform.eulerAngles = new Vector3(0, 0, eulerAngle);
            FirstBottle.transform.RotateAround(FirstBottle.chosenRotation.position, Vector3.forward, lastEulerAngle-eulerAngle);
            
            FirstBottle.bottleMask.material.SetFloat("_ScaleAndRotation", FirstBottle.ScaleAndRotationCurve.Evaluate(eulerAngle));

            lastEulerAngle = eulerAngle;
            
            yield return new WaitForEndOfFrame();
        }
        
        FirstBottle.UpdateTopColors();
        
        eulerAngle = 0;
        FirstBottle.transform.eulerAngles = new Vector3(0, 0, eulerAngle);
        FirstBottle.bottleMask.material.SetFloat("_ScaleAndRotation", FirstBottle.ScaleAndRotationCurve.Evaluate(eulerAngle));

        StartCoroutine(MoveBottleBack(FirstBottle));
    }

}
