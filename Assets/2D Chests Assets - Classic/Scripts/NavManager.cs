using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavManager : MonoBehaviour
{
    private int index;
    private GameObject currentPanel;
    [SerializeField]
    private GameObject[] panels;

    void Start()
    {
        SetActivePanel(0);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextPanel();
        }   
    }
    public void NextPanel()
    {
        SetActivePanel((int)Mathf.Repeat(++index, panels.Length));
    }
    private void SetActivePanel(int index)
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        currentPanel = panels[index];

        currentPanel.SetActive(true);
    }

}
