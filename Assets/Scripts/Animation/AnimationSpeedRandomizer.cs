using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationSpeedRandomizer : MonoBehaviour
{
    [SerializeField] float m_MinSpeed, m_Max_Speed =1;
    [SerializeField] Animator m_Animator;
    private const string SPEED_ID = "speed";
    private int m_SpeedID;
    private void OnValidate()
    {
        m_Animator= m_Animator?? GetComponent<Animator>();
        m_SpeedID = Animator.StringToHash(SPEED_ID);
    }

    void Start()
    {
        m_Animator.SetFloat(m_SpeedID, UnityEngine.Random.Range(m_MinSpeed, m_Max_Speed)) ;
    }

}
