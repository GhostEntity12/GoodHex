using UnityEngine;

public class TaskMenu : MonoBehaviour
{
    public GameObject menu;

    public void whenButtonClicked()
    {
        if(menu.activeInHierarchy == true)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }
    }
}