using UnityEngine;
using UnityEditor;

// CopyComponents - by Michael L. Croswell for Colorado Game Coders, LLC
// March 2010
//Modified by Kristian Helle Jespersen
//June 2011

public class ReplaceGameObjects : ScriptableWizard
{
    public bool copyValues = true;
    public GameObject NewType;
    public GameObject[] OldObjects;

    [MenuItem("Custom/Replace GameObjects")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace GameObjects", typeof(ReplaceGameObjects), "Replace");
    }

    void OnWizardCreate()
    {
        int counter = 0;

        foreach (GameObject go in OldObjects)
        {
            GameObject newObject;
            newObject = (GameObject)PrefabUtility.InstantiatePrefab(NewType); ;
            newObject.transform.position = go.transform.position;
            newObject.transform.rotation = go.transform.rotation;
            newObject.transform.parent = go.transform.parent;
            newObject.name += counter;
            counter++;

            DestroyImmediate(go);
        }
    }
}