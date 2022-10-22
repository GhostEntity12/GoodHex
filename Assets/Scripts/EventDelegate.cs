using UnityEngine;
using UnityEngine.Events;

public class EventDelegate : MonoBehaviour
{
    [SerializeField] UnityEvent e;
    public void InvokeEvents() => e?.Invoke();
}
