using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartExitButton : MonoBehaviour
{
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject setUpTxt;
    [SerializeField] TextMeshProUGUI setUpText;

    public void StartGame()
    {
        Board.preGame = true;
        Destroy(startButton);
        setUpText.fontSize = 42; 
        setUpText.text = "Game Started!!!";
        Destroy(setUpTxt,3);

    }

    public void Quit()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
