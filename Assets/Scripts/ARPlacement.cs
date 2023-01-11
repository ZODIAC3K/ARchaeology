using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacement : MonoBehaviour
{

    // public Transform _parentTransform;
    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    private GameObject spawnedObject = null;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
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
        if(spawnedObject == null && placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
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
        spawnedObject = Instantiate(arObjectToSpawn,  PlacementPose.position, PlacementPose.rotation * Quaternion.Euler(0, -180, 0));
    }

    public void ResetPlacedObject(){
        Destroy(spawnedObject);
        spawnedObject = null;
        
        
        // int childCount = _parentTransform.childCount;

        // for (int i = childCount - 1; i >= 0; i--){
        //     Destroy(_parentTransform.GetChild(i).gameObject);
        // }
    }
}
