using Oculus.Interaction;
using UnityEngine;

namespace BrandosLab.Hand {

	public class HandController : MonoBehaviour {

		[SerializeField] private Animator m_animator;
		[SerializeField] private OVRInput.Controller m_controller;

		[Header("Interactor Settings")]
		[SerializeField] private GrabInteractor m_grabInteractor;
		[SerializeField] private GameObject m_handVisual;

		private int m_gripID = Animator.StringToHash("Grip");
		private int m_triggerID = Animator.StringToHash("Trigger");

		private void OnEnable() {
			if (m_grabInteractor != null) {
				m_grabInteractor.WhenStateChanged += HandleOnGrabInteractorStateChanged;
			}
		}

		private void OnDisable() {
			if (m_grabInteractor != null) {
				m_grabInteractor.WhenStateChanged -= HandleOnGrabInteractorStateChanged;
			}
		}

		private void HandleOnGrabInteractorStateChanged(InteractorStateChangeArgs args) {

			if (args.NewState == InteractorState.Select) {

				if (m_grabInteractor.Interactable != null)
					m_handVisual.SetActive(false);

			} else if (args.PreviousState == InteractorState.Select)
				m_handVisual.SetActive(true);

		}

		private void Update() {

			if (m_animator != null && m_animator.gameObject.activeSelf) {

				m_animator.SetFloat(
					m_gripID,
					OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller));

				m_animator.SetFloat(
					m_triggerID,
					OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller));

			}

		}

	}

}