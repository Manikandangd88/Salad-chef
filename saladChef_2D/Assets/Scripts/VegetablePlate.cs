using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VegetablePlate : MonoBehaviour
{
    public string VegetableName = string.Empty;

    private void Awake()
    {
        VegetableName = transform.GetChild(0).GetComponent<Text>().text;
    }
}
