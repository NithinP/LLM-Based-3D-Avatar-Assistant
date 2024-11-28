using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record_Gizmo : MonoBehaviour
{
    [Header("GameObject to control")]
    public GameObject GizmoOne;
    public GameObject GizmoTwo;
    // Start is called before the first frame update
    void Start()
    {
        Record_Gizmo_Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Record_Gizmo_Enable()
    {
        GizmoOne.SetActive(true);
    }
    public void Record_Gizmo_Disable()
    {
        GizmoOne.SetActive(false);
    }
    public void Recog_Gizmo_Enable()
    {
        GizmoTwo.SetActive(true);
    }
    public void Recog_Gizmo_Disable() {
        GizmoTwo.SetActive(false);
    }
}
