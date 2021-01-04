using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartingPoint : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = new Color(4f,253f,46f,130f);
    [SerializeField][Range(0.1f,10f)] private float debugCubeSize = 1f;


    private void OnDrawGizmos() {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position,new Vector3(debugCubeSize,debugCubeSize,debugCubeSize));
    }
}
