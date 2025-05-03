using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

	[Header("Volume Setting Components")]
	[SerializeField] private Slider m_volumeSlider;

	private void Awake() {
		
		m_volumeSlider.onValueChanged.AddListener((value) =>  {

			if (MusicManager.Instance != null) {

				if (Mathf.Approximately(value, 0f))
					MusicManager.Instance.MuteMusic(true);
				
				else {
					MusicManager.Instance.MuteMusic(false);
					MusicManager.Instance.SetVolume(value);
				
				}

			}

		});

	}

	private void OnEnable() {
		if( MusicManager.Instance != null )
			m_volumeSlider.value = MusicManager.Instance.GetCurrentVolume();
	}

}
