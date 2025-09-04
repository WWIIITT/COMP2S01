using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    // Toggles the active state of the GameObject to which this script is attached
    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
