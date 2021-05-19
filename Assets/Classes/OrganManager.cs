using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OrganManager
{
    public static string CurrentOrgan { get; set; }
    public static GameObject CurrentOrganObject { get; set; }
    public static bool IsRotating { get; set; }
    public static bool IsMoving { get; set; }
    public static bool IsTagged { get; set; }
    public static bool IsInfo { get; set; }
    public static Organ DataOrgan { get; set; }
    public static void InitOrgan(string currentOrganName, GameObject currentOrganObject, Organ dataOrgan, bool isRotating, bool isMoving)
    { 
        CurrentOrgan = currentOrganName;
        CurrentOrganObject = currentOrganObject;
        DataOrgan = dataOrgan;
        IsRotating = isRotating;
        IsMoving = isMoving;
    }
}
