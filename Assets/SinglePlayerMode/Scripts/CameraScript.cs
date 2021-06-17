using UnityEngine;

namespace SinglePlayerMode
{
    public class CameraScript : MonoBehaviour
    {
        //Variables 
        public GameObject player;
        [SerializeField] private float cameraHeight = 20f;
        [SerializeField] private float zAxisOffset = 3f;
        private float smoothSpeed = 0.2f;
        private Vector3 velocity = Vector3.zero;
        LivingEntity playerEntity;
        //Methods

        private void Start()
        {
            playerEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<LivingEntity>();
        }
        private void FixedUpdate()
        {
            if (!playerEntity.dead)
            {
                Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y + cameraHeight, player.transform.position.z - zAxisOffset);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
            }
        }
    }

}
