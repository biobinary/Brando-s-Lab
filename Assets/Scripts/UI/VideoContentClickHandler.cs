using UnityEngine;
using UnityEngine.EventSystems;

namespace BrandosLab.UI {

	public class VideoContentClickHandler : MonoBehaviour, IPointerClickHandler {

		[Header("Controller")]
		[SerializeField] private VideoPlayerController m_videoPlayerController;

		public void OnPointerClick(PointerEventData eventData) {

			if (m_videoPlayerController.IsPlayerControlHidden())
				m_videoPlayerController.ShowPlayerControl();

			else
				m_videoPlayerController.HidePlayerControl();

		}

	}

}
