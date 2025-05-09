using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public ObjectiveManager objectiveParent;
    TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        float score = Mathf.Round(objectiveParent.CalculateScore() * 100.0f) * 0.01f;
        if(text.text != null) { text.text = "Score: " + score.ToString(); }
        else { text = gameObject.GetComponent<TextMeshPro>();}
    }
}
