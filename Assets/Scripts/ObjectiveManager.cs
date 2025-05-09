using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    ObjectiveGenerator objectiveGenerator;
    public Transform userObjectsParent;

    [SerializeField]
    float distanceScoringFactor = 1.0f;
    [SerializeField]
    float angleScoringFactor = 1.0f;
    [SerializeField]
    float missingObjectPenalty = 100.0f;
    [SerializeField]
    float extraObjectPenalty = 50.0f;
    float timeRemaining = 0.0f;
    bool timerActive = false;
    float finalScore = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        objectiveGenerator = GetComponent<ObjectiveGenerator>();
        //StartObjective();
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        if (timerActive && timeRemaining <= 0.0f)
        {
            finalScore = CalculateScore();
            timerActive = false;
        }
        if (Input.GetKeyDown("space"))
        {
            StartObjective();
        }
    }

    public void StartObjective()
    {
        if (!timerActive)
        {
            timeRemaining = 120.0f;
            timerActive = true;
            objectiveGenerator.PlaceObjective();
            foreach (Transform child in userObjectsParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public string GetTimeText()
    {
        if (timeRemaining < 0)
        {
            return "Time's up!";
        }
        int minutes = (int)(timeRemaining / 60.0f);
        int seconds = (int)(timeRemaining % 60.0f);
        if (seconds < 10)
        {
            return minutes.ToString() + ":0" + seconds.ToString();
        }
        return minutes.ToString() + ":" + seconds.ToString();
    }

    public float CalculateScore()
    {
        if (!timerActive)
        {
            return finalScore;
        }
        // Get list of objective objects
        List<Transform> objectives = new List<Transform>();
        foreach (Transform child in transform)
        {
            foreach (Transform grandchild in child)
            {
                objectives.Add(grandchild);
            }
        }

        // Get list of user-created objects
        List<Transform> userObjects = new List<Transform>();
        foreach (Transform child in userObjectsParent)
        {
            userObjects.Add(child);
        }


        List<Transform> a_0 = new List<Transform>();
        List<Transform> a_1 = new List<Transform>();
        // Greedy matching
        GreedyMatchReverse(a_0, a_1, objectives, userObjects);
        
        // Cost calculation
        float score = 0;
        for (int i = 0; i < a_0.Count; i++)
        {
            // Score for this object
            float objectScore = 100.0f;

            // Factor in Distance
            float dist = Vector3.Distance(a_0[i].position, a_1[i].position);
            objectScore = objectScore / (1 + (distanceScoringFactor * dist));

            // Factor in Angle
            float angle = FindAngle(a_0[i], a_1[i]);
            objectScore = objectScore / (1 + (angleScoringFactor * angle / 30.0f));

            score += objectScore;
        }

        // Deduction for missed objects
        int missedObjectives = objectives.Count - a_0.Count;
        score -= missingObjectPenalty * missedObjectives;

        // Deduction for extra user objects
        int extraObjects = userObjects.Count - a_1.Count;
        score -= extraObjectPenalty * extraObjects;

        return score;
    }

    // void BruteForceMatch(List<Transform> pairedUserObjOut,
    //                      List<Transform> pairedObjectiveOut,
    //                      List<Transform> objectives,
    //                      List<Transform> userObjects)
    // {
    //     // Struct combining name and color for use with hash table
    //     struct ShapeType
    //     {
    //         public string shape;
    //         public Color color;

    //         public override bool Equals(object obj)
    //         {
    //             if (obj is ShapeType other)
    //             {
    //                 return shape == other.shape && color.Equals(other.color);
    //             }
    //             return false;
    //         }

    //         public override int GetHashCode()
    //         {
    //             // Combine hash codes
    //             return shape.GetHashCode() ^ color.GetHashCode();
    //         }
    //     }

    //     // List of objective object types
    //     List<ShapeType> uniqueShapeTypes = new List<ShapeType>();
    //     HashSet<ShapeType> nameHash = new HashSet<ShapeType>();
    //     foreach(Transform child in transform)
    //     {
    //         ShapeType childShapeType;
    //         childShapeType.name = child.name;
    //         Renderer renderer = child.GetComponent<Renderer>();
    //         childShapeType.Color = renderer.material.color;
    //         if (nameHash.Add(childShapeType))
    //         {
    //             uniqueShapeTypes.Add(childShapeType);
    //         }
    //     }

    //     foreach (ShapeType shapeType in uniqueShapeTypes)
    //     {
    //         List<Transform> filteredObjectives = new List<Transform>();
    //         foreach (Transform objective in objectives)
    //         {
    //             Renderer renderer = objective.GetComponent<renderer>();
    //             Color objectiveColor = renderer.material.color;
    //             if (objectiveColor == shapeType.color && objective.name == shapeType.name)
    //             {
    //                 filteredObjectives.Add(objective);
    //             }
    //         }
            
    //         List<Transform> filteredUserObjects = new List<Transform>();
    //         foreach (Transform userObject in userObjects)
    //         {
    //             Renderer renderer = userObject.GetComponent<renderer>();
    //             Color userObjectColor = renderer.material.color;
    //             if (userObjectColor == shapeType.color && userObject.name == shapeType.name)
    //             {
    //                 filteredUserObjects.Add(userObject);
    //             }
    //         }
    //     }
    // }

    // public static List<List<T>> GetPermutations<T>(List<T> list, int length)
    // {
    //     if (length == 1)
    //         return list.Select(t => new T[] { t });

    //     return GetPermutations(list, length - 1)
    //         .SelectMany(t => list.Where(o => !t.Contains(o)),
    //                     (t1, t2) => t1.Concat(new T[] { t2 }));
    // }

    void GreedyMatchReverse(List<Transform> pairedUserObjOut,
                     List<Transform> pairedObjectiveOut,
                     List<Transform> objectives,
                     List<Transform> userObjects)
    {
        foreach (Transform userObject in userObjects)
        {
            // Find closest object of the same shape
            float minDist = (float)Mathf.Infinity;
            Transform minObjective = null;
            Renderer userObjectRenderer = userObject.GetComponent<Renderer>();
            Color userObjectColor = userObjectRenderer.material.color;
            
            foreach (Transform currObjective in objectives)
            {
                Renderer objectiveRenderer = currObjective.GetComponent<Renderer>();
                Color currObjectiveColor = objectiveRenderer.material.color;
                // Checking if objects are the same shape using their tags
                if (userObject.gameObject.tag == currObjective.gameObject.tag &&
                    AreColorsEqual(currObjectiveColor, userObjectColor) &&
                    !pairedObjectiveOut.Contains(currObjective))
                {
                    float distance = Vector3.Distance(userObject.position, currObjective.position);
                    if (distance < minDist)
                    {
                        minDist = distance;
                        minObjective = currObjective;
                    }
                }
            }

            // Add to paired lists
            if (minObjective != null)
            {
                pairedUserObjOut.Add(userObject);
                pairedObjectiveOut.Add(minObjective);
            }
        }
    }

    // void GreedyMatch(List<Transform> pairedUserObjOut,
    //                  List<Transform> pairedObjectiveOut,
    //                  List<Transform> objectives,
    //                  List<Transform> userObjects)
    // {
    //     foreach (Transform objective in objectives)
    //     {
    //         // Find closest object of the same shape
    //         float minDist = (float)Mathf.Infinity;
    //         Transform minUserObj = null;
            
    //         foreach (Transform currUserObj in userObjects)
    //         {
    //             // Checking if objects are the same shape using their names
    //             if (objective.name == currUserObj.name && !pairedUserObjOut.Contains(currUserObj))
    //             {
    //                 float distance = Vector3.Distance(objective.position, currUserObj.position);
    //                 if (distance < minDist)
    //                 {
    //                     minDist = distance;
    //                     minUserObj = currUserObj;
    //                 }
    //             }
    //         }

    //         // Add to paired lists
    //         if (minUserObj != null)
    //         {
    //             pairedObjectiveOut.Add(objective);
    //             pairedUserObjOut.Add(minUserObj);
    //         }
    //     }
    // }

    float FindAngle(Transform obj1, Transform obj2)
    {
        // Not complete, always returns 0.0f
        return 0.0f;
    }

    bool AreColorsEqual(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b);
    }
}
