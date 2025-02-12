using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AtivateCamShake : MonoBehaviour
{
    public CinemachineImpulseSource impulse;
    public float strength = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            impulse.GenerateImpulse(strength);
    }


}
