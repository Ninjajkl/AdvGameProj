using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class TitleSceneManager : MonoBehaviour
{
    public PlayableDirector cutscene;
    public GameObject StartButton;
    public GameObject SettingsButton;
    public GameObject BedrockText;
    public GameObject VirtualCam1;
    public GameObject VirtualCam2;

    [Header("Camera Shaking")]
    public CinemachineVirtualCamera VirtualCamera1ThugShaker;
    [SerializeField] private float shakeIntensity = 1f;
    private CinemachineBasicMultiChannelPerlin _cbmcp;

    private void Awake()
    {
        _cbmcp = VirtualCamera1ThugShaker.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void HideMainUI()
    {
        StartButton.SetActive(false);
        SettingsButton.SetActive(false);
        BedrockText.SetActive(false);
    }

    public void ShowMainUI()
    {
        StartButton.SetActive(true);
        SettingsButton.SetActive(true);
        BedrockText.SetActive(true);
    }

    public void ShowSettingsMenu()
    {
        HideMainUI();
        VirtualCam1.SetActive(false);
        VirtualCam2.SetActive(true);
    }

    public void HideSettingsMenu()
    {
        ShowMainUI();
        VirtualCam1.SetActive(true);
        VirtualCam2.SetActive(false);
    }

    public void StartTimeline()
    {
        HideMainUI();
        cutscene.Play();
    }

    #region Camera Shaking Functions

    public void ShakeCamera()
    {
        VirtualCam1.SetActive(false);
        VirtualCamera1ThugShaker.gameObject.SetActive(true);
        _cbmcp.m_AmplitudeGain = shakeIntensity;
    }

    public void StopShake()
    {
        _cbmcp.m_AmplitudeGain = 0f;
        VirtualCam1.SetActive(true);
        VirtualCamera1ThugShaker.gameObject.SetActive(false);
    }

    #endregion
}
