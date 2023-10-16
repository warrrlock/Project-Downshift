using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform currentCam;
    void Update()
    {
        transform.LookAt(currentCam);

        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.x = 0;
        eulerAngles.z = 0;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }

}
