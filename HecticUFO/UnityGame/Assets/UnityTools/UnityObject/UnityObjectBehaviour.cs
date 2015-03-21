using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

public class UnityObjectBehaviour : MonoBehaviour
{
    public UnityObject UnityObject;

    public void Start()
    {
        //Normal process
        //new UnityObject, UnityObjectBehaviour, Awake(), set UnityObject, Step()

        //UnityObject should always be set
        if (UnityObject == null)
        {
            //Disable incase we continue in editor
            enabled = false;
            throw new Exception(gameObject.name + " has UnityObjectBehaviour but doesn't have UnityObject assigned, bad prefab?");
        }

        if (UnityObject.UnityStart != null)
            UnityObject.UnityStart.Fire(UnityObject);
    }

    private void OnMouseEnter()
    {
        if (UnityObject.UnityMouseEnter != null)
            UnityObject.UnityMouseEnter.Fire(UnityObject);
    }
    private void OnMouseExit()
    {
        if (UnityObject.UnityMouseExit != null)
            UnityObject.UnityMouseExit.Fire(UnityObject);
    }
    private void OnMouseUp()
    {
        if (UnityObject.UnityMouseUp != null)
            UnityObject.UnityMouseUp.Fire(UnityObject);
    }
    private void OnGUI()
    {
        if (UnityObject.UnityGUI != null)
            UnityObject.UnityGUI.Fire(UnityObject);
    }
    private void Update()
    {
        if (UnityObject.UnityUpdate != null)
            UnityObject.UnityUpdate.Fire(UnityObject);
    }
    private void FixedUpdate()
    {
        if (UnityObject.UnityFixedUpdate != null)
            UnityObject.UnityFixedUpdate.Fire(UnityObject);
    }
    private void OnMouseOver()
    {
        if (UnityObject.UnityMouseOver != null)
            UnityObject.UnityMouseOver.Fire(UnityObject);
    }
    private void OnDrawGizmos()
    {
        //Start has nice clean error
        if (UnityObject == null)
            return;

        if (UnityObject.UnityDrawGizmos != null)
            UnityObject.UnityDrawGizmos.Fire(UnityObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Start has nice clean error
        if (UnityObject == null)
            return;

        if (UnityObject.UnityOnCollisionEnter != null)
            UnityObject.UnityOnCollisionEnter.Fire(UnityObject, collision);
    }
}