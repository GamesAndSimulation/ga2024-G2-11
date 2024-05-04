using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemationPovExtension : CinemachineExtension
{
    
    [SerializeField] private float clampAngle = 80.0f;
    [SerializeField] private float sensitivity = 10f;

    private InputManager _inputManager;
    private Vector3 _startingRotation;
    
    protected override void Awake()
    {
        _inputManager = InputManager.Instance;
        _startingRotation = transform.localRotation.eulerAngles;
        base.Awake();
    }
    
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var deltaInput = _inputManager.GetMouseDelta();
                _startingRotation.x += deltaInput.x * sensitivity * Time.deltaTime;
                _startingRotation.y += deltaInput.y * sensitivity * Time.deltaTime;
                _startingRotation.y = Mathf.Clamp(_startingRotation.y, -clampAngle, clampAngle);
                state.RawOrientation = Quaternion.Euler(_startingRotation.y, _startingRotation.x, 0f);
            }
        }
    }
}
