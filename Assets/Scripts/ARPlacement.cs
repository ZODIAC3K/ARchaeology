using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ARPlacement : MonoBehaviour
{   
    public TextMeshProUGUI model_name;
    public GameObject ParentObject;
    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    private GameObject spawnedObject = null;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        // LoadModel(model_name.text);
    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        if(spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject();
        }


        UpdatePlacementPose();
        UpdatePlacementIndicator();


    }
    void UpdatePlacementIndicator()
    {
        if(spawnedObject == null && placementPoseIsValid && ParentObject.transform.childCount == 0)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
            ParentObject.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        // elseif(parentObject.transform.childCount > 1)
        // {
        //     placementIndicator.SetActive(false);
        // }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if(placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }

    void ARPlaceObject()
    {
        // arObjectToSpawn.transform.TransformPose(PlacementPose);
        // spawnedObject = Instantiate(arObjectToSpawn,  _parentTransform, true);
        if(ParentObject.transform.childCount == 0)
        {
            spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation * Quaternion.Euler(0, -180, 0));
            // Set the parent of the spawnedObject GameObject to the parent GameObject's Transform
            spawnedObject.transform.SetParent(ParentObject.transform);
            // spawnedObject = Instantiate(arObjectToSpawn,  PlacementPose.position, PlacementPose.rotation * Quaternion.Euler(0, -180, 0));
        }
    }

    public void ResetPlacedObject(){
        if(spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }
    }

    // loading prefab from a path
    void LoadModel(string model_name){
        string path = model_name + ".prefab";
        arObjectToSpawn = Resources.Load<GameObject>(path);
    }
}
