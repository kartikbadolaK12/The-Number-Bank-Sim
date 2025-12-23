using System.Collections;

using UnityEngine;

public class FinalManager : MonoBehaviour
{
    public GameObject glowSlotsRow;
    public GameObject arab;
    public float delay=1f;
    public float delay2=1f;
    public GameObject recorderAnimation;
    public GameObject hideGameObject1;
    public GameObject hideGameObject2;
    public GameObject confettiEffect;
    public AudioManager audioManager;
    public AudioClip confettiSound;
    public GameObject instructionPanel;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        glowSlotsRow.SetActive(true);
        arab.SetActive(false);
        instructionPanel.SetActive(false);
       StartCoroutine(StartAnimation()); 
        StartCoroutine(StartNextAnimation()); 
    }
IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(delay);
        recorderAnimation.SetActive(true);
        hideGameObject1.SetActive(false);
        hideGameObject2.SetActive(false);
        glowSlotsRow.SetActive(false);

   
    }
    IEnumerator StartNextAnimation()
    {
        yield return new WaitForSeconds(delay2);
       confettiEffect.SetActive(true);
              audioManager.StopAudio();
       audioManager.PlayAudio(confettiSound);

   
    }
}
