using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public abstract class TriggerBasedTool : MonoBehaviour {

	[SerializeField] protected GrabInteractable m_mainGrabbable;

	protected ControllerRef m_currentControllerRef = null;

	private void OnEnable() {
		m_mainGrabbable.WhenSelectingInteractorViewAdded += OnHandleSelectedByInteractor;
		m_mainGrabbable.WhenSelectingInteractorViewRemoved += OnHandleUnselectedByInteractor;
	}

	private void OnDisable() {
		m_currentControllerRef = null;
		m_mainGrabbable.WhenSelectingInteractorViewAdded -= OnHandleSelectedByInteractor;
		m_mainGrabbable.WhenSelectingInteractorViewRemoved -= OnHandleUnselectedByInteractor;

	}

	private void OnHandleSelectedByInteractor(IInteractorView interactor) {

		object interactorData = interactor.Data;
		GrabInteractor grabInteractor = interactorData as GrabInteractor;
		ControllerRef controllerRef = grabInteractor.GetComponent<ControllerRef>();

		if (controllerRef != null)
			m_currentControllerRef = controllerRef;

	}

	private void OnHandleUnselectedByInteractor(IInteractorView interactor) {
		m_currentControllerRef = null;
	}

	protected virtual void Update() {

		if (m_currentControllerRef != null) {
			if (m_currentControllerRef.ControllerInput.TriggerButton) {
				OnHandleTriggerPressed();
				return;
			}

		}

	}

	protected abstract void OnHandleTriggerPressed();

}
