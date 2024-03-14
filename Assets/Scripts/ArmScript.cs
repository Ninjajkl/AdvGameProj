using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    [SerializeField] Animator arm_animator;
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            arm_animator.SetBool("mining", true);
        }
        else
        {
            arm_animator.SetBool("mining", false);
        }
    }
}
