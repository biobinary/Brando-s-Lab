using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {

	public static MusicManager Instance { get; private set; }

	[System.Serializable]
	public class MusicTrack {
		public string trackName;
		public AudioClip clip;
		[Range(0f, 1f)]
		public float volume = 1.0f;
		[Range(0f, 1f)]
		public float pitch = 1.0f;
		public bool loop = true;
	}

	[Header("Music Settings")]
	[SerializeField] private List<MusicTrack> m_musicTracks = new List<MusicTrack>();
	[SerializeField] private string m_defaultTrack = "";

	private Dictionary<string, MusicTrack> m_trackDictionary = new Dictionary<string, MusicTrack>();

	private AudioSource m_audioSource = null;

	private string m_currentTrackName = "";
	private bool m_isMuted = false;
	
	private float m_savedVolume = 1.0f;
	private float m_muffleVolume = 0.0f;

	private bool m_isOnMuffle = false;

	private Coroutine m_volumeFadeCoroutine = null;

	private void Awake() {
		
		if (Instance == null) {

			Instance = this;
			DontDestroyOnLoad(gameObject);

			m_audioSource = gameObject.GetComponent<AudioSource>();

			foreach (MusicTrack track in m_musicTracks) {
				
				if (m_trackDictionary.TryGetValue(track.trackName, out _))
					Debug.LogWarning($"Duplicate track name found: {track.trackName}. Only the first one will be used.");
				
				else
					m_trackDictionary.Add(track.trackName, track);

			}

			if (string.IsNullOrEmpty(m_defaultTrack))
				return;

			if (m_trackDictionary.TryGetValue(m_defaultTrack, out MusicTrack musicTrack))
				PlayMusic(m_defaultTrack);

		} else if(Instance != null && Instance != this) {
			Destroy(gameObject);
		
		}

	}

	public void PlayMusic(string trackName) {

		if (m_currentTrackName == trackName && m_audioSource.isPlaying)
			return;

		if (!m_trackDictionary.TryGetValue(trackName, out _)) {
			Debug.LogWarning($"Track {trackName} not found in Music Manager.");
			return;
		}

		MusicTrack track = m_trackDictionary[trackName];

		if (m_audioSource.isPlaying)
			m_audioSource.Stop();

		m_audioSource.clip = track.clip;
		m_audioSource.pitch = track.pitch;
		m_audioSource.loop = track.loop;
		
		SetVolume(track.volume);

		m_audioSource.Play();
		m_currentTrackName = trackName;

	}

	public void StopMusic() {
		m_audioSource.Stop();
		m_currentTrackName = "";
	}

	public void SetVolume(float volume) {

		volume = Mathf.Clamp01(volume);
		m_savedVolume = volume;

		if ( !m_isMuted && m_volumeFadeCoroutine == null) {
			
			if( m_isOnMuffle ) {
				
				if (m_savedVolume < m_muffleVolume)
					m_audioSource.volume = m_savedVolume;
				
				else
					m_audioSource.volume = m_muffleVolume;

			} else {
				m_audioSource.volume = m_savedVolume;
			
			}
		}

	}

	public void MuffleMusic(float targetVolume, float fadeDuration) {

		targetVolume = Mathf.Clamp01(targetVolume);
		if (targetVolume > m_savedVolume)
			return;

		m_isOnMuffle = true;
		m_muffleVolume = targetVolume;

		if (fadeDuration <= 0 || m_isMuted) {
			
			if (!m_isMuted) {
				m_audioSource.volume = targetVolume;
			}
			
			return;

		}

		if (m_volumeFadeCoroutine != null) {
			StopCoroutine(m_volumeFadeCoroutine);
		}

		m_volumeFadeCoroutine = StartCoroutine(FadeVolumeCoroutine(targetVolume, fadeDuration));

	}

	public void CancelMuffle(float fadeDuration) {

		m_isOnMuffle = false;

		if (fadeDuration <= 0 || m_isMuted) {
			
			if (!m_isMuted) {
				m_audioSource.volume = m_savedVolume;
			
			}
			
			return;

		}


		if (m_volumeFadeCoroutine != null) {
			StopCoroutine(m_volumeFadeCoroutine);
		}

		m_volumeFadeCoroutine = StartCoroutine(FadeVolumeCoroutine(m_savedVolume, fadeDuration));

	}

	private IEnumerator FadeVolumeCoroutine(float targetVolume, float duration) {

		float startVolume = m_audioSource.volume;
		float startTime = Time.time;
		float endTime = startTime + duration;

		if (m_isMuted) {
			m_volumeFadeCoroutine = null;
			yield break;

		}

		while (Time.time < endTime) {

			float t = (Time.time - startTime) / duration;
			float currentVolume = Mathf.Lerp(startVolume, targetVolume, t);

			m_audioSource.volume = currentVolume;

			yield return null;

		}

		m_audioSource.volume = targetVolume;
		m_volumeFadeCoroutine = null;

	}

	public void MuteMusic(bool mute) {
		
		m_isMuted = mute;

		if (mute) {

			if (m_volumeFadeCoroutine != null) {
				StopCoroutine(m_volumeFadeCoroutine);
				m_volumeFadeCoroutine = null;
			}

			m_audioSource.volume = 0f;

		} else {

			if( m_isOnMuffle && m_savedVolume > m_muffleVolume)
				m_audioSource.volume = m_muffleVolume;
			else
				m_audioSource.volume = m_savedVolume;

		}

	}

	public float GetCurrentVolume() {
		return m_savedVolume;
	}

	public bool IsPlaying() => m_audioSource.isPlaying && !m_isMuted;

	public bool IsOnMuffle() => m_isOnMuffle;

}
