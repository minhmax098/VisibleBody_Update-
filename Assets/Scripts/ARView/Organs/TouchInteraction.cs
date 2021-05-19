using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TouchInteraction : MonoBehaviour
{
    private Touch touch;
    private float rotationSpeed = 0.5f;
    private bool isDragging = false;
  
    private Touch firstFinger;
    private Touch secondFinger;
    private float initialFingersDistance;
    private Vector3 initialScale;
    private float currentFingersDistance;
    private float scaleFactor;

    public ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();

    void Update()
    {
        if (Input.touchCount < 1 || ModelSeparation.Instance.IsHighlighting)
        {
            return;
        }
        
        if(Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                isDragging = Helper.IsTouchOnCurrentOrgan(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (isDragging)
                {
                    if (arRaycastManager.Raycast(touch.position, arRaycastHits, TrackableType.PlaneWithinPolygon))
                    {
                        // AROrganManager.Instance.SetOrganPosition(arRaycastHits[0].pose);
                    }
                }
                else
                {
                    AROrganManager.Instance.ChangeOrganRotation(new Vector3(0, -touch.deltaPosition.x * rotationSpeed, 0));
                }
            }  
        }
        else if(Input.touches.Length == 2)
        {
            // Scale object
            firstFinger = Input.touches[0];
            secondFinger = Input.touches[1];
           
            if (firstFinger.phase == TouchPhase.Began || secondFinger.phase == TouchPhase.Began)
            {
                initialFingersDistance = Vector2.Distance(firstFinger.position, secondFinger.position);
                initialScale = AROrganManager.Instance.GetLocalScaleOfCurrentOrganObject();
            }
            else if(firstFinger.phase == TouchPhase.Moved || secondFinger.phase == TouchPhase.Moved)
            {
                currentFingersDistance = Vector2.Distance(firstFinger.position, secondFinger.position);
                scaleFactor = currentFingersDistance / initialFingersDistance;
                AROrganManager.Instance.ChangeOrganScale(initialScale * scaleFactor);
            }
        }
    }
}