using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] float damping;

    private Vector3 velocity = Vector3.zero;


    void FixedUpdate()
    {
        Vector3 movePosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position,movePosition,ref velocity, damping);
    }
}
