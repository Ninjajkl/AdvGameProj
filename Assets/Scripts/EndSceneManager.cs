using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EndSceneManager : MonoBehaviour
{
    public GameObject[] cinemachineVirtualCameras;
    public int index = 0;

    private void Awake() {
        
        cinemachineVirtualCameras[index].SetActive(true);
    }

    private void Start() {
        StartCoroutine(SwitchCamCoroutine());
    }

    private IEnumerator SwitchCamCoroutine() {
        while(true) {
            yield return new WaitForSeconds(10);
            switchCam();
        }
    }

    public void switchCam() {
        cinemachineVirtualCameras[index].SetActive(false);
        index++;

        if(index >= cinemachineVirtualCameras.Length) {
            index = 0;
        }

        cinemachineVirtualCameras[index].SetActive(true);
    }
}
