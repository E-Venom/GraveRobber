using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UIHandler : MonoBehaviour
{   
    // used for NPC dialogue
    public float displayTime = 4.0f;

    // NPC Visual dialogue box
    private VisualElement m_NonPlayerDialogue;

    // timer used for NPC dialogue display
    private float m_TimerDisplay;
    public static UIHandler instance { get; private set; }

    // UI element for players health = red
    VisualElement m_Healthbar;

    // UI element for gold collected = gold
    VisualElement m_GoldBar;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // access UIDocument 
        UIDocument uiDocument = GetComponent<UIDocument>();

        // initialize healthbar variable with UI HealthBar sprite
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");

        // set the health bar at 100% full to start
        SetHealthValue(1.0f);

        // initialize goldbar variable with UI GoldBar sprite
        m_GoldBar = uiDocument.rootVisualElement.Q<VisualElement>("GoldBar");

        // set the gold bar at 0% to start
        SetGoldValue(0.0f);

        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        if (m_NonPlayerDialogue == null)
        {
            //Debug.LogError("NPCDialogue VisualElement not found!");TO DO NEED TO FIX NPC DIALOGUE
            return;
        }
        else
        {
            m_NonPlayerDialogue.style.display = DisplayStyle.None;
        }
        m_TimerDisplay = -1.0f;
    }

    /*An Update function that decrements (decreases) the display timer if it is above zero and 
     * then resets m_NonPlayerDialogue to DisplayStyle.None when the countdown is over.*/
    private void Update()
    {
        if (m_NonPlayerDialogue != null)
        {
            if (m_TimerDisplay > 0)
            {
                m_TimerDisplay -= Time.deltaTime;
            }
            else
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None; // throws null pointer
            }
        }
    }

    // displays NPC dialogue and resets dialogue display timer
    public void DisplayDialogue()
    {
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;   // throws null pointer
        m_TimerDisplay = displayTime;
    }

    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    public void SetGoldValue(float percentage)
    {
        m_GoldBar.style.width = Length.Percent(100 * percentage);
    }
}
