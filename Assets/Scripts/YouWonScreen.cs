using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWonScreen : MonoBehaviour
{

    public void LoadMainMenu()
    {
        // Load the main menu scene, assuming it's named "Main"
        SceneManager.LoadScene("Main");
    }
}
