using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth = 12f;
    [SerializeField] private float swayMultiplier = 0.2f;
    private InputManager _inputManager;

    private void Start()
    {
        _inputManager = InputManager.Instance;
    }

    void Update()
    {
        var mouseX = _inputManager.GetMouseDelta().x * swayMultiplier;
        var mouseY = _inputManager.GetMouseDelta().y * swayMultiplier;
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY; 

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
