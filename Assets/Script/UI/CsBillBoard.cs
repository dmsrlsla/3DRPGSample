using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsBillBoard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + CsGameData.Instance.InGameCamera.transform.rotation * Vector3.forward,
            CsGameData.Instance.InGameCamera.transform.rotation * Vector3.up);
    }
}
