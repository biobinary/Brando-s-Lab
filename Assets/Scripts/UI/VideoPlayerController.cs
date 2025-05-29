using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BrandosLab.UI {

	public class VideoPlayerController : MonoBehaviour {

		[Header("Video Player Components")]
		[SerializeField] private VideoPlayer m_videoPlayer;

		[Header("UI Controls")]
		[SerializeField] private Toggle m_playPauseButton;
		[SerializeField] private Toggle m_skipForwardButton;
		[SerializeField] private Toggle m_skipBackwardButton;
		[SerializeField] private Slider m_progressSlider;
		[SerializeField] private TextMeshProUGUI m_currentTimeText;
		[SerializeField] private TextMeshProUGUI m_totalTimeText;

		[Header("Visual Settings")]
		[SerializeField] private Image m_playPauseButtonImage;
		[SerializeField] private Sprite m_playIcon;
		[SerializeField] private Sprite m_pauseIcon;
		[SerializeField] private CanvasGroup m_playerControl;

		private const float skipDuration = 5.0f;
		private VideoClip m_currentVideoClip;

		private Coroutine m_controlHideFadeCoroutine = null;

		private float m_controlHideMaxDuration = 3.0f;
		private float m_controlHideTimeout = 0.0f;

		private bool m_isControlHidden = false;

		private void Awake() {

			if (m_playPauseButton != null)
				m_playPauseButton.onValueChanged.AddListener(TogglePlayPause);

			if (m_skipForwardButton != null)
				m_skipForwardButton.onValueChanged.AddListener((value) => SkipTime(skipDuration));

			if (m_skipBackwardButton != null)
				m_skipBackwardButton.onValueChanged.AddListener((value) => SkipTime(-skipDuration));

			if (m_progressSlider != null) {
				m_progressSlider.interactable = false;
				m_progressSlider.minValue = 0f;
			}

		}

		private void OnEnable() {

			m_videoPlayer.clip = m_currentVideoClip;

			if (m_videoPlayer != null) {
				m_videoPlayer.prepareCompleted += OnVideoPrepared;
				m_videoPlayer.Prepare();

			} else {
				Debug.LogError("VideoPlayer component not assigned!");

			}

			UpdatePlayPauseIcon();
			ShowPlayerControl();

		}

		private void OnDisable() {
			m_videoPlayer.Stop();
			m_videoPlayer.clip = null;
		}

		public void TogglePlayPause(bool value) {

			if (m_videoPlayer == null) return;

			if (m_videoPlayer.isPlaying)
				m_videoPlayer.Pause();
			else
				m_videoPlayer.Play();

			UpdatePlayPauseIcon();
			ShowPlayerControl();

		}

		private void UpdatePlayPauseIcon() {

			if (m_playPauseButton == null) return;

			m_playPauseButtonImage.sprite = (
				(m_videoPlayer != null && m_videoPlayer.isPlaying) ? m_pauseIcon : m_playIcon);

		}

		public void SkipTime(float seconds) {

			if (m_videoPlayer == null || !m_videoPlayer.isPrepared) return;

			double newTime = m_videoPlayer.time + seconds;
			newTime = Mathf.Clamp((float)newTime, 0f, (float)m_videoPlayer.length);
			m_videoPlayer.time = newTime;

			ShowPlayerControl();

		}

		private void UpdateTimeTexts() {

			if (m_videoPlayer == null || !m_videoPlayer.isPrepared)
				return;

			if (m_currentTimeText != null)
				m_currentTimeText.text = FormatTime(m_videoPlayer.time);

			if (m_totalTimeText != null)
				m_totalTimeText.text = FormatTime(m_videoPlayer.length);

		}

		private string FormatTime(double timeInSeconds) {

			int hours = (int)(timeInSeconds / 3600);
			int minutes = (int)((timeInSeconds % 3600) / 60);
			int seconds = (int)(timeInSeconds % 60);

			if (hours > 0)
				return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);

			else
				return string.Format("{0:D2}:{1:D2}", minutes, seconds);

		}

		private void OnVideoPrepared(VideoPlayer vp) {

			if (m_progressSlider != null)
				m_progressSlider.maxValue = (float)m_videoPlayer.length;

			m_videoPlayer.Play();
			UpdateTimeTexts();
			UpdatePlayPauseIcon();

		}

		private void Update() {

			if (m_videoPlayer == null || !m_videoPlayer.isPrepared)
				return;

			UpdateTimeTexts();

			if (m_progressSlider != null)
				m_progressSlider.value = (float)m_videoPlayer.time;

			if (m_videoPlayer.isPlaying && !m_isControlHidden) {
				m_controlHideTimeout += Time.deltaTime;
				if (m_controlHideTimeout > m_controlHideMaxDuration) {
					m_controlHideFadeCoroutine = StartCoroutine(ControlHideFade());
				}
			}

		}

		public void SetupNewVideo(VideoClip videoClip) {
			m_currentVideoClip = videoClip;
		}

		private IEnumerator ControlHideFade() {

			m_isControlHidden = true;

			float hideDuration = 1.0f;
			float hideTimeout = 0.0f;

			while (hideDuration > hideTimeout) {
				float newAlpha = Mathf.Lerp(1.0f, 0.0f, hideTimeout / hideDuration);
				m_playerControl.alpha = newAlpha;
				hideTimeout += Time.deltaTime;
				yield return null;
			}

			m_playerControl.alpha = 0.0f;
			m_playerControl.interactable = false;
			m_controlHideFadeCoroutine = null;

		}

		public void ShowPlayerControl() {

			if (m_controlHideFadeCoroutine != null)
				StopCoroutine(m_controlHideFadeCoroutine);

			m_controlHideFadeCoroutine = null;

			m_isControlHidden = false;

			m_controlHideTimeout = 0.0f;
			m_playerControl.alpha = 1.0f;
			m_playerControl.interactable = true;
			m_playerControl.blocksRaycasts = true;

		}

		public void HidePlayerControl() {

			m_isControlHidden = true;
			m_playerControl.alpha = 0.0f;
			m_playerControl.interactable = false;
			m_playerControl.blocksRaycasts = false;

		}

		public bool IsPlayerControlHidden() => m_isControlHidden;

	}

}