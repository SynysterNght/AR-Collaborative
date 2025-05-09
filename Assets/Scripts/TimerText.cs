using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerText : MonoBehaviour
{
    public ObjectiveManager objectiveManager;
    TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        string timerText = objectiveManager.GetTimeText();
        text.text = timerText;
    }
}
