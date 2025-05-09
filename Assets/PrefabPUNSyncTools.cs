using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using static UnityEngine.UI.GridLayoutGroup;
using System.Linq;


public class PrefabPUNSyncTools : MonoBehaviourPunCallbacks
{
    private GameObject placedobjparent;
    private Renderer[] renderers;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log($"[InstantiationLogger] Prefab '{gameObject.name}' instantiated at position {transform.position}. Local pos {transform.localPosition}");
        //photonView.RPC("SpawnNetworkObject", RpcTarget.All, Vector3.zero, Quaternion.identity);
        renderers = GetComponentsInChildren<Renderer>();
        Renderer parentRenderer = gameObject.GetComponent<Renderer>();
        if ( parentRenderer != null)
        {
            renderers.Append<Renderer>(parentRenderer);
        }
    }

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Start()
    {
        placedobjparent = GameObject.FindGameObjectWithTag("PlacedObjectParent");
        if( placedobjparent != null )
        {
            Debug.Log("found placed object parent. Setting as parent for instantiated object");
            this.transform.SetParent( placedobjparent.transform, true);
        }
        lastPosition = transform.localPosition;
        Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
        float beamLength = 0.15f;
        float beamWidth = 0.05f;
        float beamHeight = 0.05f;
        Debug.Log("REACHED HERE================");
        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            //Debug.Log("Shape size: " + size);
/*            Debug.Log("Shape size.x: " + size.x);
            Debug.Log("Shape size.y: " + size.y);
            Debug.Log("Shape size.z: " + size.z);*/
            beamLength = size.x;
            beamWidth = size.z;
            beamHeight = size.y;
        }
        else
        {
            Debug.Log("POTENTIALLY FATAL: Dimensions not extracted from beam \"temp\" properly!");
        }
    }

    void Update()
    {
        if (SystemInfo.deviceModel.ToLower().Contains("quest"))
        {
            if (transform.localPosition != lastPosition)
            {
                OnMoved(lastPosition, transform.localPosition);
                lastPosition = transform.localPosition;
            }
            if (transform.localRotation != lastRotation)
            {
                OnRotated(lastRotation, transform.localRotation);
                lastRotation = transform.localRotation;
            }
        }
    }

    void OnMoved(Vector3 from, Vector3 to)
    {
        Debug.Log($"Object moved from {from} to {to}");
        // Do something in response to movement
        photonView.RPC("SetPosition", RpcTarget.AllBuffered, to);
    }

    [PunRPC]
    public void SetPosition(Vector3 pos)
    {
        Renderer rendererInSetPosition = gameObject.GetComponentInChildren<Renderer>();
        float movedObjectLength = 0f;
        float movedObjectWidth = 0f;
        float movedObjectHeighth = 0f;
        Debug.Log("REACHED HERE FROM SetPosition================");
        Vector3 size = new Vector3(0, 0, 0);
        if (rendererInSetPosition != null)
        {
            size = rendererInSetPosition.bounds.size;
            //Debug.Log("Shape size: " + size);
            /*            Debug.Log("Shape size.x: " + size.x);
                        Debug.Log("Shape size.y: " + size.y);
                        Debug.Log("Shape size.z: " + size.z);*/
            movedObjectLength = size.x;
            movedObjectWidth = size.z;
            movedObjectHeighth = size.y;
        }
        else
        {
            Debug.Log("POTENTIALLY FATAL: Dimensions not extracted from beam \"temp\" properly!");
        }
        transform.localPosition = pos;
        Debug.Log("Headset from SetPosition size: " + size);
        Debug.Log("Headset from SetPosition Regular Position: " + rendererInSetPosition.transform.position);
        Debug.Log("Headset from SetPosition Local Positoin: " + rendererInSetPosition.transform.localPosition);
    }


    void OnRotated(Quaternion from, Quaternion to)
    {
        photonView.RPC("SetRotation", RpcTarget.AllBuffered, to);
    }

    [PunRPC]
    public void SetRotation(Quaternion rot)
    {
        transform.localRotation = rot;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        PhotonView view = GetComponent<PhotonView>();
        if (view != null)
        {
            string owner = view.Owner != null ? view.Owner.NickName : "Scene/Room Object";
            Debug.Log($"[Photon] Object '{gameObject.name}' owned by: {owner}, ViewID: {view.ViewID}");
        }
        else
        {
            Debug.Log($"[Photon] No PhotonView on '{gameObject.name}' — local-only object.");
        }
    }


    [PunRPC]
    public void SetColor(float r, float g, float b)
    {
        UnityEngine.Color color = new UnityEngine.Color(r, g, b);

        foreach (var rend in renderers)
        {
            if (rend != null)
            {
                // Important: Instantiate a new material instance (don't modify shared material)
                rend.material = new Material(rend.material);
                rend.material.color = color;
            }

            float beamLength = 0f;
            float beamWidth = 0f;
            float beamHeight = 0f;
            Debug.Log("REACHED HERE ON PHONE================");
            if (rend != null)
            {
                Vector3 size = rend.bounds.size;
                Debug.Log("Phone Shape size: " + size);
                Debug.Log("Phone Regular Position: " + rend.transform.position);
                Debug.Log("Phone Local Positoin: " + rend.transform.localPosition);
                /*            Debug.Log("Shape size.x: " + size.x);
                            Debug.Log("Shape size.y: " + size.y);
                            Debug.Log("Shape size.z: " + size.z);*/
                beamLength = size.x;
                beamWidth = size.z;
                beamHeight = size.y;
            }
            else
            {
                Debug.Log("POTENTIALLY FATAL: Dimensions not extracted from beam \"temp\" properly!");
            }
        }
    }

    public void SetAndBroadcastColor(UnityEngine.Color color)
    {
        // Apply locally
        photonView.RPC("SetColor", RpcTarget.AllBuffered, color.r, color.g, color.b);
    }

}
