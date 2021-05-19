using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ModelSeparation : MonoBehaviour
{
    private Touch touch;
    public float duration = 1.5f;
    private float touchTime;
    private bool isCheckingLongTouch = true;
    private bool ableToMoveObject = false;

    public GameObject SelectedObject;
    public bool IsHighlighting { get; set; }

    private int redColor;
    private int greenColor;
    private int blueColor;
    private bool flashIn = false;
    private Color32 originalColor;
    private Vector3 originalSelectedPosition;
    private Vector3 originalDeltaRotation;
    private Vector3 currentSelectedPosition;

    private static ModelSeparation instance;
    public static ModelSeparation Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ModelSeparation>();
            }
            return instance;
        }   
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        KeepHightLightingObject ();
    
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
        
            if (touch.phase ==  TouchPhase.Began)
            {
                InitForLongTouchChecking();
            }
            if (touch.phase == TouchPhase.Stationary && isCheckingLongTouch)
            {
                touchTime += Time.deltaTime;
                if (touchTime >= duration)
                {
                    SelectedObject = Helper.GetChildObjectInSpaceOnTouchByTag(touch.position, ObjectTag.organTag);
                    // check if touch on organ
                    if (SelectedObject != null)
                    {
                        isCheckingLongTouch = false;
                        StartHightLightingObject();
                    }
                }
            }
            if (touch.phase == TouchPhase.Moved)
            {
                // Move then Stationary is not accepted
                touchTime = 0;
                isCheckingLongTouch = false;
                if (ableToMoveObject)
                {
                    currentSelectedPosition = new Vector3 (touch.position.x, touch.position.y, originalSelectedPosition.z);
                    currentSelectedPosition = Camera.main.ScreenToWorldPoint(currentSelectedPosition);
                    MoveObject(currentSelectedPosition);
                }
            }

            if (touch.phase == TouchPhase.Ended && IsHighlighting)
            {
               StopHightLightingObject();
            }
        }

        if (Input.touchCount == 3)
        {
            UnifyObject();
        }
    }

    void KeepHightLightingObject()
    {
        if (IsHighlighting && SelectedObject != null)
        {
            SelectedObject.GetComponent<Renderer>().material.color = new Color32((byte) redColor, (byte) greenColor, (byte) blueColor, 0);
        }
    }

    void InitForLongTouchChecking()
    {
        touchTime = 0;
        isCheckingLongTouch = true;
        ableToMoveObject = false;
        IsHighlighting = false;
    }

    public void StartHightLightingObject()
    {
        IsHighlighting = true;
        originalColor = SelectedObject.GetComponent<Renderer>().material.color;
        originalSelectedPosition = SelectedObject.transform.position;
        originalDeltaRotation = SelectedObject.transform.eulerAngles - SelectedObject.transform.parent.gameObject.transform.eulerAngles;
        StartCoroutine(FlashObject());
        ableToMoveObject = true;
    }

    public void MoveObject(Vector3 position)
    {
        if (ableToMoveObject) 
        {
            SelectedObject.transform.position = position;
            SelectedObject.transform.LookAt(Camera.main.transform.position);
        }
    }

    public void StopHightLightingObject()
    {
        IsHighlighting = false;
        StopCoroutine(FlashObject());
        SelectedObject.GetComponent<Renderer>().material.color = originalColor;
        ableToMoveObject = false;
    }

    public void UnifyObject()
    {
        StartCoroutine(Helper.MoveObject(SelectedObject, originalSelectedPosition));
        SelectedObject.transform.eulerAngles = SelectedObject.transform.parent.gameObject.transform.eulerAngles + originalDeltaRotation;
    }
    
    IEnumerator FlashObject() 
    {
        while (IsHighlighting)
        {
            yield return new WaitForSeconds(0.05f);
            if (flashIn)
            {
                if (greenColor < 70)
                {

                    flashIn = false;
                }
                else
                {
                    greenColor -= 25;
                    blueColor -=1;
                }
            }
            else
            {
                if (greenColor >= 250)
                {
                   flashIn = true;
                }
                else
                {
                    greenColor += 25;
                    blueColor +=1;
                }
            }
        }
    }
}
