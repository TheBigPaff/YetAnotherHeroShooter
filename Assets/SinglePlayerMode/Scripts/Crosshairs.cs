using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinglePlayerMode
{
    public class Crosshairs : MonoBehaviour
    {
        public LayerMask targetMask;
        public SpriteRenderer dot;
        public Color dotHighlightColor;
        Color initialColor;

        void Start()
        {
            Cursor.visible = false;
            initialColor = dot.color;
        }
        void Update()
        {
            transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
        }

        public void DetectTargets(Transform transform, Transform transformInside)
        {
            if (Physics.Raycast(transformInside.position, transform.right, Mathf.Infinity, targetMask))
            {
                dot.color = dotHighlightColor;
            }
            else
            {
                dot.color = initialColor;
            }

        }
    }
}
