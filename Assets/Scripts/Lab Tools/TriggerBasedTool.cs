using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public abstract class TriggerBasedTool : MonoBehaviour {

	[SerializeField] protected GrabInteractable m_mainGrabbable;

	protected ControllerRef m_currentControllerRef = null;
	private bool m_wasTriggerPressedLastFrame = false;

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
		if (interactorData is GrabInteractor grabInteractor)
			m_currentControllerRef = grabInteractor.GetComponent<ControllerRef>();
		
	
	}

	private void OnHandleUnselectedByInteractor(IInteractorView interactor) {
		m_currentControllerRef = null;
	}

	protected virtual void Update() {

		if (m_currentControllerRef != null) {

			bool isPressed = m_currentControllerRef.ControllerInput.TriggerButton;
			
			if (isPressed && !m_wasTriggerPressedLastFrame) {
				OnHandleTriggerPressed();

			} else if (!isPressed && m_wasTriggerPressedLastFrame) {
				OnHandleTriggerReleased();
			
			}

			m_wasTriggerPressedLastFrame = isPressed;
		
		} else {

			if (m_wasTriggerPressedLastFrame)
				OnHandleTriggerReleased();

			m_wasTriggerPressedLastFrame = false;

		}

	}

	protected abstract void OnHandleTriggerPressed();
	protected abstract void OnHandleTriggerReleased();

}
