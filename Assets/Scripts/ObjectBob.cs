using UnityEngine;

public class ObjectBob : MonoBehaviour
{
    float yCache;
    [SerializeField] float bobSpeed;
    [SerializeField] float bobHeight;

    // Start is called before the first frame update
    void Start()
    {
        yCache = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, yCache + Mathf.Sin((Time.time + transform.position.x) * bobSpeed) * bobHeight, transform.position.z);
    }
}
