using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PUNObjectPlacer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] displayPrefabs;
    public String[] placablePrefabNames;
    private int currentIndex = -1; // Index of currently displayed prefab
    public GameObject displayObject; // The currently displayed instance
    public GameObject placedObjectParent; //The gameobject that all placed objects will be a child of
    private Color currentColor;
    private int colorIndex = 0;
    public Color[] colorSelection = {new Color(1, 0, 0), new Color(0, 1, 0), new Color(0,0,1) };
    public GameManager gameManager;



    [Tooltip("The color to change the collided Cube to.")]
    public Color collisionColor = Color.red;
    // --- Event Definition ---
    // Signature for methods listening to the event.
    public delegate void ChildCollisionHandler(GameObject triggeredObject);
    // The event itself.
    public event ChildCollisionHandler OnChildTriggeredTarget;

    void Start()
    {
        currentColor = colorSelection[colorIndex];
    }

    public void CyclePrefab()
    {
        if (displayObject != null)
        {
            Vector3 spawnPosition = displayObject.transform.position;
            Console.WriteLine(spawnPosition.x + " " + spawnPosition.y + " " + spawnPosition.z);
            Debug.Log(spawnPosition);
            Quaternion spawnRotation = displayObject.transform.rotation;

            //currentIndex = (currentIndex + 1) % placeablePrefabs.Length; //change index
            currentIndex = updateIndexForPrefabsWithRole(currentIndex);

            //displayObject = Instantiate(placeablePrefabs[currentIndex], spawnPosition, spawnRotation);
            Destroy(displayObject);

            displayObject = Instantiate(displayPrefabs[currentIndex], spawnPosition, spawnRotation);

            ChangeDisplayObjectAndChildColor(displayObject);
            displayObject.transform.SetParent(transform);
        }
    }

    GameManager FindFirstGameManagerObj()
    {
        GameManager manager = FindObjectOfType(typeof(GameManager)) as GameManager;

        if (manager != null) { return manager; }
       
        return null; // Not found
    }

    private int updateIndexForPrefabsWithRole(int currentIndex)
    {
        int newIndex = 0;
        if(gameManager == null)
        {
            gameManager = FindFirstGameManagerObj();
        }
        if(gameManager != null)
        {

            Debug.Log("Game Manager exists. Will try to use role");
            int myNumber = gameManager.getMyPhoneNumber();
            int numberOfPhones = gameManager.getNumberOfPhones();
            Debug.Log("Number of phones: ");
            Debug.Log(numberOfPhones);
            Debug.Log("My phone number: ");

            Debug.Log(myNumber);
            if (myNumber == 0 || numberOfPhones <= 1) //if you are still 0 or no other phones, then allow all permissions
            {
                newIndex = (currentIndex + 1) % displayPrefabs.Length;
            }
            else //only allow for up to two roles. Either will be odd or even
            {
                int evenOrOddOffset = myNumber % 2;

                //now increment the current Index
                newIndex = (currentIndex + 2) % displayPrefabs.Length;

                //Set the thing to be either even or odd (depending on role)
                if(newIndex >= 1)
                {
                    if (newIndex % 2 != evenOrOddOffset) { newIndex--; }
                }
                else
                {
                    if (newIndex % 2 != evenOrOddOffset) { newIndex++; } //if at 0, set it 1 for odd stuff
                }
            }
        }
        else
        {
            Debug.Log("Game manager object not provided to PUNObjectPlacer. Will cycle prefabs without roles");
            newIndex = (currentIndex + 1) % displayPrefabs.Length;
        }
        return newIndex;
    }

    public void PlacePrefab()
    {
        if (currentIndex >= 0 && currentIndex < displayPrefabs.Length)
        {
            if (displayObject != null)
            {
                Vector3 spawnPosition = displayObject.transform.position;
                Console.WriteLine(spawnPosition.x + " " + spawnPosition.y + " " + spawnPosition.z);
                Debug.Log(spawnPosition);
                Quaternion spawnRotation = displayObject.transform.rotation;


                if (placedObjectParent != null) //place as a child of the parent object
                {
                    //GameObject placedObject = Instantiate(placeablePrefabs[currentIndex], spawnPosition, spawnRotation, placedObjectParent.transform);
                    GameObject placedObject = PhotonNetwork.Instantiate(placablePrefabNames[currentIndex], spawnPosition, spawnRotation);
                    ChangeObjectAndChildColor(placedObject);
                    //placedObject.GetComponent<Renderer>().material.color = currentColor;
                }
                else //just place as child of camera
                {
                    GameObject placedObject = PhotonNetwork.Instantiate(placablePrefabNames[currentIndex], spawnPosition, spawnRotation);
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
        if (currentIndex >= 0) //Once you cycle to prefab, then color matters
        {
            if (displayObject != null) //Change the color of the display object (if there)
            {
                ChangeDisplayObjectAndChildColor(displayObject);
            }
        }
    }

    private void ChangeObjectAndChildColor(GameObject ob)
    {
        if (ob != null)
        {
            /*
            Material material = ob.GetComponent<Material>();
            if (material != null)
            {
                material.color = currentColor;
            }
            Renderer[] child_ms = ob.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < child_ms.Length; i++)
            {
                child_ms[i].material.color = currentColor;
            }
            */
            PrefabPUNSyncTools il = ob.GetComponent<PrefabPUNSyncTools>();
            if (il != null)
            {
                Debug.Log("Calling object's script to change color");
                il.SetAndBroadcastColor(currentColor);
            }
        }
      
    }

    private void ChangeDisplayObjectAndChildColor(GameObject ob)
    {
        if (ob != null)
        {
            Material material = ob.GetComponent<Material>();
            if (material != null)
            {
                material.color = currentColor;
            }
            Renderer[] child_ms = ob.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < child_ms.Length; i++)
            {
                child_ms[i].material.color = currentColor;
                Debug.Log("From ObjectPlacement, renderer size:" + child_ms[i].bounds.size);
                Debug.Log("From ObjectPlacement: " + child_ms[i].transform.position);
                Debug.Log("From ObjectPlacement: " + child_ms[i].transform.localPosition);
            }
            
        }

    }


    void Awake()
    {
        // Subscribe the color change method to our own event when the script wakes up.
        OnChildTriggeredTarget += ChangeTriggeredObjectColor;
    }
    void OnDestroy()
    {
        // IMPORTANT: Always unsubscribe from events when the listener object is destroyed
        // to prevent memory leaks and errors.
        OnChildTriggeredTarget -= ChangeTriggeredObjectColor;
    }
    public void ReloadSceneRPC()
    {
        SceneManager.LoadScene("General Room");
    }

    public void NotifyTrigger(GameObject sphereChild, GameObject triggeredObject)
    {
        Debug.Log($"Parent notified: Child '{sphereChild.name}' triggered with '{triggeredObject.name}'.");
        // You might invoke the same event or a different one:
        //OnChildCollidedWithTarget?.Invoke(sphereChild, triggeredObject, null); // Pass null for Collision if not applicable
        // Or define a separate event: public event Action<GameObject, GameObject> OnChildTriggeredTarget;
        OnChildTriggeredTarget?.Invoke(triggeredObject);

        // And then call a handler like ChangeCollidedObjectColor, adjusting parameters as needed.
        ChangeTriggeredObjectColor(triggeredObject); // Example separate handler
    }

    private void ChangeTriggeredObjectColor(GameObject triggeredObject)
    {
        Renderer objectRenderer = triggeredObject.GetComponent<Renderer>();
        if (objectRenderer != null && objectRenderer.material != null)
        {
            objectRenderer.material.color = collisionColor;
            Debug.Log($"Successfully changed color of '{triggeredObject.name}' via trigger.");
        }
        else
        {
            Debug.LogWarning($"Triggered object '{triggeredObject.name}' does not have a Renderer or Material. Cannot change color.", triggeredObject);
        }

    }
}
