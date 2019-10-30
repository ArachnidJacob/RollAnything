using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntervalEventFire : MonoBehaviour
{
    public enum StartOn
    {
        Awake,
        OnEnable,
        Start
    }

    public StartOn startOn;

    public float intervalTiming;
    public UnityEvent intervalEvents;


    private void Awake()
    {
        if (startOn == StartOn.Awake)
            StartCoroutine("IntervalEvent");
    }

    private void OnEnable()
    {
        if (startOn == StartOn.OnEnable)
            StartCoroutine("IntervalEvent");
    }

    private void Start()
    {
        if (startOn == StartOn.Start)
            StartCoroutine("IntervalEvent");
    }

    IEnumerator IntervalEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            intervalEvents.Invoke();
        }
    }
}