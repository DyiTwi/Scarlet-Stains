using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float lookSpeed = 2f;
    public float gravity = -9.81f;

    private CharacterController characterController;
    private Transform cameraTransform;
    private float verticalRotation = 0f;
    private Vector3 moveVelocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        // locks mouse cursor to center of screen and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // mouse look around
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f); // Prevents looking upside down
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // keyboard movement
        float moveForward = Input.GetAxis("Vertical") * moveSpeed;
        float moveSide = Input.GetAxis("Horizontal") * moveSpeed;

        Vector3 movement = (transform.forward * moveForward) + (transform.right * moveSide);
        
        // gravity
        if (characterController.isGrounded && moveVelocity.y < 0)
        {
            moveVelocity.y = -2f; 
        }

        moveVelocity.y += gravity * Time.deltaTime;
        movement.y = moveVelocity.y;

        characterController.Move(movement * Time.deltaTime);
    }
}