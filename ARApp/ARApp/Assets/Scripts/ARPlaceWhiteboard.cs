using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlaceWhiteboard : MonoBehaviour
{
    public Whiteboard WhiteboardTemplate;
    public GameObject Label;
    public Button RepeatButton;
    public Button RestartButton;
    
    private ARPlaneManager _arPlaneManager;
    private TextMeshPro _labelText;
    private Whiteboard _whiteboard;
    private ARRaycastManager _arRaycastManager;
    private Vector2 _touchPosition;
    private Camera _camera;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    private void Start()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _labelText = Label.GetComponent<TextMeshPro>();
        _arPlaneManager = GetComponent<ARPlaneManager>();
        
        RepeatButton.onClick.AddListener(repeat);
        RestartButton.onClick.AddListener(restart);
        
        RepeatButton.enabled = false;
        RestartButton.enabled = false;
    }

    void Update()
    {
        if (_whiteboard == null)
            placeBoard();
    }

    private void placeBoard()
    {
        Vector2 position = new Vector2(Screen.width / 2, Screen.height / 4);
        
        if (_arRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            float dist = Vector3.Distance(_camera.transform.position, hitPose.position);
            if (dist > 2.5f && dist < 3f)
            {
                _whiteboard = Instantiate(WhiteboardTemplate, hitPose.position, hitPose.rotation);
                _whiteboard.StartSpelling();
                
                RepeatButton.enabled = true;
                RestartButton.enabled = true;
                
                _labelText.enabled = false;
                disablePlanes();
            }
        }
    }

    private void restart()
    {
        SceneManager.LoadScene(sceneName:"CaptureImageScene");
    }

    private void repeat()
    {
        _whiteboard.StartSpelling();
    }
    
    private void disablePlanes()
    {
        foreach (var plane in _arPlaneManager.trackables)
            plane.gameObject.SetActive(false);
    }
}
