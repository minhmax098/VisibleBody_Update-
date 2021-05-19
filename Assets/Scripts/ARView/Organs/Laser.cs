using UnityEngine;
using System.Collections;
public class Laser : MonoBehaviour {
    private LineRenderer laser;
	// Use this for initialization
	void Start () 
    {
        laser = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        laser.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider)
            {
                laser.SetPosition(1, hit.point);
            }
        }
        else 
        {
            laser.SetPosition(1, transform.forward * 5000);
        }
	}
}
