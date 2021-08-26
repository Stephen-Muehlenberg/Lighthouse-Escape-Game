#if ENABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM_PACKAGE
#define USE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Controls;
#endif

using Mirror;
using UnityEngine;

public class FirstPersonController : NetworkBehaviour
{
  public float speed = 10f;

  class CameraState
  {
    public float yaw;
    public float pitch;
    public float x;
    public float y;
    public float z;

    public void SetFromTransform(Transform t)
    {
      pitch = t.eulerAngles.x;
      yaw = t.eulerAngles.y;
      x = t.position.x;
      y = t.position.y;
      z = t.position.z;
    }

    public void Translate(Vector3 translation)
    {
      Vector3 rotatedTranslation = Quaternion.Euler(0/*pitch*/, yaw, 0) * translation;

      x += rotatedTranslation.x;
      y += rotatedTranslation.y;
      z += rotatedTranslation.z;
    }

    public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
    {
      yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
      pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);

      x = Mathf.Lerp(x, target.x, positionLerpPct);
      y = Mathf.Lerp(y, target.y, positionLerpPct);
      z = Mathf.Lerp(z, target.z, positionLerpPct);
    }

    public void UpdateTransform(Transform body, Transform head)
    {
      body.eulerAngles = new Vector3(0, yaw, 0);
      head.localEulerAngles = new Vector3(pitch, 0, 0);
      body.position = new Vector3(x, y, z);
    }
  }

  CameraState TargetCameraState = new CameraState();
  CameraState InterpolatingCameraState = new CameraState();

  [Header("Movement Settings")]
  [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
  public float boost = 3.5f;

  [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
  public float positionLerpTime = 0.2f;

  [Header("Rotation Settings")]
  [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
  public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

  [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
  public float rotationLerpTime = 0.01f;

  [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
  public bool invertY = false;

  [Tooltip("Object rotated left and right.")]
  public Transform body;
  [Tooltip("Object rotated up and down.")]
  public Transform head;

  public override void OnStartClient()
  {
    if (transform.position.x < 0)
      this.name = "LEFTY";
    else
      this.name = "RIGHTY";
    UnityEngine.Debug.Log(name + ".OnStartClient() - isLocal ? " + isLocalPlayer);

    if (!isLocalPlayer)
    {
      GetComponentInChildren<Camera>().enabled = false;
      this.enabled = false;
      return;
    }

    TargetCameraState.SetFromTransform(transform);
    InterpolatingCameraState.SetFromTransform(transform);

    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
  }

  Vector3 GetInputTranslationDirection()
  {
    Vector3 direction = new Vector3();
    if (Input.GetKey(KeyCode.W))
    {
      direction += Vector3.forward;
    }
    if (Input.GetKey(KeyCode.S))
    {
      direction += Vector3.back;
    }
    if (Input.GetKey(KeyCode.A))
    {
      direction += Vector3.left;
    }
    if (Input.GetKey(KeyCode.D))
    {
      direction += Vector3.right;
    }
    if (Input.GetKey(KeyCode.Q))
    {
      direction += Vector3.down;
    }
    if (Input.GetKey(KeyCode.E))
    {
      direction += Vector3.up;
    }
    return direction;
  }

  void Update()
  {
#if UNITY_EDITOR
#else
    if (Input.GetKey(KeyCode.Escape))
    {
      Cursor.visible = !Cursor.visible;
      Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
#endif

    if (Input.GetKey(KeyCode.Q))
    {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }

    if (!isLocalPlayer) return;

    var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

    var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

    TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
    TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;

    // Translation
    Vector3 translation = GetInputTranslationDirection() * Time.deltaTime;
    translation *= speed;
    TargetCameraState.Translate(translation);

    // Framerate-independent interpolation
    // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
    var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
    var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
    InterpolatingCameraState.LerpTowards(TargetCameraState, positionLerpPct, rotationLerpPct);

    InterpolatingCameraState.UpdateTransform(body, head);
  }
}
