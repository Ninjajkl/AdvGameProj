using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class drillCode : MonoBehaviour
{
    private int upgradeLevel;

    //material variables
    Material mat;
    GameObject obj;
    Transform trans;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        obj = GetComponent<GameObject>();
        trans = GetComponent<Transform>();

        mat = rend.material;

        upgradeLevel = 0;
    }

    void Update()
    {
        Debug.Log(mat);

        if (Input.GetKeyDown(KeyCode.T))
        {
            upgradeLevel++;
        }

        //code to change colors of drill
        if (upgradeLevel == 0)
        {
            mat.color = Color.grey;
        }
        else if (upgradeLevel == 1)
        {
            mat.color = new Color(0.7f,0.3f,0.1f);
        }
        else if (upgradeLevel == 2)
        {
            mat.color = new Color(0.75f,0.7f,0.7f);
        }
        else if (upgradeLevel == 3)
        {
            mat.color = new Color(0.9f, 0.9f, 0.9f);
        }
        else if (upgradeLevel == 4)
        {
            mat.color = new Color(0.65f, 0.65f, 0.3f);
        }

        if (Input.GetMouseButton(0))
        {
            trans.rotation.Set(trans.rotation.x, trans.rotation.y, trans.rotation.z + 1, trans.rotation.w);
        }
    }
}
