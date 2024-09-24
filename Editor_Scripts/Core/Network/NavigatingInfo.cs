using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigatingInfo : MonoBehaviour
{
    public string Name;
    public Vector3 Pos;
    public string objRealName;
    private void Awake()
    {
        //Pos = new Vector3(transform.position.x, 0, transform.transform.position.z);
        Pos = transform.position;
        objRealName = gameObject.name;
    }
}
