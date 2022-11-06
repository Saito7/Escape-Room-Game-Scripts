using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeComplete : MonoBehaviour
{
    [SerializeField] private VoidEvent puzzleComplete;
    [SerializeField] private LevelLoader levelLoader;

    private void OnTriggerEnter(Collider other)
    {
        puzzleComplete.Raise();
        PlayerPrefs.SetInt("Maze", 1);
        PlayerPrefs.SetInt("currentScene", 2);
        levelLoader.LoadNextLevel(2);
    }
}
