using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    public Color[] bottleColors;
    public SpriteRenderer bottleMask;

    public AnimationCurve ScaleAndRotationCurve;
    public AnimationCurve fillAmountCurve;
    public AnimationCurve rotationSpeedCurve;

    public float rotationTime=1f;

    public float[] fillAmounts;
    public float[] rotationAngles;

    [Range(0,4)]
    public int numberOfColors = 4;
    public int rotationIndex = 0;

    public Color TopColor;
    public int numberOfTopColorLayers=1;
    
    public int numberOfColorsToAdd;

    public Transform leftRotation;
    public Transform rightRotation;
    public LineRenderer lineRenderer;
    
    public Transform chosenRotation;
    public float directionMultiplier=1.0f;

    public Vector3 originalPosition;
    public Vector3 startPosition;
    public Vector3 endPosition;

    // Start is called before the first frame update
    void Start()
    {
        bottleMask.material.SetFloat("_FillAmount", fillAmounts[numberOfColors]);
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        originalPosition = transform.position;
        UpdateColors();
        UpdateTopColors();
    }
    public void UpdateColors()
    {
        bottleMask.material.SetColor("_C1", bottleColors[0]);
        bottleMask.material.SetColor("_C2", bottleColors[1]);
        bottleMask.material.SetColor("_C3", bottleColors[2]);
        bottleMask.material.SetColor("_C4", bottleColors[3]);
    }
    
    public void UpdateTopColors()
    {
        if (numberOfColors != 0)
        {
            numberOfTopColorLayers = 1;
            TopColor = bottleColors[numberOfColors-1];
            
            if (numberOfColors == 4)
            {
                if (bottleColors[3].Equals(bottleColors[2]))
                {
                    numberOfTopColorLayers = 2;
                    if (bottleColors[2].Equals(bottleColors[1]))
                    {
                        numberOfTopColorLayers = 3;
                        if (bottleColors[1].Equals(bottleColors[0]))
                        {
                            numberOfTopColorLayers = 4;
                        }
                    }
                }
            }

            else if (numberOfColors == 3)
            {
                if (bottleColors[2].Equals(bottleColors[1]))
                {
                    numberOfTopColorLayers = 2;
                    if (bottleColors[1].Equals(bottleColors[0]))
                    {
                        numberOfTopColorLayers = 3;
                    }      
                }
            }

            else if (numberOfColors == 2)
            {
                if (bottleColors[1].Equals(bottleColors[0]))
                {
                    numberOfTopColorLayers = 2;
                } 
            }

            rotationIndex = 3 - (numberOfColors - numberOfTopColorLayers);
        }
    }
    
    public void CalculateRotationIndex(int numberOfEmptySpacesInSecondBottle)
    {
        rotationIndex = 3 - (numberOfColors -
                             Mathf.Min(numberOfEmptySpacesInSecondBottle, numberOfTopColorLayers));
    }

    public void FillUpTheBottle(float fillAmountToAdd)
    {
        bottleMask.material.SetFloat("_FillAmount", bottleMask.material.GetFloat("_FillAmount") + fillAmountToAdd);
    }

    public bool CheckBottleFilled(Color color)
    {
        if (numberOfColors != 0)
        {
            if (numberOfColors == 4)
            {
                return false;
            }
            else{
                if(TopColor.Equals(color))
                {
                    return true;
                }
                else{
                    return false;
                }
            }
        }
        else
        {
            return true;
        }
    }
    
    public void ChoseRotationAndDirection(Bottle SecondBottle)
    {
        if (transform.position.x>SecondBottle.transform.position.x)
        {
            chosenRotation = leftRotation;
            directionMultiplier = -1.0f;
        }
        else
        {
            chosenRotation = rightRotation;
            directionMultiplier = 1.0f;
        }
    }
}
