using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Drill : MonoBehaviour
{
    public int upgradeLevel;
    [SerializeField] private Material brokenDrillMat;
    [SerializeField] private Material fixedDrillMat;
    [SerializeField] private Transform pivot;
    [SerializeField] private Renderer rend;
    [SerializeField] private float rotationSpeed = 1000f;
    public UnityEvent updateDrillColor;

    void Start()
    {
        updateDrillColor.AddListener(UpdateDrillColor);
        updateDrillColor.Invoke();

        upgradeLevel = 0;
        rend.material = brokenDrillMat;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RotateDrill();
        }
    }

    public void UpdateDrillColor()
    {
        switch (upgradeLevel) {
            case 1:
                rend.material = fixedDrillMat;
                break;
            case 2:
                rend.material.color = new Color(0.7f, 0.3f, 0.1f);
                break;
            case 3:
                rend.material.color = new Color(0.75f, 0.7f, 0.7f);
                break;
            case 4:
                rend.material.color = new Color(0.9f, 0.9f, 0.9f);
                break;
            case 5:
                rend.material.color = new Color(0.65f, 0.65f, 0.3f);
                break;
        }
    }

    private void RotateDrill() {
        pivot.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
    }
}
