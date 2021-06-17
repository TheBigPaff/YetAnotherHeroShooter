using UnityEngine;

namespace SinglePlayerMode
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GunController))]
    public class PlayerScript : LivingEntity
    {
        public float movementSpeed = 12f;

        public Crosshairs crosshairs;

        private Rigidbody rb;
        private Vector3 movement;
        private GunController gunController;

        Joystick movementJoystick;
        Joystick aimJoystick;

        protected override void Start()
        {
            base.Start();

            // GET REFERENCES 
            movementJoystick = GameObject.Find("MovementJoystick").GetComponent<Joystick>();
            aimJoystick = GameObject.Find("AimJoystick").GetComponent<Joystick>();
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            gunController = GetComponent<GunController>();
            FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        }

        void OnNewWave(int waveNumber)
        {
            health = startingHealth;
            gunController.EquipGun(waveNumber - 1);
        }

        private void Update()
        {
            //Plane playerPlane = new Plane(transform.up, Vector3.up * gunController.GunHeight);
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //float rayDistance = 0f;

            if (Mathf.Abs(aimJoystick.Horizontal) > 0.01 || Mathf.Abs(aimJoystick.Vertical) > 0.01)
            {
                Vector3 aimInput = new Vector3(aimJoystick.Horizontal, 0, aimJoystick.Vertical);
                transform.rotation = Quaternion.LookRotation(aimInput);
                //transform.LookAt(aimInput);
                //gunController.Aim(aimInput);
            }

            if (Mathf.Abs(aimJoystick.Horizontal) > 0.7 || Mathf.Abs(aimJoystick.Vertical) > 0.7)
            {
                gunController.OnTriggerHold();
            }
            else
            {
                gunController.OnTriggerRelease();
            }

            //if (playerPlane.Raycast(ray, out rayDistance))
            //{
            //    //looking at the mouse point
            //    Vector3 targetPoint = ray.GetPoint(rayDistance);
            //    Vector3 targetPointWithCorrectedHeight = new Vector3(targetPoint.x, transform.position.y, targetPoint.z);
            //    transform.LookAt(targetPointWithCorrectedHeight);

            //    crosshairs.transform.position = targetPoint;
            //    crosshairs.DetectTargets(gunController.weaponHolder, gunController.weaponHolderInside);
            //    if ((new Vector2(targetPoint.x, targetPoint.z) - new Vector2(transform.position.x, transform.position.z)).magnitude > 1)
            //        gunController.Aim(targetPoint);
            //}

            //Getting Movement Input
            //movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            movement = new Vector3(movementJoystick.Horizontal, 0, movementJoystick.Vertical);

            ////Getting Shooting Input
            //if (Input.GetMouseButton(0))
            //{
            //    gunController.OnTriggerHold();
            //}
            //if (Input.GetMouseButtonUp(0))
            //{
            //    gunController.OnTriggerRelease();
            //}
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    gunController.Reload();
            //}



            if (transform.position.y < -10f)
            {
                TakeDamage(health);
            }
        }

        public override void Die()
        {
            AudioManager.instance.PlaySound("Player Death", transform.position);
            base.Die();
        }
        private void FixedUpdate()
        {
            rb.MovePosition(transform.position + (movement.normalized * movementSpeed * Time.fixedDeltaTime));
        }
    }
}
