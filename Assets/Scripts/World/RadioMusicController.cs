using Oculus.Interaction;
using System.Linq;
using TMPro;
using UnityEngine;

public class RadioMusicController : MonoBehaviour {

	[SerializeField] private TextMeshProUGUI m_volumeLabel;
	[SerializeField] private Transform m_volumeKnob;
	[SerializeField] private GrabInteractable m_grabInteractable;

	private void Update() {

		if (MusicManager.Instance == null || m_grabInteractable == null || m_volumeKnob == null) 
			return;
		
		SetupVolumeLabel();

		if( m_grabInteractable.SelectingInteractorViews.Any() )
			SetNewVolumeLevel();
		else
			SetupVolumeKnob();

	}

	private void SetupVolumeLabel() {
		
		int musicPercentage = (int)(MusicManager.Instance.GetCurrentVolume() * 100.0f);
		m_volumeLabel.text = $"Volume: {musicPercentage}%";
	
	}

	private void SetupVolumeKnob() {

		float newKnobRotation = Mathf.Lerp(-90.0f, 90.0f, MusicManager.Instance.GetCurrentVolume());
		m_volumeKnob.localEulerAngles = new Vector3(
			newKnobRotation,
			m_volumeKnob.localEulerAngles.y,
			m_volumeKnob.localEulerAngles.z 
		);
			
	}

	private void SetNewVolumeLevel() {

		float t = Mathf.InverseLerp(
			-90.0f, 
			90.0f, 
			GetNormalizedAngle(m_volumeKnob.localEulerAngles.x)
		);

		if (Mathf.Approximately(t, 0.0f)) {
			MusicManager.Instance.MuteMusic(true);
		
		} else {
			MusicManager.Instance.MuteMusic(false);
			MusicManager.Instance.SetVolume(t);
		
		}

	}

	private float GetNormalizedAngle(float angle) {
		
		angle %= 360;
		
		if (angle > 180) angle -= 360;
		return angle;

	}

}
