using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ActionHandler : MonoBehaviour
{
    private GameObject btnBack;

    void Start()
    {
        initGUI();
        onClick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initGUI() {
        btnBack = GameObject.Find("btnBack");
    }
    
    void onClick() {
        btnBack.GetComponent<Button>().onClick.AddListener(loadOrganDetailScene);
    }
    void loadOrganDetailScene() {
        SceneManager.LoadScene(SceneConfig.organDetailScene);
    }
}
