using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class GestureInteraction : MonoBehaviour
{   
    private HandInfo currentHandInfo;
    private GestureInfo currentGestureInfo;
    private TrackingInfo currentTrackingInfo;
    private TrackingInfo previousTrackingInfo;
    private Vector3 currentPamlCenter;
    private Vector3 previousPamlCenter;
    private Vector3 currentPoi;
    private Vector3 previousPoi;
    private float epsilonRange = 0.02f;
    private float xDistance;
    private float yDistance;
    private float rotationHandTrackingSpeed = 900f;
    private bool isScaling = false;
    private float scaleSpeed = 1.5f;   

    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 10f;

    private static GestureInteraction instance;
    public static GestureInteraction Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GestureInteraction>();
            }
            return instance;
        }
        
    }
    void Start()
    {      
        InitLaser();  
    }
    
    // Update is called once per frame
    void Update()
    {
        if (ManomotionManager.Instance.Hand_infos[0].hand_info.warning == Warning.WARNING_HAND_NOT_FOUND || ModelSeparation.Instance.IsHighlighting)
        {
            ResetScreen();
            return;
        }
        UpdateInfos(ManomotionManager.Instance.Hand_infos[0].hand_info);
        ControlByHandTracking();
    }

    void InitLaser()
    {
        Vector3[] initLaserPositions = new Vector3[ 2 ] { Vector3.zero, Vector3.zero };
        laserLineRenderer.SetPositions(initLaserPositions);
        laserLineRenderer.SetWidth(laserWidth, laserWidth);
    }

    void ResetScreen()
    {
        isScaling = false;
    }

    void UpdateInfos (HandInfo handInfo)
    {
        currentHandInfo = handInfo;
        currentGestureInfo = currentHandInfo.gesture_info;
        currentTrackingInfo = currentHandInfo.tracking_info;
    }
    void ControlByHandTracking()
    {
        if(currentGestureInfo.mano_gesture_continuous == ManoGestureContinuous.POINTER_GESTURE) 
        {
            ShootLaserFromTargetPosition(GetPointerPositionOnScreen(), Vector3.forward, laserMaxLength, ObjectTag.organTag);
            laserLineRenderer.enabled = true;
        }
        else 
        {
            laserLineRenderer.enabled = false;
        }

        if (currentGestureInfo.mano_gesture_trigger == ManoGestureTrigger.PICK && !isScaling)
        {
            isScaling = true;
        }
        else if (currentGestureInfo.mano_gesture_trigger == ManoGestureTrigger.DROP && isScaling)
        {
            isScaling = false;
        }
        else if (currentGestureInfo.mano_gesture_continuous == ManoGestureContinuous.HOLD_GESTURE && isScaling)
        {
            previousPoi = previousTrackingInfo.poi;
            currentPoi = currentTrackingInfo.poi;
            xDistance = currentPoi.x - previousPoi.x;
            AROrganManager.Instance.ScaleOrganByPercentage(1 + xDistance * scaleSpeed);
        }
        else if (currentGestureInfo.hand_side == HandSide.Backside && currentGestureInfo.mano_gesture_continuous == ManoGestureContinuous.OPEN_HAND_GESTURE)
        {
            previousPamlCenter = previousTrackingInfo.palm_center;
            currentPamlCenter = currentTrackingInfo.palm_center;
            xDistance = currentPamlCenter.x - previousPamlCenter.x;
            yDistance = currentPamlCenter.y - previousPamlCenter.y;
            if (Mathf.Abs(yDistance) < epsilonRange)
            {
                AROrganManager.Instance.ChangeOrganRotation(new Vector3(0,  xDistance * rotationHandTrackingSpeed, 0));
            } 
        } 
        previousTrackingInfo = currentTrackingInfo;
    }

    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length, string objectTag)
    {
        Vector3 endPosition = targetPosition + (length * direction);
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit[] raycastHits;
        raycastHits = Physics.RaycastAll(ray);
        foreach (RaycastHit raycastHit in raycastHits)
        {
            if(raycastHit.collider.tag == objectTag)
            {
                endPosition = raycastHit.point;
            }
        }
        laserLineRenderer.SetPosition(0, targetPosition);
        laserLineRenderer.SetPosition(1, endPosition);
    }

    Vector3 GetPointerPositionOnScreen()
    {
        Vector3 pointerPosition = currentTrackingInfo.bounding_box.top_left + new Vector3(currentTrackingInfo.bounding_box.width / 3.6f, -currentTrackingInfo.bounding_box.height / 21f,0);
        return ManoUtils.Instance.CalculateNewPosition(pointerPosition, currentTrackingInfo.depth_estimation);
    }
}