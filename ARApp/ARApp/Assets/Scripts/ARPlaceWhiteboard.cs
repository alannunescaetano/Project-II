using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlaceWhiteboard : MonoBehaviour
{
    public Whiteboard WhiteboardTemplate;

    private Whiteboard whiteboard;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    void Start()
    {
        
    }

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            
            if (whiteboard == null)
                whiteboard = Instantiate(WhiteboardTemplate, hitPose.position, hitPose.rotation);
            else
                whiteboard.transform.position = hitPose.position;
            
            //whiteboard.transform.LookAt(transform);
            whiteboard.isVisible = true;
        }
    }
}
