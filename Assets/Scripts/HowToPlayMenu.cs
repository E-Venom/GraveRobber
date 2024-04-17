using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class HowToPlayMenu : MonoBehaviour
{
    private UIDocument _document;
    private Button _startButton;
    private Button _backButton;
    private List<Button> _menuButtons = new List<Button>();

    private AudioSource _audioSource;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();

        if (_document == null)
        {
            Debug.LogError("UIDocument component not found on the GameObject.");
            return;
        }

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

        // Initialize the back button
        _backButton = _document.rootVisualElement.Q<Button>("BackButton");
        if (_backButton != null)
        {
            _backButton.RegisterCallback<ClickEvent>(OnBackButtonClick);
        }
        else
        {
            Debug.LogError("BackButton not found!");
        }

        // Initialize all menu buttons for general purpose if needed
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
        if (_backButton != null)
        {
            _backButton.UnregisterCallback<ClickEvent>(OnBackButtonClick);
        }

        foreach (Button button in _menuButtons)
        {
            button.UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnPlayGameClick(ClickEvent evt)
    {
        _audioSource.Play();
        SceneManager.LoadScene("Level1");
    }

    private void OnBackButtonClick(ClickEvent evt)
    {
        _audioSource.Play();
        SceneManager.LoadScene("Main"); // Change "Main" to your main scene's name
    }

    private void OnAllButtonsClick(ClickEvent evt)
    {
        _audioSource.Play();
    }
}
