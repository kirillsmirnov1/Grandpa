using System;
using Nightmares.Code.Model;
using UnityEngine;

namespace Nightmares.Code.Control.Enemy
{
    public class EnemyStaff : Enemy
    {
        [SerializeField] private GrandpaEnemyMovement grandpa;
        [SerializeField] private float moveSpeed = 1000f;
        [SerializeField] private float rotateSpeed = 5000f;
        
        private Vector3 _defaultLocalPos;
        private Quaternion _defaultRotation;

        private Action _onUpdate;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _defaultLocalPos = transform.localPosition;
            _defaultRotation = transform.localRotation;
        }

        private void Update()
        {
            _onUpdate?.Invoke();
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.layer != Constants.LayerEnemy)
            {
                rb.simulated = false;
                _onUpdate = GoToGrandpa;
            }
        }

        public override void Damage() { /* Cannot be damaged */ }

        private void GoToGrandpa()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _defaultLocalPos, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _defaultRotation, rotateSpeed * Time.deltaTime);

            if ((transform.localPosition - _defaultLocalPos).magnitude < .1f)
            {
                transform.localPosition = _defaultLocalPos;
                transform.localRotation = _defaultRotation;
                grandpa.OnStaffReturned();
                _onUpdate = null;
            }
        }
    }
}
