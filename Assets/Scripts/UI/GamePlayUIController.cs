using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIController : MonoBehaviour
{
    [Header("------PLAYER INPUT------")]
    [SerializeField]PlayerInput playerInput;
    [Header("------AUDIO DATE------")]
    [SerializeField]AudioDate pauseSFX;
    [SerializeField]AudioDate unpauseSFX;
    [Header("------CANVAS------")]
    [SerializeField]Canvas hUDCanvas;
    [SerializeField]Canvas menuCanvas;
    [Header("------PLAYER INPUT------")]
    [SerializeField]Button resumeButon;
    [SerializeField]Button optionsButon;
    [SerializeField]Button mainMenuButon;

    int ButtonPressedParameterID = Animator.StringToHash("Pressed");

    void OnEnable() 
    {
        playerInput.onPause += Pause;
        playerInput.onUnpause += Unpause;

        ButtonPressedBehavior.buttonFunctionTable.Add(resumeButon.gameObject.name, OnResumeButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(optionsButon.gameObject.name, OnOptionsButtonClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(mainMenuButon.gameObject.name, OnMainMenuButtonClick);
    }
    void OnDisable() 
    {
        playerInput.onPause -= Pause;
        playerInput.onUnpause -= Unpause;

        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }
    void Pause()
    {
        hUDCanvas.enabled = false;
        menuCanvas.enabled = true;
        GameManager.GameState = GameState.Paused;
        TimeController.Instance.Pause();
        playerInput.EnablePauseMenuInput();
        playerInput.SwitchToDynamicUpdateMode();
        UIInput.Instance.SelectUI(resumeButon);
        AutoManager.Instance.PlaySFX(pauseSFX);
    }

    void Unpause()
    {
        resumeButon.Select();
        resumeButon.animator.SetTrigger(ButtonPressedParameterID);
        AutoManager.Instance.PlaySFX(unpauseSFX);
    }
    void OnResumeButtonClick()
    {
        hUDCanvas.enabled = true;
        menuCanvas.enabled = false;
        GameManager.GameState = GameState.Playing;
        TimeController.Instance.Unpause();
        playerInput.EnableGameplayInput();
        playerInput.SwitchToFixedUpdateMode();
    }
    void OnOptionsButtonClick()
    {
        UIInput.Instance.SelectUI(optionsButon);
        playerInput.EnablePauseMenuInput();
    }
    void OnMainMenuButtonClick()
    {
        menuCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }
}
