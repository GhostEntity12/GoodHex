using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public static string[] names = new string[] {
        "Jerry",
        "Cheddar",
        "Gouda",
        "Brie",
        "Cam",
        "Whiskers",
        "Dr. Squeaks",
        "Geronimo",
        "Moz",
        "Roque",
        "Rick",
        "Gru",
        };

    public GameObject progressBarPrefab;

    public Canvas c;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Restart() => SceneManager.LoadScene(0);
}
