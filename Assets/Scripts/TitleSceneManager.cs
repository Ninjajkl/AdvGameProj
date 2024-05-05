using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class TitleSceneManager : MonoBehaviour
{
    public PlayableDirector cutscene;
    public GameObject directionalLight;
    public GameObject StartButton;
    public GameObject SettingsButton;
    public GameObject BedrockText;
    public GameObject VirtualCam1;
    public GameObject VirtualCam2;
    public AudioSource nuke_aud;
    public AudioSource amb_aud;
    public AudioClip nuke_whistle;
    public AudioClip nuke_boom;
    public bool directionalLightOff;
    Quaternion zeroRotation = Quaternion.Euler(Vector3.zero);

    [Header("Camera Shaking")]
    public CinemachineVirtualCamera VirtualCamera1ThugShaker;
    [SerializeField] private float shakeIntensity = 1f;
    private CinemachineBasicMultiChannelPerlin _cbmcp;

    private void Awake()
    {
        directionalLight.SetActive(true);
        _cbmcp = VirtualCamera1ThugShaker.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        VirtualCam1.SetActive(true);
        VirtualCam2.SetActive(false);
        VirtualCamera1ThugShaker.gameObject.SetActive(false);
        directionalLightOff = false;
    }

    private void Update() {
        if(directionalLight.transform.rotation == zeroRotation) {
            directionalLight.SetActive(false);
        }
        
        if(directionalLightOff) {
            directionalLight.transform.rotation = Quaternion.Slerp(directionalLight.transform.rotation, zeroRotation, Time.deltaTime * 5f);
        }
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
        nuke_aud.Play();
    }

    #region Camera Shaking Functions

    public void ShakeCamera()
    {
        VirtualCam1.SetActive(false);
        VirtualCamera1ThugShaker.gameObject.SetActive(true);
        _cbmcp.m_AmplitudeGain = shakeIntensity;
        directionalLightOff = true;
    }

    public void StopShake()
    {
        _cbmcp.m_AmplitudeGain = Mathf.MoveTowards(1f,0f,Time.deltaTime);
    }

    #endregion
}
