using UnityEngine;

namespace Highlighter
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class LookAtHighlight : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] protected Highlightable highlightable;

        protected virtual void Awake()
        {
            if(cam == null)
                cam = Camera.main;
        }


        public virtual void Update()
        {
            if (highlightable)
            {
                var camToObjDir = transform.position - cam.transform.position;
                float alpha = Vector3.Dot(camToObjDir.normalized, cam.transform.forward);
                alpha = Mathf.InverseLerp(0.93f, 0.98f, alpha);
                highlightable.alpha = alpha;
            }
#if UNITY_EDITOR
            else
                if(Application.isPlaying)
                    Debug.LogWarning("No any highlightable effect assigned in 'LookAtHighlight' component\nPlease assign it manually");
#endif
        }
    }
}