using Oculus.Interaction;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BrandosLab.World {

	public class RadioMusicController : MonoBehaviour {

		[SerializeField] private TextMeshProUGUI m_volumeLabel;
		[SerializeField] private Transform m_volumeKnob;
		[SerializeField] private GrabInteractable m_grabInteractable;

		[Header("Visual Settings")]
		[SerializeField] private ParticleSystem m_musicalSpeakerLeft;
		[SerializeField] private ParticleSystem m_musicalSpeakerRight;

		private bool m_isEffectPlaying = false;

		private void Update() {

			if (MusicManager.Instance == null || m_grabInteractable == null || m_volumeKnob == null)
				return;

			SetupVolumeLabel();

			if (m_grabInteractable.SelectingInteractorViews.Any())
				SetNewVolumeLevel();
			else
				SetupVolumeKnob();

			if (MusicManager.Instance.IsPlaying() && !m_isEffectPlaying) {
				m_isEffectPlaying = true;
				m_musicalSpeakerLeft.Play();
				m_musicalSpeakerRight.Play();

			} else if (!MusicManager.Instance.IsPlaying() && m_isEffectPlaying) {
				m_isEffectPlaying = false;
				m_musicalSpeakerLeft.Stop();
				m_musicalSpeakerRight.Stop();

			}

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

			if (t < 0.01f) {
				MusicManager.Instance.MuteMusic(true);
				MusicManager.Instance.SetVolume(0.0f);

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

}