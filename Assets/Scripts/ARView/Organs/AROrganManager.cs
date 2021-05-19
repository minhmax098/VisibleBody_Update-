using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AROrganManager : MonoBehaviour
{
    private static AROrganManager instance;
    public static AROrganManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AROrganManager>();
            }
            return instance;
        }   
    }
    public ARAnchorManager arAnchorManager;

    private GameObject currentOrganObject;

    void Start()
    {
        OrganManager.CurrentOrganObject = Resources.Load(PlayerPrefs.GetString("nameOrgan")) as GameObject;
    }
    void Update()
    {

    }

    public void InitOrgan (Pose pose)
    {
        currentOrganObject = Instantiate(OrganManager.CurrentOrganObject, pose.position, pose.rotation);
        currentOrganObject.transform.localScale = currentOrganObject.transform.localScale / 5;
    }

    public GameObject GetCurrentOrganObject()
    {
        return currentOrganObject;
    }

    public void SetOrganPosition(Pose pose)
    {
        arAnchorManager.AddAnchor(pose);
        currentOrganObject.transform.position = pose.position;
    }

    public void ChangeOrganRotation(Vector3 rotation)
    {
        currentOrganObject.transform.rotation *= Quaternion.Euler(rotation);
    }
    public void ChangeOrganScale(Vector3 scale)
    {
        currentOrganObject.transform.localScale = scale;
    }

    public void ScaleOrganByPercentage(float percentage)
    {
        currentOrganObject.transform.localScale *= percentage;
    }

    public Vector3 GetLocalScaleOfCurrentOrganObject() 
    {
        return currentOrganObject.transform.localScale;
    }

    public Vector3 GetPositionOfCurrentOrganObject()
    {
        return currentOrganObject.transform.position;
    }
}
