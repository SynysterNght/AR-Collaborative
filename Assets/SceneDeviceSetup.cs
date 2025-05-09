using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDeviceSetup : MonoBehaviour
{
    public GameObject[] phoneSpecificObjects;
    public GameObject[] hmdSpecificObjects;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Device Model: " + SystemInfo.deviceModel);

        if (SystemInfo.deviceModel.ToLower().Contains("quest")) //turn on quest stuff. turn of phone stuff
        {
            Debug.Log("Running on a Meta Quest device.");
            for(int i=0; i< hmdSpecificObjects.Length; i++)
            {
                hmdSpecificObjects[i].gameObject.SetActive(true);
            }

            for (int i = 0; i < phoneSpecificObjects.Length; i++)
            {
                phoneSpecificObjects[i].gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Running on a regular Android phone.");

            for (int i = 0; i < phoneSpecificObjects.Length; i++)
            {
                phoneSpecificObjects[i].gameObject.SetActive(true);
            }

            for (int i = 0; i < hmdSpecificObjects.Length; i++)
            {
                hmdSpecificObjects[i].gameObject.SetActive(false);
            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
