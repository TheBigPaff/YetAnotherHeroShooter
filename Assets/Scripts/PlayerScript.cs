using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerScript : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller = null;
    Joystick joystick;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5f;

    bool m_IsAlive = true;
    private void Start()
    {
        joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
    }
    private void Update()
    {
        if (!IsLocalPlayer || !IsOwner) return;
        if (!m_IsAlive) return;

        //Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 input = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        controller.Move(input * movementSpeed * Time.deltaTime);


        if (controller.velocity.sqrMagnitude > 0.2f)
        {
            transform.rotation = Quaternion.LookRotation(input);
        }

        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        Vector3 velocity = controller.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }

}
