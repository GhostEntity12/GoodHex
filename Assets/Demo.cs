using UnityEngine;

public class Demo : MonoBehaviour
{
    public Animator anim;
    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.P))
		{
            anim.SetBool("Active", true);
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
            anim.SetBool("Active", false);
		}
    }
}
