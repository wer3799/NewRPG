using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreSceneStartButton : SingletonMono<PreSceneStartButton>
{
    [SerializeField]
    private Button startButton;
    [SerializeField] 
    private GameObject textObject;
    [SerializeField]
    private GameObject waitText;


    private new void Awake()
    {
        base.Awake();
        startButton.interactable = false;
        textObject.SetActive(false);
        waitText.SetActive(true);
    }
    public void SetInteractive()
    {
        textObject.SetActive(true);
        waitText.SetActive(false);
        startButton.interactable = true;
    }
    public void OnClickStartButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}