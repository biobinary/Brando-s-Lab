using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace BrandosLab.Character {

	public class NPCHeadTracking : MonoBehaviour {

		[SerializeField] private GameObject m_npcMainTransform;
		[SerializeField] private Rig m_rig;
		[SerializeField] private GameObject m_customTrackingTarget = null;

		private Transform m_mainCameraTransform = null;

		private void Start() {

			OVRCameraRig cameraRig = FindAnyObjectByType<OVRCameraRig>();
			if (cameraRig != null) {
				m_mainCameraTransform = cameraRig.centerEyeAnchor;
			}

			if (m_rig != null) {

				if (m_mainCameraTransform == null) {
					m_rig.weight = 0.0f;
					return;
				}

				m_rig.weight = 1.0f;

			}

		}

		private void Update() {

			if (m_customTrackingTarget == null) return;
			if (m_mainCameraTransform == null) return;

			m_customTrackingTarget.transform.position = m_mainCameraTransform.position;

		}

	}

}