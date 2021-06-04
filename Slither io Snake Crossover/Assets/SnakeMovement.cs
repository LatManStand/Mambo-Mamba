using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    public List<Transform> bodyParts = new List<Transform>();

    public float speed = 3.5f;
    public float currentRotation;
    public float RotationSensitivity = 50;

    [Range(0.0f, 1.0f)]
    public float smoothTime = 0.5f;


    void Update() //rotacion 
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentRotation += RotationSensitivity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentRotation -= RotationSensitivity * Time.deltaTime;
        }
    }
    void FixedUpdate()
    {
        MoveForward();
        Rotation();

        CameraFollow();
    }

    void MoveForward() //serpiente se mueve hacia adelante
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    void Rotation() //rotacion
    {
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, currentRotation));
    }

    void CameraFollow() //la camara
    {
        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;

        Vector3 cameraVelocity = Vector3.zero;

        camera.transform.position = Vector3.SmoothDamp(camera.position,new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10), 
            ref cameraVelocity, smoothTime);

       
    }
}
