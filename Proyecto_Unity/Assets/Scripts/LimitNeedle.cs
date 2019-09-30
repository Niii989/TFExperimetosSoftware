using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitNeedle : MonoBehaviour
{
    public Button savebtn;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Ground")
        {           
            savebtn.interactable = false;
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Ground")
        {            
            savebtn.interactable = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Ground")
        {            
            savebtn.interactable = true;
        }

    }
}
