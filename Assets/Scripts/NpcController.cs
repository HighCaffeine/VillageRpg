using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private Transform target; // gameManager���� ������

    public float npcSpeed;

}
