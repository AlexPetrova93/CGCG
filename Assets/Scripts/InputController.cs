using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EScene { Frog, Car, Marina }

[Serializable]
public class SceneProperties 
{
    public EScene currentScene;
    public Color backgroundColor;
    public GameObject model;

    public float defaultDepthEdgeThreshold;
    public float defaultNormalEdgeThreshold;
    public Vector2 defaultNoiseScale;
    public Vector4 defaultUncertaintyMatrix;
}

public class InputController : MonoBehaviour
{
    public SceneProperties[] scenes;

    [SerializeField] private ControlPanel controlPanel;
    [SerializeField] private GameObject exitScreen;

    private int currentScene;

    private Camera cam;
    private FlyCam flyCam;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        flyCam = cam.GetComponent<FlyCam>();

        currentScene = -1;
        foreach (var scene in scenes)
        {
            scene.model.SetActive(false);
        }
        SwitchScene();

        exitScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(KeyCode.Escape.ToString()))
        {
            exitScreen.SetActive(!exitScreen.activeSelf);
            flyCam.canMove = !exitScreen.activeSelf;
        }
    }

    public void SwitchScene()
    {
        // hide previous model
        if (currentScene >= 0) scenes[currentScene].model.SetActive(false);

        // increment scene
        ++currentScene;
        currentScene %= scenes.Length;

        // show next model
        scenes[currentScene].model.SetActive(true);
        // change camera background
        cam.backgroundColor = scenes[currentScene].backgroundColor;

        // set default properties
        controlPanel.depthEdgeThreshold.SetCurrent(scenes[currentScene].defaultDepthEdgeThreshold.ToFloatArray());
        controlPanel.normalEdgeThreshold.SetCurrent(scenes[currentScene].defaultNormalEdgeThreshold.ToFloatArray());
        controlPanel.noiseScale.SetCurrent(scenes[currentScene].defaultNoiseScale.ToFloatArray());
        controlPanel.uncertaintyMatrix.SetCurrent(scenes[currentScene].defaultUncertaintyMatrix.ToFloatArray());
    }

    public void HideExitScreen()
    {
        exitScreen.SetActive(false);
        flyCam.canMove = true;
    }

    public void ExitApp()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }
}
