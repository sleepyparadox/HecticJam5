using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityTools_4_6;

[CustomEditor(typeof(UnityObjectBehaviour))]
public class UnityObjectInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var unityObjectBehaviour = (UnityObjectBehaviour)target;

        //Handle duped unity objects
        if(unityObjectBehaviour.UnityObject == null)
        {
            GUILayout.Label("Bad unity object refrence");
            return;
        }

        var unityObject = (UnityObject)unityObjectBehaviour.UnityObject;

        if(unityObject.UnityInspectorGUI != null)
        {
            unityObject.UnityInspectorGUI.Fire(unityObject);
        }
        else
        {
            GUILayout.Label(".UnityInspectorGUI event is empty");
        }
    }
}
