using System;
using DG.Tweening;
using UnityEngine;

namespace Nightmares.Code.Control
{
    public class CameraFollowsPlayer : MonoBehaviour
    {
        public float lerpSpeed = 1f;
        
        public Transform target;
        [SerializeField] private PlatformerGameManager gameManager;
        
        [Header("Tween Down")]
        [SerializeField] private float tweenDownDuration = 5f;
        [SerializeField] private Ease ease = Ease.InCubic;
        
        private Camera _cam;
        
        private float _yMax;
        private float _yMin;
        private Action _onFixedUpdate;

        // TODO move with lerp

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            ResetFixedUpdate();
        }

        private void FixedUpdate()
        {
            _onFixedUpdate?.Invoke();
        }

        public void InitDimensions()
        {
            _yMax = 1f;
            _yMin = -gameManager.LevelDimensions.y - gameManager.GrandpasRoomHeight;
        }

        private void FollowPlayer()
        {
            var pos = transform.position;
            var newY = Mathf.Clamp(target.position.y, _yMin + _cam.orthographicSize, _yMax - _cam.orthographicSize);
            pos.y = newY;
            transform.position = Vector3.Lerp(transform.position, pos, lerpSpeed * Time.fixedDeltaTime);
        }

        [ContextMenu("Reset Fixed Update")]
        private void ResetFixedUpdate()
        {
            transform.DOKill();
            _onFixedUpdate = FollowPlayer;
        }

        [ContextMenu("Tween Down")]
        private void TweenDown()
        {
            _onFixedUpdate = null;
            transform.DOMoveY(_yMin + _cam.orthographicSize, tweenDownDuration).SetEase(ease);
        }
    }
}