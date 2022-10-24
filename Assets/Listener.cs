using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum MenuState { Null, Play, Options };
public class Listener : MonoBehaviour
{
    public MenuState ms;
    public Texture[] textures;

    public void SetState(int i)
    {
        Renderer r = gameObject.GetComponent<Renderer>();
        r.materials[0].SetTexture("_MainTex", textures[0]);
        ms = (MenuState)i;
        Debug.Log("State set to " + ms);
        switch (ms)
        {
            case MenuState.Play:
                Debug.Log("Started flip, do something based on Play");
                break;
            case MenuState.Options:
                Debug.Log("Started flip, do something based on Options");
                break;
            default:
                Debug.Log("Started flip, do something based on Null");
                break;
        }

    }
    public void OnPageFlipped()
    {
        switch (ms)
        {
            case MenuState.Play:
                Debug.Log("Finished flip, do something based on Play");
                break;
            case MenuState.Options:
                Debug.Log("Finished flip, do something based on Options");
                break;
            default:
                Debug.Log("Finished flip, do something based on Null");
                break;
        }
    }
}

