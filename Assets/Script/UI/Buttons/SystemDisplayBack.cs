using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemDisplayBack : MonoBehaviour
{
    public GameObject MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        var button = transform.GetComponent<Button>();
        button.onClick.AddListener(this.ButtonClicked);
    }
 
    public void ButtonClicked()
    {
        MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, 1200f, MainCamera.transform.position.z);
    }
}
