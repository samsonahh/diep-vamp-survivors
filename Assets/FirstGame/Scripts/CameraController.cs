using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform followTarget;

        [Header("Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] private float followSpeed = 5f;

        private void FixedUpdate()
        {
            FollowTarget();
        }

        private void FollowTarget()
        {
            transform.position = Vector3.Lerp(transform.position, followTarget.position + offset, followSpeed * Time.deltaTime);
        }
    }
}

