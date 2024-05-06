using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private UpgradeableObject reactor;
    [SerializeField] private Material yellowLight;
    [SerializeField] private Material redLight;
    [SerializeField] private Color yellowLightColor;
    [SerializeField] private Color redLightColor;
    [SerializeField] private MeshRenderer[] lightObjects;
    [SerializeField] private Light[] pointLights;

    private void Start() {
        for(int i = 0; i < lightObjects.Length; i++) {
            var materialsCopy = lightObjects[i].materials;
            materialsCopy[1] = redLight;
            lightObjects[i].materials = materialsCopy;
            pointLights[i].color = redLightColor;
        }
    }

    public void CheckIfReactorFixed() {
        if(reactor.objectFixed) {
            for(int i = 0; i < lightObjects.Length; i++) {
                var materialsCopy = lightObjects[i].materials;
                materialsCopy[1] = yellowLight;
                lightObjects[i].materials = materialsCopy;
                pointLights[i].color = yellowLightColor;
            }
        }
    }
}
