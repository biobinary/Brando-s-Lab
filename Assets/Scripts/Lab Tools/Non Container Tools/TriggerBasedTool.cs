using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public abstract class TriggerBasedTool : MonoBehaviour {

	[SerializeField] protected GrabInteractable m_mainGrabbable;

	private bool m_isSelectedByInteractor = false;
	private bool m_wasTriggerPressedLastFrame = false;

	private void OnEnable() {
		m_mainGrabbable.WhenSelectingInteractorViewAdded += OnHandleSelectedByInteractor;
		m_mainGrabbable.WhenSelectingInteractorViewRemoved += OnHandleUnselectedByInteractor;
	}

	private void OnDisable() {
		m_isSelectedByInteractor = false;
		m_mainGrabbable.WhenSelectingInteractorViewAdded -= OnHandleSelectedByInteractor;
		m_mainGrabbable.WhenSelectingInteractorViewRemoved -= OnHandleUnselectedByInteractor;

	}

	private void OnHandleSelectedByInteractor(IInteractorView interactor) {
		m_isSelectedByInteractor = true;
	}

	private void OnHandleUnselectedByInteractor(IInteractorView interactor) {
		m_isSelectedByInteractor = false;
	}

	protected virtual void Update() {

		if (m_isSelectedByInteractor) {

			bool isPressed = false;

			foreach (IInteractorView interactorView in m_mainGrabbable.SelectingInteractorViews) {
				
				object interactorData = interactorView.Data;
				
				if (interactorData is GrabInteractor grabInteractor) {
					ControllerRef currentController = grabInteractor.GetComponent<ControllerRef>();
					isPressed = currentController.ControllerInput.TriggerButton;
					break;
				}

			}
			
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
