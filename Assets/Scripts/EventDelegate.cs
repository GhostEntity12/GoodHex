using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EventDelegate : MonoBehaviour
{
    [SerializeField] UnityEvent e;
    public void Invoke() => e?.Invoke();
}
