using UnityEngine;
using System.Collections;

public class PaperMarker : MonoBehaviour {
		/// <summary>
		/// The Tango time stamp when this object is created
		/// 
		/// This field is used in the Area Learning example, the timestamp is save for the position adjustment when the
		/// loop closure happens.
		/// </summary>
		public float m_timestamp = -1.0f;

		/// <summary>
		/// The marker's transformation with respect to the device frame.
		/// </summary>
		public Matrix4x4 m_deviceTMarker = new Matrix4x4();

		/// <summary>
		/// Awake this instance.
		/// </summary>
/*		private void Awake()
		{
			// The animation should be started in Awake and not Start so that it plays on its first frame.
			m_anim = GetComponent<Animation>();
			m_anim.Play("ARMarkerShow", PlayMode.StopAll);
		}

		/// <summary>
		/// Plays an animation, then destroys.
		/// </summary>
		private void Hide()
		{
			m_anim.Play("ARMarkerHide", PlayMode.StopAll);
		}
*/
		/// <summary>
		/// Callback for the animation system.
		/// </summary>
		private void HideDone()
		{
			Destroy(gameObject);
		}
}
