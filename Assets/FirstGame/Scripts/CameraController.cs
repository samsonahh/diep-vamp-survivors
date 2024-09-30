using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGameProg2Game
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;

        [Header("References")]
        [SerializeField] private Transform followTarget;

        [Header("Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] private float followSpeed = 5f;
        private Vector3 targetPosition;

        private bool isCamShaking;
        private float camShakeTimer = Mathf.Infinity;
        private float camShakeMagnitude;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        private void FixedUpdate()
        {
            AssignTargetPosition();
            FollowTarget();

            UpdateCameraShake();
        }

        private void AssignTargetPosition()
        {
            if (followTarget == null) return;

            targetPosition = followTarget.position + offset;
        }

        private void FollowTarget()
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        private void UpdateCameraShake()
        {
            if (!isCamShaking) return;

            Vector3 randomShakeOffset = camShakeMagnitude * (Vector3)Random.insideUnitCircle.normalized;
            Vector3 targetShakePosition = targetPosition + randomShakeOffset;

            camShakeTimer -= Time.unscaledDeltaTime;
            if (camShakeTimer <= 0) isCamShaking = false;

            transform.position = Vector3.Lerp(transform.position, targetShakePosition, Time.unscaledDeltaTime);
        }

        public void ShakeCamera(float magnitude, float duration)
        {
            isCamShaking = true;

            camShakeTimer = duration;
            camShakeMagnitude = magnitude;
        }
    }
}

