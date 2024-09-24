using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
public class ChangeTargetName : MonoBehaviour
{
    public static ChangeTargetName instance;
    public GameObject areaTargetCapture;
    public AreaTargetCaptureBehaviour acb;
    private void Awake()
    {
        instance = this;
        acb = areaTargetCapture.GetComponent<AreaTargetCaptureBehaviour>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeName(string id)
    {
        acb.TargetName = id;
    }
}
