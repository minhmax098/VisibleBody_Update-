using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSeparationByGesture : MonoBehaviour
{
    public enum State
    {
        None,
        Pointing,
        ReadyMoving
    }
    State state = State.None;
    private HandInfo currentHandInfo;
    private GestureInfo currentGestureInfo;
    private TrackingInfo currentTrackingInfo;
    public float minAcceptableCorrectTime = 1.5f;
    public int acceptableWrongFrame = 20;
    private float correctTime;
    private int wrongFrame;
    public float castingRadius = 0.06f;
    private Vector3 currentPosition;
    private Vector3 currentPointerPosition;
    private Vector3 deltaPosition;
    public GameObject pointerPrefab;
    private GameObject pointerReferance;
    
    // Start is called before the first frame update
    void Start()
    {
        pointerReferance = Instantiate(pointerPrefab);
        pointerReferance.transform.localScale /= 20;
        pointerReferance.SetActive(false);

        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (ManomotionManager.Instance.Hand_infos[0].hand_info.warning == Warning.WARNING_HAND_NOT_FOUND || ModelSeparation.Instance.IsHighlighting)
        {
            Reset();
            return;
        }

        UpdateInfos(ManomotionManager.Instance.Hand_infos[0].hand_info);

        UpdatePointerPosition();

        switch(state)
        {
            case State.None: 
                if (IsPointingOnObject())
                {
                    correctTime += Time.deltaTime;
                    if (correctTime >= minAcceptableCorrectTime)
                    {
                        ModelSeparation.Instance.StartHightLightingObject();
                        state = State.Pointing;
                    }
                }
                else
                {
                    wrongFrame++;
                    if (wrongFrame >= acceptableWrongFrame)
                    {
                        Reset();
                    }
                }
                break;
            case State.Pointing: 
                if (IsPointing())
                {
                    currentPointerPosition = Helper.GetPointerPosition(currentTrackingInfo);
                    deltaPosition = currentPointerPosition - Camera.main.transform.position;
                    currentPosition.z = ModelSeparation.Instance.SelectedObject.transform.position.z;
                    currentPosition.x = currentPointerPosition.x + deltaPosition.x * (currentPosition.z - currentPointerPosition.z) / deltaPosition.z;
                    currentPosition.y = currentPointerPosition.y + deltaPosition.y * (currentPosition.z - currentPointerPosition.z) / deltaPosition.z;
                    ModelSeparation.Instance.MoveObject(currentPosition);
                }
                else 
                {
                    ModelSeparation.Instance.StopHightLightingObject();
                    Reset();
                }
                break;
            default: 
                break;
        }
    }

    void Reset() 
    {
        correctTime = 0;
        wrongFrame = 0;
        state = State.None;
    }

    void Init()
    {
        correctTime = 0;
    }
    void UpdateInfos (HandInfo handInfo)
    {
        currentHandInfo = handInfo;
        currentGestureInfo = currentHandInfo.gesture_info;
        currentTrackingInfo = currentHandInfo.tracking_info;
    }

    bool IsPointingOnObject()
    {
        if (!IsPointing())
        {
            return false;
        }
        ModelSeparation.Instance.SelectedObject = Helper.GetObjectOnPointerByTag(Helper.GetPointerPosition(currentTrackingInfo), castingRadius, ObjectTag.organTag);
        return ModelSeparation.Instance.SelectedObject != null;
    }

    bool IsPointing()
    {
        return currentGestureInfo.mano_gesture_continuous == ManoGestureContinuous.POINTER_GESTURE;
    }

    void UpdatePointerPosition()
    {
        if (IsPointing())
        {
            pointerReferance.transform.position = Helper.GetPointerPosition(currentTrackingInfo);
            pointerReferance.SetActive(true);
        }
        else
        {
            pointerReferance.SetActive(false);
        }
    }
}
