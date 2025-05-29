using UnityEngine;
using Oculus.Interaction;
using BrandosLab.LabTools.Model;

namespace BrandosLab.LabTools {

	public class ToolsExplainable : MonoBehaviour, IExplainable {

		[SerializeField] private AudioClip m_audioDescription;

		private bool m_isSelected = false;

		public void OnHandleSelectedEvent(PointerEvent pointerEvent) {


			if (pointerEvent.Type == PointerEventType.Select)
				m_isSelected = true;

			else if (pointerEvent.Type == PointerEventType.Unselect)
				m_isSelected = false;

		}

		public AudioClip GetExplanationVoiceOverClip() {
			return m_audioDescription;
		}

		public bool IsCanExplain() => m_isSelected;

	}

}