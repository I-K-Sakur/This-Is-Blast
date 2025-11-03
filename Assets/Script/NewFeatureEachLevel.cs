using UnityEngine;

public class NewFeatureEachLevel : MonoBehaviour
{
    private bool colorBlock = false, multipleColorBlock = false, revealColor = false, levelRiseHigher = false;

    [SerializeField] private GameObject ColorBlockUi;
    [SerializeField] private GameObject MultipleColorBlockUi;
    [SerializeField] private GameObject RevealColorUI;
    [SerializeField] private GameObject LevelRiseHigherUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SingleColorBlockActive()
    {
        colorBlock = true;
        ColorBlockUi.SetActive(false);
    }
    public void MultipleColorBlockActive()
    {
        multipleColorBlock = true;
        MultipleColorBlockUi.SetActive(false);
    }  
    public void RevealColorActive()
    {
        revealColor = true;
        RevealColorUI.SetActive(false);
    }
    public void LevelRiseHigherActive()
    {
        levelRiseHigher = true;
        LevelRiseHigherUI.SetActive(false);
    }
}
