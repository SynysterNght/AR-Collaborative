using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneObjectPlacer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] placeablePrefabs;
    private int currentIndex = -1; // Index of currently displayed prefab
    public GameObject displayObject; // The currently displayed instance
    public GameObject placedObjectParent; //The gameobject that all placed objects will be a child of
    private Color currentColor;
    private int colorIndex = 0;
    public Color[] colorSelection = {new Color(1, 1, 1), new Color(1, 0, 0), new Color(1, 1, 0), new Color(0, 1, 0),
    new Color(0,1,1), new Color(1,0,1), new Color(0,0,0) };
    void Start()
    {
        currentColor = colorSelection[colorIndex];
    }

    public void CyclePrefab()
    {
        if (displayObject != null)
        {
            Destroy(displayObject);

            Vector3 spawnPosition = displayObject.transform.position;
            Console.WriteLine(spawnPosition.x + " " + spawnPosition.y + " " + spawnPosition.z);
            Debug.Log(spawnPosition);
            Quaternion spawnRotation = displayObject.transform.rotation;

            currentIndex = (currentIndex + 1) % placeablePrefabs.Length; //change index
            
            displayObject = Instantiate(placeablePrefabs[currentIndex], spawnPosition, spawnRotation);
            ChangeObjectAndChildColor(displayObject);
            displayObject.transform.SetParent(transform);
        }
    }

    public void PlacePrefab()
    {
        if (currentIndex >= 0 && currentIndex < placeablePrefabs.Length)
        {
            if (displayObject != null)
            {
                Vector3 spawnPosition = displayObject.transform.position;
                Console.WriteLine(spawnPosition.x + " " + spawnPosition.y + " " + spawnPosition.z);
                Debug.Log(spawnPosition);
                Quaternion spawnRotation = displayObject.transform.rotation;

                
                if(placedObjectParent != null) //place as a child of the parent object
                {
                    GameObject placedObject = Instantiate(placeablePrefabs[currentIndex], spawnPosition, spawnRotation, placedObjectParent.transform);
                    ChangeObjectAndChildColor(placedObject);
                    //placedObject.GetComponent<Renderer>().material.color = currentColor;
                }
                else //just place as child of camera
                {
                    GameObject placedObject = Instantiate(placeablePrefabs[currentIndex], spawnPosition, spawnRotation);
                    ChangeObjectAndChildColor(placedObject);
                    //placedObject.GetComponent<Renderer>().material.color = currentColor;
                }
            }
        }
    }

    public void CycleColor()
    {
        colorIndex += 1;
        colorIndex %= colorSelection.Length;
        currentColor = colorSelection[colorIndex];
        if(currentIndex >=0) //Once you cycle to prefab, then color matters
        {
            if(displayObject != null) //Change the color of the display object (if there)
            {
                ChangeObjectAndChildColor(displayObject);
            }
        }
    }

    private void ChangeObjectAndChildColor(GameObject ob)
    {
        if(ob != null)
        {
            Material material = ob.GetComponent<Material>();
            if (material != null)
            {
                material.color = currentColor;
            }
            Renderer[] child_ms = ob.GetComponentsInChildren<Renderer>();
            for (int i = 0; i< child_ms.Length; i++)
            {
                child_ms[i].material.color = currentColor;
            }
        }
    }
}
