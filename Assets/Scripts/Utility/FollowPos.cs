using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPos : MonoBehaviour
{
    [SerializeField] private Transform followed;

    private void Update() {
        transform.position = followed.position;
    }
}
