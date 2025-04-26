using Oculus.Interaction;
using System.Collections.Generic;
using UnityEngine;

public class ContainerExplainable : MonoBehaviour, IExplainable {

	[SerializeField] private MonoBehaviour chemicalContainer;

	private bool m_isSelected = false;

	public void OnHandleSelectedEvent(PointerEvent pointerEvent) {

		if (pointerEvent.Type == PointerEventType.Select)
			m_isSelected = true;

		else if (pointerEvent.Type == PointerEventType.Unselect)
			m_isSelected = false;

	}

	public AudioClip GetExplanationVoiceOverClip() {

		if (chemicalContainer is IChemicalContainer<ChemicalData> chemContainer) {
			return GetAudioClipFromContainer(chemContainer);
		
		} else if (chemicalContainer is IChemicalContainer<MetalSaltData> saltContainer) {
			return GetAudioClipFromContainer(saltContainer);
		
		}

		return null;

	}

	private AudioClip GetAudioClipFromContainer<T>(IChemicalContainer<T> container) where T : ScriptableObject, IExplainableChemical {
		
		List<ChemicalPortion<T>> chems = container.GetChemicalContents();
		T chemicalData = chems?.Count > 0 ? chems[0].data : null;

		if (chemicalData != null) {
			chemicalData.hasBeenExplained = true;
			return chemicalData.audioDescription;
		}
		
		return null;

	}

	public bool IsCanExplain() => m_isSelected;

}
