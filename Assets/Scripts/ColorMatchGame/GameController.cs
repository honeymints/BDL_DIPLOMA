using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Bottle FirstBottle;
    [SerializeField] private Bottle SecondBottle;

    [SerializeField]
    private BottleController _bottleController;
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);
            Debug.Log(hit);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<Bottle>() != null)
                {
                    if (FirstBottle == null)
                    {
                        FirstBottle = hit.collider.GetComponent<Bottle>();
                    }
                    else
                    {
                        if (FirstBottle == hit.collider.GetComponent<Bottle>())
                        {
                            FirstBottle = null;
                        }
                        else
                        {
                            SecondBottle = hit.collider.GetComponent<Bottle>();
                            /*FirstBottle.SecondBottle = SecondBottle;*/
                            
                            FirstBottle.UpdateTopColors();
                            SecondBottle.UpdateTopColors();

                            if (SecondBottle.CheckBottleFilled(FirstBottle.TopColor))
                            {
                                _bottleController.StartColorTransferring(SecondBottle, FirstBottle);
                            }
                            
                            FirstBottle = null;
                            SecondBottle = null;
                            
                        }
                    }
                }
            }
        }
    }
}
