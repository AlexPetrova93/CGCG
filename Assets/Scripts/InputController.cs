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
}

public class InputController : MonoBehaviour
{
    public SceneProperties[] scenes;

    [SerializeField] private GameObject exitScreen;

    private int currentScene;

    private Camera cam;
    private FlyCam flyCam;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        flyCam = cam.GetComponent<FlyCam>();

        currentScene = 0;
        foreach (var scene in scenes)
        {
            scene.model.SetActive(false);
        }
        scenes[0].model.SetActive(true);
        cam.backgroundColor = scenes[0].backgroundColor;

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
        scenes[currentScene].model.SetActive(false);

        // increment scene
        ++currentScene;
        currentScene %= scenes.Length;

        // show next model
        scenes[currentScene].model.SetActive(true);
        // change camera background
        cam.backgroundColor = scenes[currentScene].backgroundColor;
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
