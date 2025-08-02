using System.Collections;
using UnityEngine.Events;
using UnityEngine;

namespace Highlighter
{
    public class SimplePathFollower : MonoBehaviour
    {
        [System.Serializable]
        public class ArriveResponse
        {
            public uint nodeIndex = 0;
            public UnityEvent OnReach;
        }

        [Tooltip("Follow speed to traverse the path")]
        public float moveSpeed = 1.0f;
        [Tooltip("Rotation speed to look towards to the moving direction")]
        public float rotationSpeed = 1.0f;
        [Tooltip("Loop condition to traverse the path")]
        public bool useLoop = false;
        public SimplePath path;

        [Header("Events")]
        [Tooltip("The Event will be fire when the tranform reached to the index that index you set on the event")]
        public ArriveResponse[] arriveResponses;

        private Coroutine followRoutine = null;
        protected virtual void OnEnable()
        {
            followRoutine = StartCoroutine(FollowPath());
        }

        protected virtual void OnDisable()
        {
            if (followRoutine != null)
                StopCoroutine(followRoutine);
        }

        private IEnumerator FollowPath()
        {
            int index = 0;
            SimplePath.PathNodeSegment nodeSegment;
            while ((nodeSegment = path.GetPathNodeSegment(ref index, useLoop)) != null)
            {
                float t = 0;
                float dt = 1 / Vector3.Distance(nodeSegment.positionA, nodeSegment.positionB);
                while (t < 1)
                {
                    t += dt * moveSpeed * Time.deltaTime;
                    transform.position = Vector3.Lerp(nodeSegment.positionA, nodeSegment.positionB, t);
                    var rot = Quaternion.Lerp(nodeSegment.rotationA, nodeSegment.rotationB, t);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
                    yield return null;
                }

                //Fire the arrive response event when the transform reached at the desired path node
                for (int i = 0; i < arriveResponses.Length; i++)
                    if (arriveResponses[i].nodeIndex == index)
                        arriveResponses[i].OnReach.Invoke();
            }
        }
    }

}
