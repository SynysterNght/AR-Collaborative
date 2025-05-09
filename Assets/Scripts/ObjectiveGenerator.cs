using System.Collections;
using System.Collections.Generic;
using static System.MathF;
using UnityEngine;


public class ObjectiveGenerator : MonoBehaviour
{


    // Aids in viewing generated objectives
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 100.0f;

    // Objective display parameters.
    public float objectivesTransparency;

    // Sphere field generation paremeters
    public int numSpheresInField;
    public int sphereScale;
    public int jengaScale;
    public int jengaNumLevels;
    public float horizontalSpread;
    public float verticalSpread;
    public int sierpinskiNumIterations;

    // Prefab slots in editor
    public GameObject cube;
    public GameObject plane;
    public GameObject cylinder;
    public GameObject disk;
    public GameObject beam;
    public GameObject sphere;
    public GameObject house1;
    public GameObject house2;
    public GameObject dumbell;

    public int singleObjectScale;

    // Placeholder
    private GameObject instantiatedObject;

    // Prefab to place based on string value from editor
    Dictionary<string, GameObject> objectLookup = new Dictionary<string, GameObject>();
    //private List<GameObject> allObjects = new List<GameObject>();
    private List<GameObject> objectiveCentersOfMass = new List<GameObject>();
    private Vector3 swimDirection;
    private Vector3 movementDirection;
    public string testObject;

    //public GameObject camera;


    // Adjust the transparency of a GameObject
    private void setTransparencyAndColor(GameObject obj, float objectiveTransparency)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            Material mat = rend.material;
            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            mat.SetFloat("_Surface", 1);
            Color color = mat.color;
            float originalAlpha = color.a;
            color = getRandomColorRGB();
            color.a = originalAlpha * objectiveTransparency;
            mat.color = color;
        }
    }

    private Color getRandomColorRGB()
    {
        int colorIndex;
        colorIndex = UnityEngine.Random.Range(0, 3);
        if (colorIndex == 0)
        {
            return new Color(1, 0, 0);
        } else if (colorIndex == 1)
        {
            return new Color(0, 1, 0);
        } else if (colorIndex == 2)
        {
            return new Color(0, 0, 1);
        } else
        {
            return new Color(1, 1, 1); // DEFAULT if no proper color is found, return [white???]
        }
    }

    // Place a single object
    private void placeObject(string objectName, int x, int y, int z) {
        instantiatedObject = Instantiate(objectLookup[objectName], new Vector3(x, y, z), Quaternion.identity) as GameObject; // ???
        instantiatedObject.transform.localScale *= singleObjectScale;
    }

    // Generates a cloud of "n" uniformly distributed spheres within a confined space around a defined center of mass.
    private void generateSphereField(int n, Vector3 center, int scale, float horizontalSpread, float verticalSpread)
    {

        GameObject sphereFieldCenter = new GameObject("SphereFieldCenter");
        sphereFieldCenter.transform.localPosition = center;
        sphereFieldCenter.transform.SetParent(transform);

        //GameObject sphereFieldCenter = Instantiate(sphere, center, Quaternion.identity) as GameObject;
        //sphereFieldCenter.transform.localScale *= scale;
        Vector3 sphereCenter = sphereFieldCenter.transform.position;

        for (int i = 0; i < n-1; i++) {
            GameObject newSphere = Instantiate(objectLookup["sphere"],
            new Vector3(UnityEngine.Random.Range(
                sphereCenter.x-horizontalSpread,
                sphereCenter.x+horizontalSpread),
            UnityEngine.Random.Range(
                sphereCenter.y,
                sphereCenter.y+verticalSpread),
            UnityEngine.Random.Range(
                sphereCenter.z-horizontalSpread,
                sphereCenter.z+horizontalSpread)),
            Quaternion.identity) as GameObject;

            newSphere.transform.localScale *= scale;
            setTransparencyAndColor(newSphere, objectivesTransparency);
            //allObjects.Add(newSphere);
            newSphere.transform.SetParent(sphereFieldCenter.transform);
        }
        objectiveCentersOfMass.Add(sphereFieldCenter);
    }

    private void generateJenga(int levels, Vector3 center, int scale)
    {
        center += new Vector3(0, 0, 0);
        GameObject centerBaseBeam = Instantiate(beam, center, Quaternion.identity) as GameObject;
        GameObject jengaBaseObject = new GameObject("JengaTowerBaseCenter");
        jengaBaseObject.transform.SetParent(transform);
        jengaBaseObject.transform.position = center;

        centerBaseBeam.transform.localScale *= scale;
        // Get dimensions and scale of beam
        //Vector3 scale = beam.transform.localScale;
        Renderer renderer = centerBaseBeam.GetComponentInChildren<Renderer>();
        float beamLength = 0.15f;
        float beamWidth = 0.05f;
        float beamHeight = 0.05f;
        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            //Debug.Log("Beam size: " + size);
            //Debug.Log("Beam size.x: " + size.x);
            //Debug.Log("Beam size.y: " + size.y);
            //Debug.Log("Beam size.z: " + size.z);
            beamLength = size.x;
            beamWidth = size.z;
            beamHeight = size.y;
        } else {
            //Debug.Log("POTENTIALLY FATAL: Dimensions not extracted from beam \"temp\" properly!");
        }
        Destroy(centerBaseBeam);

        for (int i = 0; i < levels; i++) {
            GameObject newMiddleBeam = Instantiate(objectLookup["beam"], center + new Vector3(0, (float) i*beamHeight, 0), Quaternion.identity) as GameObject;
            GameObject newFrontBeam = Instantiate(objectLookup["beam"], center + new Vector3(0, (float) i*beamHeight, beamWidth), Quaternion.identity) as GameObject;
            GameObject newBackBeam = Instantiate(objectLookup["beam"], center + new Vector3(0, (float) i*beamHeight, -beamWidth), Quaternion.identity) as GameObject;
            //GameObject newBeam = Instantiate(objectLookup["beam"], center + new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            

            newMiddleBeam.transform.localScale *= scale;
            setTransparencyAndColor(newMiddleBeam, objectivesTransparency);
            //allObjects.Add(newMiddleBeam);
            newMiddleBeam.transform.SetParent(jengaBaseObject.transform);

            newFrontBeam.transform.localScale *= scale;
            setTransparencyAndColor(newFrontBeam, objectivesTransparency);
            //allObjects.Add(newFrontBeam);
            newFrontBeam.transform.SetParent(jengaBaseObject.transform);

            newBackBeam.transform.localScale *= scale;
            setTransparencyAndColor(newBackBeam, objectivesTransparency);
            //allObjects.Add(newBackBeam);
            newBackBeam.transform.SetParent(jengaBaseObject.transform);

            if (i % 2 == 1)
                {
                    newMiddleBeam.transform.Rotate(0f, 90f, 0f);
                    newFrontBeam.transform.Translate(beamWidth, 0, -beamWidth);
                    newFrontBeam.transform.Rotate(0f, 90f, 0f);
                    newBackBeam.transform.Translate(-beamWidth, 0, beamWidth);
                    newBackBeam.transform.Rotate(0f, 90f, 0f);
                }
        }
        objectiveCentersOfMass.Add(jengaBaseObject);
    }
    
    private void generateHouse1(Vector3 center, int scale)
    {
        center += new Vector3(0, 0, 0);
        //center += this.transform.position;
        GameObject placedHouse = Instantiate(house1, center, Quaternion.identity) as GameObject;
        placedHouse.transform.localScale *= scale;
        setTransparencyAndColor(placedHouse, objectivesTransparency);
        //allObjects.Add(placedHouse);
        objectiveCentersOfMass.Add(placedHouse);
        placedHouse.transform.SetParent(transform);
    }
    private void generateHouse2(Vector3 center, int scale)
    {
        center += new Vector3(0, 0, 0);
        //center += this.transform.position;
        GameObject placedHouse = Instantiate(house2, center, Quaternion.identity) as GameObject;
        placedHouse.transform.localScale *= scale;
        setTransparencyAndColor(placedHouse, objectivesTransparency);
        //allObjects.Add(placedHouse);
        objectiveCentersOfMass.Add(placedHouse);
        placedHouse.transform.SetParent(transform);

    }

    private void generateDumbell(Vector3 center, int scale)
    {
        center += new Vector3(0, 0, 0);
        GameObject placedDumbell = Instantiate(dumbell, center, Quaternion.identity) as GameObject;
        placedDumbell.transform.localScale *= scale;
        setTransparencyAndColor(placedDumbell, objectivesTransparency);
        objectiveCentersOfMass.Add(placedDumbell);
        placedDumbell.transform.SetParent(transform);
    }

    private void generateSierpinski(int iterations, Vector3 center, int scale)
    {
        center += new Vector3(0, 0, 0);
        GameObject baseCube = Instantiate(cube, center, Quaternion.identity) as GameObject;
        baseCube.transform.localScale *= scale;
        setTransparencyAndColor(baseCube, objectivesTransparency);

        GameObject sierpinskiParentObject = new GameObject("SierpinskiParentObject");
        sierpinskiParentObject.transform.SetParent(transform);
        sierpinskiParentObject.transform.position = center;

        List<GameObject> sierpinskiThusFar = new List<GameObject>();
        sierpinskiThusFar.Add(baseCube);

        Renderer renderer = baseCube.GetComponentInChildren<Renderer>();
        float cubeLength = 0.05f;
        float cubeWidth = 0.05f;
        float cubeHeight = 0.05f;
        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            Debug.Log("Cube size: " + size);
            Debug.Log("Cube size.x: " + size.x);
            Debug.Log("Cube size.y: " + size.y);
            Debug.Log("Cube size.z: " + size.z);
            cubeLength = size.x;
            cubeWidth = size.z;
            cubeHeight = size.y;
        }
        else
        {
            Debug.Log("POTENTIALLY FATAL: Dimensions not extracted from cube \"temp\" properly!");
        }

        int offset;
        for (int i = 1; i < iterations; i++)
        {
            List<GameObject> newCubes = new List<GameObject>();
            foreach (var obj in sierpinskiThusFar)
            {
                offset = (int)Mathf.Pow((float)2, (float)i - 1); // formerly 2^(i-1)
                GameObject newCubeLeft = Instantiate(cube, obj.transform.position + new Vector3(offset * cubeLength, 0, 0), Quaternion.identity) as GameObject;
                newCubeLeft.transform.localScale *= scale;
                setTransparencyAndColor(newCubeLeft, objectivesTransparency);
                GameObject newCubeRight = Instantiate(cube, obj.transform.position + new Vector3(0, 0, offset * cubeLength), Quaternion.identity) as GameObject;
                newCubeRight.transform.localScale *= scale;
                setTransparencyAndColor(newCubeRight, objectivesTransparency);
                newCubes.Add(newCubeLeft);
                newCubes.Add(newCubeRight);
            }
            sierpinskiThusFar.AddRange(newCubes);
        }
        // loop through final list and set them all as children of sierpinskiParentObject

        foreach (var obj in sierpinskiThusFar)
        {
            obj.transform.SetParent(sierpinskiParentObject.transform);
        }

        objectiveCentersOfMass.Add(sierpinskiParentObject);
    }

    void Start()
    {
        // Link prefabs to string names
        objectLookup.Add("cube", cube);
        objectLookup.Add("plane", plane);
        objectLookup.Add("cylinder", cylinder);
        objectLookup.Add("disk", disk);
        objectLookup.Add("beam", beam);
        objectLookup.Add("sphere", sphere);
        objectLookup.Add("house1", house1);
        objectLookup.Add("house2", house2);
    }

    public void PlaceObjective()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        System.Random random = new System.Random();
        Vector3 sphereFieldCenter = new Vector3(0, 0.15f, 0) + this.transform.position;
        int level = random.Next(1, 7);
        //level = 5;
        if (level == 1) generateSphereField(n: numSpheresInField, center: sphereFieldCenter, scale: sphereScale, horizontalSpread: horizontalSpread, verticalSpread: verticalSpread);
        if (level == 2) generateJenga(random.Next(3,8), sphereFieldCenter, jengaScale);
        if (level == 3) generateHouse1(sphereFieldCenter, 1);
        if (level == 4) generateHouse2(sphereFieldCenter , 1);
        if (level == 5) generateDumbell(sphereFieldCenter , 1);
        if (level == 6) generateSierpinski(sierpinskiNumIterations, sphereFieldCenter , 1);
        /*
        generateSphereField(n: numSpheresInField, center: sphereFieldCenter + new Vector3(0, 2, 2), scale: sphereScale, horizontalSpread: horizontalSpread, verticalSpread: verticalSpread);
        generateJenga(jengaNumLevels, sphereFieldCenter - new Vector3(1, -1, -1), jengaScale);
        generateHouse1(sphereFieldCenter + new Vector3(0, 0.05f, 0), 1);
        generateHouse2(sphereFieldCenter + new Vector3(4, 0, -5), 1);
        generateDumbell(sphereFieldCenter + new Vector3(-2, 1, -2), 1);
        generateSierpinski(sierpinskiNumIterations, sphereFieldCenter + new Vector3(3, 1, -4), 1);
        */
    }

    void HandleMovement()
    {
        movementDirection = Vector3.zero;
        swimDirection = Vector3.zero;

        // WASD keys to swim
        if (Input.GetKey(KeyCode.UpArrow)) swimDirection.z += 1;  // Move forward
        if (Input.GetKey(KeyCode.DownArrow)) swimDirection.z -= 1;  // Move backward
        if (Input.GetKey(KeyCode.LeftArrow)) swimDirection.x -= 1;  // Move left
        if (Input.GetKey(KeyCode.RightArrow)) swimDirection.x += 1;  // Move right

        // IJKL keys to move objects
        if (Input.GetKey(KeyCode.W)) movementDirection.z += 1;  // Move forward
        if (Input.GetKey(KeyCode.S)) movementDirection.z -= 1;  // Move backward
        if (Input.GetKey(KeyCode.A)) movementDirection.x -= 1;  // Move left
        if (Input.GetKey(KeyCode.D)) movementDirection.x += 1;  // Move right

        movementDirection.Normalize();
        swimDirection.Normalize();

        foreach (var obj in objectiveCentersOfMass) // allObjects
        {
            obj.transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);
        }

       // camera.transform.Translate(swimDirection * moveSpeed * Time.deltaTime, Space.World);
    }
    void HandleRotation()
    {
        float rotationDirection = 0;

        // Q/E keys for rotation
        if (Input.GetKey(KeyCode.Q)) rotationDirection += 1;  // Rotate counter-clockwise
        if (Input.GetKey(KeyCode.E)) rotationDirection -= 1;  // Rotate clockwise

        // Rotate all instantiated spheres
        foreach (var obj in objectiveCentersOfMass)
        {
            obj.transform.Rotate(Vector3.up, rotationDirection * rotateSpeed * Time.deltaTime);
        }
    }

    void AnimateStructures(int rotateDirection)
    {
        foreach (var structure in objectiveCentersOfMass) // allObjects
        {
            //if (Vector3.Distance(camera.transform.position, structure.transform.position) > 2)
            //{
                structure.transform.Rotate(Vector3.up, rotateDirection * rotateSpeed * Time.deltaTime / 5);    
                structure.transform.Translate(0, Mathf.Sin(Time.time * 2) / 80, 0);
            //}
        }
    }



    // Update is called once per frame
    void Update()
    {
        //HandleMovement();
        //HandleRotation();
        //AnimateStructures(1);
    }
}
