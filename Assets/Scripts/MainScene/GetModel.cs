using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetModel : MonoBehaviour
{
    public string[] modelNames;  // Array to store the names of the 3D models
    public Object[] models;

    // Start is called before the first frame update
    void Start()
    {
         // Getting all 3D Model Objects for Resources/3d_model_prefabs folder at runtime.
        models = Resources.LoadAll("3d_model_prefabs", typeof(GameObject)); // Loaded all models present in model "Resources/3d_model_prefabs"
        modelNames = new string[models.Length]; // getting length of models and making modelNames == that length.
        
        // Storing the name of each model in the modelNames array.
        for (int i = 0; i < models.Length; i++) {
            GameObject model = GameObject.Instantiate(models[i]) as GameObject;
            modelNames[i] = models[i].name;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
