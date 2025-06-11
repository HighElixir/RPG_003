using UnityEngine;

namespace HighElixir.FollowObject
{
    public abstract class FollowObject : MonoBehaviour
    {
        [SerializeField] protected Transform targetTransform;
        [SerializeField] protected Vector3 targetPosition;
        [SerializeField] protected Vector3 offset;
        protected bool UseTargetPosition => targetTransform != null;
        public bool Enable { get; set; } = true;
        public void SetTarget(Transform target, Vector3 offset = default)
        {
            targetTransform = target;
            this.offset = offset;
        }
        public void SetTarget(Vector3 target)
        {
            targetPosition = target;
        }

        protected abstract void Update();
    }
}