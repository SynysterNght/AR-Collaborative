using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARObjectsSelection : MonoBehaviour
{
    [Tooltip("The tag assigned to the external 'Cube' GameObject we want to detect collisions with.")]
    public string targetTag;
    private PUNObjectPlacer parentController;
    private Collider ownCollider; // Reference to this object's collider

    // Start is called before the first frame update
    void Start()
    {
        parentController = GetComponentInParent<PUNObjectPlacer>();

        if (parentController == null)
        {
            Debug.LogError($"ChildCollisionDetector on '{gameObject.name}' could not find a ParentController script on any of its parent GameObjects. Please ensure the parent has the ParentController script.", this);
        }

        // --- Physics Setup Check (Optional but Recommended) ---
        // For OnCollisionEnter to work, one of the colliding objects MUST have a non-kinematic Rigidbody.
        // The other object needs at least a Collider.
        //Rigidbody rb = GetComponent<Rigidbody>();
        //Rigidbody parentRb = GetComponentInParent<Rigidbody>(); // Check parent too, in case Rigidbody is there

        // If this sphere doesn't have a Rigidbody and isn't controlled by a parent Rigidbody,
        // the 'Cube' it collides with will need one.
        /*if (rb == null && parentRb == null)
        {
            // This is just a warning, as the Cube might have the Rigidbody.
            // Debug.LogWarning($"Child '{gameObject.name}' does not have a Rigidbody. Ensure the '{targetTag}' object it collides with has a non-kinematic Rigidbody for OnCollisionEnter to trigger.", this);
        }
        else if (rb != null && rb.isKinematic && (parentRb == null || parentRb.isKinematic))
        {
            // Kinematic Rigidbodies only trigger collisions reliably if the OTHER object has a non-kinematic Rigidbody.
            // Debug.LogWarning($"Child '{gameObject.name}' has a kinematic Rigidbody. Ensure the '{targetTag}' object it collides with has a non-kinematic Rigidbody for OnCollisionEnter to trigger.", this);
        }*/
    }
    /*
    void OnCollisionEnter(Collision collision)
    {
        // First, check if we successfully found the parent controller
        if (parentController == null)
        {
            return; // Can't report the collision if there's no controller
        }

        // Check if the GameObject we collided with has the specific tag we're looking for (e.g., "Cube")
        if (collision.gameObject.CompareTag(targetTag))
        {
            // It's the Cube! Notify the parent controller.
            // Pass 'this.gameObject' (the specific sphere that collided)
            // and 'collision.gameObject' (the cube that was hit).
            parentController.NotifyCollision(this.gameObject, collision.gameObject, collision);
        }
    }*/
    void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("Using  "+ other.name+"  "+other.tag, this);
        if (parentController == null) return;

        if (other.gameObject.CompareTag(targetTag))
        {
            // Note: OnTriggerEnter provides the Collider 'other', not a 'Collision' object.
            // You might need to adjust the NotifyCollision signature or create a new one.
            // Example: parentController.NotifyTrigger(this.gameObject, other.gameObject);
            parentController.NotifyTrigger(this.gameObject, other.gameObject);
            Debug.LogWarning("Using OnTriggerEnter - make sure ParentController.NotifyCollision or a similar method handles this.", this);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
