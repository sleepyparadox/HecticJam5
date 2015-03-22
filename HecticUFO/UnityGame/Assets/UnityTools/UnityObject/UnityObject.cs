using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityTools_4_6;

public class UnityObject
{
    public readonly GameObject GameObject;

    //Exposed functionality
    public Vector3 WorldPosition { get { return GameObject.transform.position; } set { GameObject.transform.position = value; } }
    public Vector3 LocalPosition { get { return GameObject.transform.localPosition; } set { GameObject.transform.localPosition = value; } }
    public Transform Transform { get { return GameObject.transform; } }
    public Transform Parent { get { return Transform.parent; } set { Transform.parent = value; } }

    //Exposed events
    public UnityEventAction<UnityObject> UnityMouseEnter;
    public UnityEventAction<UnityObject> UnityMouseExit;
    public UnityEventAction<UnityObject> UnityMouseUp;
    public UnityEventAction<UnityObject> UnityMouseOver;
    public UnityEventAction<UnityObject> UnityUpdate;
    public UnityEventAction<UnityObject> UnityFixedUpdate;
    public UnityEventAction<UnityObject> UnityStart;
    public UnityEventAction<UnityObject> OnDispose;
    public UnityEventAction<UnityObject> UnityGUI;
    public UnityEventAction<UnityObject> UnityDrawGizmos;
    public UnityEventAction<UnityObject> UnityInspectorGUI;
    public UnityEventAction<UnityObject, Collision> UnityOnCollisionEnter;

    private UnityObjectBehaviour _behaviour;

    public UnityObject()
        : this(new GameObject("Unnamed UnityObject"))
    {
        GameObject.name = GetType().ToString();
    }

    public UnityObject(string name)
        : this(new GameObject(name))
    {
    }

    public UnityObject(PrefabAsset asset)
        : this(asset.Prefab.Clone())
    {

    }

    public UnityObject(GameObject sceneObject)
    {
        if (sceneObject == null)
            throw new Exception("UnityObject sceneObject is null");

        GameObject = sceneObject;

        //Try to get the existing behaviour
        _behaviour = sceneObject.GetComponent<UnityObjectBehaviour>();

        if (_behaviour == null)
            _behaviour = sceneObject.AddComponent<UnityObjectBehaviour>();

        _behaviour.UnityObject = this;
    }
    public static GameObject CreatePrefabInstance(string prefabPath)
    {
        return CreatePrefabInstance(prefabPath, Vector3.zero, Quaternion.identity);
    }
    public static GameObject CreatePrefabInstance(string prefabPath, Vector3 position, Quaternion rotation)
    {
        var prefab = Resources.Load(prefabPath);
        return (GameObject)UnityEngine.Object.Instantiate(prefab, position, rotation);
    }
    public static GameObject FindInScene(string name)
    {
        return GameObject.Find(name);
    }

    public bool Alive
    {
        get
        {
            return GameObject != null;
        }
    }

    public void SetActive(bool active)
    {
        GameObject.SetActive(active);
    }

    public virtual void Dispose()
    {
        if (OnDispose != null)
            OnDispose.Fire(this);
        if (GameObject != null)
            UnityEngine.Object.Destroy(GameObject);
    }
        
    public GameObject FindChild(string child)
    {
        return GameObject.transform.FindChild(child).gameObject;
    }

    public T0 FindChildComponent<T0>(string child) where T0 : Component
    {
        return GameObject.transform.FindChild(child).GetComponent<T0>();
    }

    public void SetLayer(int layer, bool recursive)
    {
        if (recursive)
            RecursiveTransform(t => t.gameObject.layer = layer);
        else
            GameObject.layer = layer;
    }

    public void RecursiveTransform(Action<Transform> operation)
    {
        RecursiveTransform(Transform, operation);
    }

    public static void RecursiveTransform(Transform target, Action<Transform> operation)
    {
        operation(target);
        for (var i = 0; i < target.childCount; ++i)
        {
            RecursiveTransform(target.GetChild(i), operation);
        }
    }

    public static implicit operator Transform(UnityObject unityObject)
    {
        return unityObject.Transform;
    }
}
