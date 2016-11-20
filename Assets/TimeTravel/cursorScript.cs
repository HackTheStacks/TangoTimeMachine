using System;
using System.Collections;
using System.Collections.Generic;
using Tango;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// </summary>
public class cursorScript : MonoBehaviour, ITangoLifecycle, ITangoDepth
{
	// Constant value for controlling the position and size of debug overlay.
	public const float UI_LABEL_START_X = 15.0f;
	public const float UI_LABEL_START_Y = 15.0f;
	public const float UI_LABEL_SIZE_X = 1920.0f;
	public const float UI_LABEL_SIZE_Y = 35.0f;
	public const float UI_LABEL_GAP_Y = 3.0f;
	public const float UI_BUTTON_SIZE_X = 250.0f;
	public const float UI_BUTTON_SIZE_Y = 130.0f;
	public const float UI_BUTTON_GAP_X = 5.0f;
	public const float UI_CAMERA_BUTTON_OFFSET = UI_BUTTON_SIZE_X + UI_BUTTON_GAP_X;
	public const float UI_LABEL_OFFSET = UI_LABEL_GAP_Y + UI_LABEL_SIZE_Y;
	public const float UI_FPS_LABEL_START_Y = UI_LABEL_START_Y + UI_LABEL_OFFSET;
	public const float UI_EVENT_LABEL_START_Y = UI_FPS_LABEL_START_Y + UI_LABEL_OFFSET;
	public const float UI_POSE_LABEL_START_Y = UI_EVENT_LABEL_START_Y + UI_LABEL_OFFSET;
	public const float UI_DEPTH_LABLE_START_Y = UI_POSE_LABEL_START_Y + UI_LABEL_OFFSET;
	public const string UI_FLOAT_FORMAT = "F3";
	public const string UI_FONT_SIZE = "<size=25>";

	public const float UI_TANGO_VERSION_X = UI_LABEL_START_X;
	public const float UI_TANGO_VERSION_Y = UI_LABEL_START_Y;
	public const float UI_TANGO_APP_SPECIFIC_START_X = UI_TANGO_VERSION_X;
	public const float UI_TANGO_APP_SPECIFIC_START_Y = UI_TANGO_VERSION_Y + (UI_LABEL_OFFSET * 2);

	public const string UX_SERVICE_VERSION = "Service version: {0}";
	public const string UX_TANGO_SERVICE_VERSION = "Tango service version: {0}";
	public const string UX_TANGO_SYSTEM_EVENT = "Tango system event: {0}";
	public const string UX_TARGET_TO_BASE_FRAME = "Target->{0}, Base->{1}:";
	public const string UX_STATUS = "\tstatus: {0}, count: {1}, position (m): [{2}], orientation: [{3}]";
	public const float SECOND_TO_MILLISECOND = 1000.0f;

	/// <summary>
	/// The marker prefab to place on taps.
	/// </summary>
	public GameObject m_prefabMarker;

	/// <summary>
	/// The touch effect to place on taps.
	/// </summary>
	public RectTransform m_prefabTouchEffect;

	/// <summary>
	/// The canvas to place 2D game objects under.
	/// </summary>
	public Canvas m_canvas;

	/// <summary>
	/// The point cloud object in the scene.
	/// </summary>
	public TangoPointCloud m_pointCloud;

	public Text debugText;

	private const float FPS_UPDATE_FREQUENCY = 1.0f;
	private string m_fpsText;
	private int m_currentFPS;
	private int m_framesSinceUpdate;
	private float m_accumulation;
	private float m_currentTime;

	private TangoApplication m_tangoApplication;
	private TangoARPoseController m_tangoPose;
	private string m_tangoServiceVersion;
	private ARCameraPostProcess m_arCameraPostProcess;

	/// <summary>
	/// If set, then the depth camera is on and we are waiting for the next depth update.
	/// </summary>
	private bool m_findPlaneWaitingForDepth;

	/// <summary>
	/// If set, this is the selected marker.
	/// </summary>
	private PaperMarker m_selectedMarker;

	/// <summary>
	/// If set, this is the rectangle bounding the selected marker.
	/// </summary>
	private Rect m_selectedRect;

	/// <summary>
	/// If set, this is the rectangle for the Hide All button.
	/// </summary>
	private Rect m_hideAllRect;

	/// <summary>
	/// If set, show debug text.
	/// </summary>
	private bool m_showDebug = false;

	/// <summary>
	/// Unity Start() callback, we set up some initial values here.
	/// </summary>
	public void Start()
	{
		m_currentFPS = 0;
		m_framesSinceUpdate = 0;
		m_currentTime = 0.0f;
		m_fpsText = "FPS = Calculating";
		m_tangoApplication = FindObjectOfType<TangoApplication>();
		m_tangoPose = FindObjectOfType<TangoARPoseController>();
		m_arCameraPostProcess = FindObjectOfType<ARCameraPostProcess>();
		m_tangoServiceVersion = TangoApplication.GetTangoServiceVersion();

		m_tangoApplication.Register(this);
	}

	/// <summary>
	/// Unity destroy function.
	/// </summary>
	public void OnDestroy()
	{
		m_tangoApplication.Unregister(this);
	}

	/// <summary>
	/// Updates UI and handles player input.
	/// </summary>
	public void Update()
	{
		m_currentTime += Time.deltaTime;
		++m_framesSinceUpdate;
		m_accumulation += Time.timeScale / Time.deltaTime;
		if (m_currentTime >= FPS_UPDATE_FREQUENCY)
		{
			m_currentFPS = (int)(m_accumulation / m_framesSinceUpdate);
			m_currentTime = 0.0f;
			m_framesSinceUpdate = 0;
			m_accumulation = 0.0f;
			m_fpsText = "FPS: " + m_currentFPS;
		}

		_UpdateLocationMarker();

		if (Input.GetKey(KeyCode.Escape))
		{
			// This is a fix for a lifecycle issue where calling
			// Application.Quit() here, and restarting the application
			// immediately results in a deadlocked app.
			AndroidHelper.AndroidQuit();
		}
	}
		

	/// <summary>
	/// This is called when the permission granting process is finished.
	/// </summary>
	/// <param name="permissionsGranted"><c>true</c> if permissions were granted, otherwise <c>false</c>.</param>
	public void OnTangoPermissions(bool permissionsGranted)
	{
	}

	/// <summary>
	/// This is called when successfully connected to the Tango service.
	/// </summary>
	public void OnTangoServiceConnected()
	{
		m_tangoApplication.SetDepthCameraRate(TangoEnums.TangoDepthCameraRate.DISABLED);
	}

	/// <summary>
	/// This is called when disconnected from the Tango service.
	/// </summary>
	public void OnTangoServiceDisconnected()
	{
	}

	/// <summary>
	/// This is called each time new depth data is available.
	/// 
	/// On the Tango tablet, the depth callback occurs at 5 Hz.
	/// </summary>
	/// <param name="tangoDepth">Tango depth.</param>
	public void OnTangoDepthAvailable(TangoUnityDepth tangoDepth)
	{
		// Don't handle depth here because the PointCloud may not have been updated yet.  Just
		// tell the coroutine it can continue.
		m_findPlaneWaitingForDepth = false;
	}

	/// <summary>
	/// Convert a 3D bounding box into a 2D rectangle.
	/// </summary>
	/// <returns>The 2D rectangle in Screen coordinates.</returns>
	/// <param name="cam">Camera to use.</param>
	/// <param name="bounds">3D bounding box.</param>
	private Rect WorldBoundsToScreen(Camera cam, Bounds bounds)
	{
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		Bounds screenBounds = new Bounds(cam.WorldToScreenPoint(center), Vector3.zero);

		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(+extents.x, +extents.y, +extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(+extents.x, +extents.y, -extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(+extents.x, -extents.y, +extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(+extents.x, -extents.y, -extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(-extents.x, +extents.y, +extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(-extents.x, +extents.y, -extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(-extents.x, -extents.y, +extents.z)));
		screenBounds.Encapsulate(cam.WorldToScreenPoint(center + new Vector3(-extents.x, -extents.y, -extents.z)));
		return Rect.MinMaxRect(screenBounds.min.x, screenBounds.min.y, screenBounds.max.x, screenBounds.max.y);
	}

	/// <summary>
	/// Construct readable string from TangoPoseStatusType.
	/// </summary>
	/// <param name="status">Pose status from Tango.</param>
	/// <returns>Readable string corresponding to status.</returns>
	private string _GetLoggingStringFromPoseStatus(TangoEnums.TangoPoseStatusType status)
	{
		string statusString;
		switch (status)
		{
		case TangoEnums.TangoPoseStatusType.TANGO_POSE_INITIALIZING:
			statusString = "initializing";
			break;
		case TangoEnums.TangoPoseStatusType.TANGO_POSE_INVALID:
			statusString = "invalid";
			break;
		case TangoEnums.TangoPoseStatusType.TANGO_POSE_UNKNOWN:
			statusString = "unknown";
			break;
		case TangoEnums.TangoPoseStatusType.TANGO_POSE_VALID:
			statusString = "valid";
			break;
		default:
			statusString = "N/A";
			break;
		}

		return statusString;
	}

	/// <summary>
	/// Reformat string from vector3 type for data logging.
	/// </summary>
	/// <param name="vec">Position to display.</param>
	/// <returns>Readable string corresponding to <c>vec</c>.</returns>
	private string _GetLoggingStringFromVec3(Vector3 vec)
	{
		if (vec == Vector3.zero)
		{
			return "N/A";
		}
		else
		{
			return string.Format("{0}, {1}, {2}",
				vec.x.ToString(UI_FLOAT_FORMAT),
				vec.y.ToString(UI_FLOAT_FORMAT),
				vec.z.ToString(UI_FLOAT_FORMAT));
		}
	}

	/// <summary>
	/// Reformat string from quaternion type for data logging.
	/// </summary>
	/// <param name="quat">Quaternion to display.</param>
	/// <returns>Readable string corresponding to <c>quat</c>.</returns>
	private string _GetLoggingStringFromQuaternion(Quaternion quat)
	{
		if (quat == Quaternion.identity)
		{
			return "N/A";
		}
		else
		{
			return string.Format("{0}, {1}, {2}, {3}",
				quat.x.ToString(UI_FLOAT_FORMAT),
				quat.y.ToString(UI_FLOAT_FORMAT),
				quat.z.ToString(UI_FLOAT_FORMAT),
				quat.w.ToString(UI_FLOAT_FORMAT));
		}
	}

	/// <summary>
	/// Return a string to the get logging from frame count.
	/// </summary>
	/// <returns>The get logging string from frame count.</returns>
	/// <param name="frameCount">Frame count.</param>
	private string _GetLoggingStringFromFrameCount(int frameCount)
	{
		if (frameCount == -1.0)
		{
			return "N/A";
		}
		else
		{
			return frameCount.ToString();
		}
	}

	/// <summary>
	/// Return a string to get logging of FrameDeltaTime.
	/// </summary>
	/// <returns>The logging string for frame delta time.</returns>
	/// <param name="frameDeltaTime">Frame delta time.</param>
	private string _GetLoggingStringFromFrameDeltaTime(float frameDeltaTime)
	{
		if (frameDeltaTime == -1.0)
		{
			return "N/A";
		}
		else
		{
			return (frameDeltaTime * SECOND_TO_MILLISECOND).ToString(UI_FLOAT_FORMAT);
		}
	}

	/// <summary>
	/// Update location marker state.
	/// </summary>
	private void _UpdateLocationMarker()
	{
		if (Input.touchCount == 1)
		{
			// Single tap -- place new location or select existing location.
			Touch t = Input.GetTouch(0);
			Vector2 guiPosition = new Vector2(t.position.x, Screen.height - t.position.y);
			Camera cam = Camera.main;

			if (t.phase != TouchPhase.Began) {
				return;
			}

			if (m_selectedRect.Contains(guiPosition) || m_hideAllRect.Contains(guiPosition)) {
				// do nothing, the button will handle it
			} else {
				// Place a new point at that location, clear selection
				m_selectedMarker = null;
				StartCoroutine(_WaitForDepthAndFindPlane(t.position));

				// Because we may wait a small amount of time, this is a good place to play a small
				// animation so the user knows that their input was received.
				RectTransform touchEffectRectTransform = (RectTransform)Instantiate(m_prefabTouchEffect);
				touchEffectRectTransform.transform.SetParent(m_canvas.transform, false);
				Vector2 normalizedPosition = t.position;
				normalizedPosition.x /= Screen.width;
				normalizedPosition.y /= Screen.height;
				touchEffectRectTransform.anchorMin = touchEffectRectTransform.anchorMax = normalizedPosition;
			}
		}

		if (Input.touchCount == 2)
		{
			// Two taps -- toggle debug text
			Touch t0 = Input.GetTouch(0);
			Touch t1 = Input.GetTouch(1);

			if (t0.phase != TouchPhase.Began && t1.phase != TouchPhase.Began)
			{
				return;
			}

			m_showDebug = !m_showDebug;
			return;
		}

		if (Input.touchCount != 1)
		{
			return;
		}
	}

	/// <summary>
	/// Wait for the next depth update, then find the plane at the touch position.
	/// </summary>
	/// <returns>Coroutine IEnumerator.</returns>
	/// <param name="touchPosition">Touch position to find a plane at.</param>
	private IEnumerator _WaitForDepthAndFindPlane(Vector2 touchPosition)
	{
		m_findPlaneWaitingForDepth = true;

		// Turn on the camera and wait for a single depth update.
		m_tangoApplication.SetDepthCameraRate(TangoEnums.TangoDepthCameraRate.MAXIMUM);

		while (m_findPlaneWaitingForDepth)
		{
			yield return null;
		}

		m_tangoApplication.SetDepthCameraRate(TangoEnums.TangoDepthCameraRate.DISABLED);

		// Find the plane.
		Camera cam = Camera.main;
		Vector3 planeCenter;
		Plane plane;
		if (!m_pointCloud.FindPlane(cam, touchPosition, out planeCenter, out plane))
		{
			yield break;
		}

		Vector3 up = plane.normal;
		Vector3 forward;
		if (Vector3.Angle(plane.normal, cam.transform.forward) < 175)
		{
			Vector3 right = Vector3.Cross(up, cam.transform.forward).normalized;
			forward = Vector3.Cross(right, up).normalized;
		}
		else
		{
			// Normal is nearly parallel to camera look direction, the cross product would have too much
			// floating point error in it.
			forward = Vector3.Cross(up, cam.transform.right);
		}

		Instantiate(m_prefabMarker, planeCenter, Quaternion.LookRotation(forward, up));
		m_selectedMarker = null;
	}
}
