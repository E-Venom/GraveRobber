using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;  // Add this namespace for scene management

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _startButton;
    private List<Button> _menuButtons = new List<Button>();

    private AudioSource _audioSource;

    private void Awake()
    {
         
        _document = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();

        // Check if the UIDocument component was found
        if (_document == null)
        {
            Debug.LogError("UIDocument component not found on the GameObject.");
            return;
        }

        // Check if the AudioSource component was found
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the GameObject.");
            return;
        }

        // Initialize the start game button
        _startButton = _document.rootVisualElement.Q<Button>("StartGameButton");
        if (_startButton != null)
        {
            _startButton.RegisterCallback<ClickEvent>(OnPlayGameClick);
        }
        else
        {
            Debug.LogError("StartGameButton not found!");
        }

        // Initialize all menu buttons
        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();
        foreach (Button button in _menuButtons)
        {
            button.RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnDisable()
    {
        if (_startButton != null)
        {
            _startButton.UnregisterCallback<ClickEvent>(OnPlayGameClick);
        }

        foreach (Button button in _menuButtons)
        {
            button.UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnPlayGameClick(ClickEvent evt)
    {
        Debug.Log("You pressed the Start Button");
        SceneManager.LoadScene("Level1");  // Add this line to load 'Level1' when the start button is pressed
    }

    private void OnAllButtonsClick(ClickEvent evt)
    {
        _audioSource.Play();
    }
}
