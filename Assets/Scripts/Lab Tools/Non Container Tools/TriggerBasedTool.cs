using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public abstract class TriggerBasedTool : MonoBehaviour {

	[SerializeField] protected GrabInteractable m_mainGrabbable;

	private bool m_wasTriggerPressedLastFrame = false;

	protected virtual void Update() {

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

	}

	protected abstract void OnHandleTriggerPressed();
	protected abstract void OnHandleTriggerReleased();

}
