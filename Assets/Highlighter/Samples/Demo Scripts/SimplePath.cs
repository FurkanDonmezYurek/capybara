using UnityEngine;

namespace Highlighter
{ 
    public class SimplePath : MonoBehaviour
    {
        public class PathNodeSegment
        {
            public Vector3 positionA;
            public Quaternion rotationA;

            public Vector3 positionB;
            public Quaternion rotationB;
        }

        [Header("Path Node")]
        public Transform[] node;

        private int lastPathNodeIndex => node.Length - 1;
   
        /// <summary>
        /// Return a path node segment to make a line segment interpulate in between two point
        /// </summary>
        /// <param name="index">node index</param>
        /// <param name="loopStatus"></param>
        /// <returns></returns>
        public PathNodeSegment GetPathNodeSegment(ref int index, bool loopStatus)
        {
            if(lastPathNodeIndex <= 0)
                return null;

            var NodeA = node[index];
            index = loopStatus && index == lastPathNodeIndex ? (index % lastPathNodeIndex) : index + 1;
            var NodeB = node[index];
            return new PathNodeSegment
            {
                positionA = NodeA.position,
                rotationA = NodeA.rotation,
                positionB = NodeB.position,
                rotationB = NodeB.rotation,
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            for (int i = 1; i < node.Length; i++)
                Gizmos.DrawLine(node[i-1].position, node[i].position);
        }
#endif
    }
}
