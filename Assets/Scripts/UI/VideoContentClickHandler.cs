using UnityEngine;
using UnityEngine.EventSystems;

public class VideoContentClickHandler : MonoBehaviour, IPointerClickHandler {

	[Header("Controller")]
	[SerializeField] private VideoPlayerController m_videoPlayerController;
	
	public void OnPointerClick(PointerEventData eventData) {
		
		if( m_videoPlayerController.IsPlayerControlHidden() )
			m_videoPlayerController.ShowPlayerControl();
		
		else
			m_videoPlayerController.HidePlayerControl();

	}

}
