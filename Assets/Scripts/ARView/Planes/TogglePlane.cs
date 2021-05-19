using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class TogglePlane : MonoBehaviour
{
    private bool isActive = true;
    private Button toggleBtn;

    [SerializeField] ARPlaneManager arPlaneManager;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        toggleBtn = gameObject.GetComponent<Button>();
		toggleBtn.onClick.AddListener(ToggleARPlane);
    }

    public void ToggleARPlane()
    {
        isActive = !isActive;
        arPlaneManager.enabled = isActive;
        SetAllPlanesActive(isActive);
        toggleBtn.image.color = isActive ? Color.white : Color.black;
	}

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }
    
}
