using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    public void exitgame() {
        Application.Quit();
        Debug.Log("Exit Game");
    }
}
