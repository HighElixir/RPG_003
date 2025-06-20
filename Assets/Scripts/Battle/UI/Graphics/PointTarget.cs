using HighElixir.FollowObject;
using UnityEngine;
namespace RPG_003.Battle
{
    public class PointTarget : FollowObject
    {
        protected override void Update()
        {
            if (!Enable) return;
            if (UseTargetPosition)
            {
                transform.position = targetTransform.position + (Vector3)offset;
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }
}