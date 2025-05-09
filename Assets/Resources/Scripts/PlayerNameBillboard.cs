using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameBillboard : MonoBehaviour
{
    private Transform mainCameraTransform;
    private RectTransform canvasRectTransform; // Assuming your text is on a Canvas

    void Start()
    {
        // Find the main camera's transform
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("No Main Camera found in the scene.");
            enabled = false;
            return;
        }

        // Get the RectTransform of the Canvas
        Canvas canvas =  GetComponent<Canvas>();
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
            if (canvasRectTransform == null)
            {
                Debug.LogError("Canvas does not have a RectTransform.");
                enabled = false;
                return;
            }
        }
        else
        {
            Debug.LogError("This script needs to be attached to a Canvas.");
            enabled = false;
            return;
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null && canvasRectTransform != null)
        {
            // Make the canvas look at the camera
            canvasRectTransform.LookAt(canvasRectTransform.position + mainCameraTransform.rotation * Vector3.forward,
                                       mainCameraTransform.rotation * Vector3.up);

            // Optional: Correct for inversion by rotating 180 degrees around the Y-axis
            canvasRectTransform.Rotate(0f, 180f, 0f);
        }
    }
}
