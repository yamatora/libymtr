using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {
    private const float FOV_MAX = 120f;
    private const float FOV_MIN = 60f;
    //  fix later: change to variable
    private const float ROTATE_SPEED = 0.2f;
    private const float PINCH_SPEED = 0.5f;

    public Transform target;
    private Camera m_camera;

    public bool IsReset { get; private set; } = false;
    //public bool IsRotate { get; private set; } = true;

    //  Pinch in/out
    private bool m_isMultiTouching = false;
    private float m_currentDistance = 0f;
    private bool m_isMouseLeftDown = false;
    private Vector3 m_baseMousePos = new Vector3();
    private Vector3 m_baseCamRot = new Vector3();
    private float m_baseScrollX = 0f;

    private bool IsMultiTouching {
        get {
            return m_isMultiTouching;
        }
        set {
            if (!m_isMultiTouching && value) {  //  Start
                m_currentDistance = PinchDistance;
            }
            m_isMultiTouching = value;
        }
    }
    private float PinchDistance {
        get {
            return Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
        }
    }
    private float FOV {
        get {
            if (m_camera == null) {
                return -1;
            }
            return m_camera.fieldOfView;
        }
        set {
            if (m_camera == null || m_camera.fieldOfView == value) {
                return;
            }
            if (value > FOV_MAX) {
                value = FOV_MAX;
            } else if (value < FOV_MIN) {
                value = FOV_MIN;
            }
            m_camera.fieldOfView = value;
            m_camera.transform.localPosition = new Vector3(0, 0, -(value - 60f) / 5f);
        }
    }

    void Start() {
        m_camera = Camera.main;
        target.rotation = Quaternion.Euler(0, 0, 0);
        m_baseCamRot = target.rotation.eulerAngles;

    }
    void Update() {
        UpdateInput();
    }
    private void UpdateInput() {
        //  Mouse wheel(FOV zoom in/out)
        IsMultiTouching = (Input.touchCount >= 2);
        float diffFOV = 0;
        if (IsMultiTouching) {
            float distance = PinchDistance;
            diffFOV = (distance - m_currentDistance) * PINCH_SPEED;
            m_currentDistance = distance;
        } else {
            diffFOV = Input.mouseScrollDelta.y;
        }
        //  Reflect
        FOV -= diffFOV;

        //  Mouse drag
        if (IsReset) {
            target.localRotation = Quaternion.Euler(0, 0, 0);
            m_baseCamRot = target.rotation.eulerAngles;
            m_baseMousePos = Input.mousePosition;
            IsReset = false;
        }
        if (Input.GetMouseButtonDown(0)) {
            m_isMouseLeftDown = true;
            m_baseMousePos = Input.mousePosition;
        } else if (m_isMouseLeftDown) {
            Vector3 diff = Input.mousePosition - m_baseMousePos;
            Vector3 rot = new Vector3(diff.y / 2f, -diff.x, 0);
            if (Input.GetMouseButtonUp(0) || m_isMultiTouching) {
                m_isMouseLeftDown = false;
                m_baseCamRot += rot * ROTATE_SPEED;
                return;
            }
            diff *= ROTATE_SPEED;
            target.localRotation = Quaternion.Euler(m_baseCamRot + rot);
        }
    }
}
