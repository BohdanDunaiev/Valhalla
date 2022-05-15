using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Variables
    [SerializeField] private float _mouseSensitivity;

    //References
    private Transform parent;

    private void Start()
    {
        parent = transform.parent;
    }

    private void Update()
    {
        RotateHero();
    }

    private void RotateHero()
    {
        float mouseX = Input.GetAxis("Horizontal") * _mouseSensitivity * Time.deltaTime;
        parent.Rotate(Vector3.up, mouseX);
    }
}
