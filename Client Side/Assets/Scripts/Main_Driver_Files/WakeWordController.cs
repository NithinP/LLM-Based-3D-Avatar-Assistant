using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeWordController : MonoBehaviour
{
    TextManager tm;
    Emitter em;
    public GameObject _CharacterPrefab;
    public Animator characterAnimator;
    public ParticleSystem _circle;
    public ParticleSystem _splash;
    private bool isProcessing = false;
    private bool isDisappearing = false;
    private bool hasPlayedInitialAnimation = false;

    void Start()
    {
        tm = FindObjectOfType<TextManager>();
        em = FindObjectOfType<Emitter>();
        _CharacterPrefab.SetActive(false);
        _circle.gameObject.SetActive(false);
        _splash.gameObject.SetActive(false);
        tm.OnSystemStateChange += HandleSystemStateChange;

        if (!tm.isSystemActive)
        {
            em.HideEmission();
        }
        else
        {
            em.showEmission();
        }
    }

    void OnDestroy()
    {
        if (tm != null)
        {
            tm.OnSystemStateChange -= HandleSystemStateChange;
        }
    }

    private void HandleSystemStateChange(bool isActive)
    {
        if (isActive && !isProcessing && !hasPlayedInitialAnimation)
        {
            StartCoroutine(PlayParticlesAndActivateCharacter());
            hasPlayedInitialAnimation = true;
        }
        else if (!isActive)
        {
            StartCoroutine(PlayDisappear());
            hasPlayedInitialAnimation = false;
        }
    }

    void Update()
    {
        // Remove the system state checking from Update
        // as it's now handled by the event system
    }

    IEnumerator PlayParticlesAndActivateCharacter()
    {
        if (isProcessing) yield break;
        isProcessing = true;
        _circle.gameObject.SetActive(true);
        _splash.gameObject.SetActive(true);
        _circle.Play();
        _splash.Play();
        yield return new WaitForSeconds(1.3f);
        em.showEmission();
        em.isEffectRunning = false;
        _CharacterPrefab.SetActive(true);
        characterAnimator = _CharacterPrefab.GetComponent<Animator>();
        if (characterAnimator != null)
        {
            Debug.Log("Found Instance");
        }
        yield return new WaitForSeconds(3.5f);
        characterAnimator.applyRootMotion = false;
        characterAnimator.SetTrigger("Idle");
        _circle.gameObject.SetActive(false);
        _splash.gameObject.SetActive(false);
        isProcessing = false;
    }

    IEnumerator PlayDisappear()
    {
        if (isDisappearing) yield break;
        isDisappearing = true;
        characterAnimator.SetTrigger("Disappear");
        yield return new WaitForSeconds(4f);
        em.HideEmission();
        _CharacterPrefab.SetActive(false);
        isDisappearing = false;
    }

    bool IsAnimationPlaying(int animLayer, string stateName)
    {
        return characterAnimator.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName)
            && characterAnimator.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f;
    }
}