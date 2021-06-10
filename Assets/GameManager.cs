using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform npcTransform, targetTransform;
    private PathFinding pathFinding;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
    }

    private void Start()
    {
        pathFinding.pathFindDelegate(npcTransform.position, targetTransform.position);
    }
}
