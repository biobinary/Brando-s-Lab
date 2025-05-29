using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;
using System.Collections.Generic;
using BrandosLab.World;

namespace BrandosLab.Character {

	public class SecondProfessorBehaviour : MonoBehaviour {

		[Header("Audio Settings")]
		[SerializeField] private AudioSource m_audioSource;
		[SerializeField] private Vector2 m_delayBetweenQuotes = new Vector2(5.0f, 10.0f);
		[SerializeField] private List<AudioClip> m_quotes = new();

		[Header("Tracking Settings")]
		[SerializeField] private float m_lookAtPlayerSpeed = 0.3f;
		[SerializeField] private Rig m_headTrackingRig;
		[SerializeField] private Transform m_customHeadTrackingTarget;

		private Transform m_playerTransform;
		private float m_lookAtVelocity = 0.0f;
		private Coroutine m_delayBetweenQuotesCoroutine = null;
		private int m_currentIndex = 0;

		private void Start() {

			if (m_quotes == null || m_quotes.Count == 0) {
				return;
			}

			ShuffleQuotes();

		}

		public void ShuffleQuotes() {

			List<AudioClip> shuffledQuotes = new List<AudioClip>(m_quotes);

			int n = shuffledQuotes.Count;
			for (int i = n - 1; i > 0; i--) {
				int j = Random.Range(0, i + 1);
				AudioClip temp = shuffledQuotes[i];
				shuffledQuotes[i] = shuffledQuotes[j];
				shuffledQuotes[j] = temp;
			}

			m_quotes = shuffledQuotes;

		}

		private void OnTriggerEnter(Collider other) {

			if (!other.CompareTag("Player"))
				return;

			m_playerTransform = other.gameObject.transform;
			m_delayBetweenQuotesCoroutine = StartCoroutine(WaitForNextQuotes());

		}

		private void OnTriggerExit(Collider other) {

			if (!other.CompareTag("Player"))
				return;

			m_playerTransform = null;

			if (m_delayBetweenQuotesCoroutine != null)
				StopCoroutine(m_delayBetweenQuotesCoroutine);

			m_delayBetweenQuotesCoroutine = null;

		}

		private IEnumerator WaitForNextQuotes() {

			yield return new WaitForSeconds(
				Random.Range(m_delayBetweenQuotes.x, m_delayBetweenQuotes.y));

			m_delayBetweenQuotesCoroutine = null;

			StartCoroutine(StartAudioInteraction());

		}

		private IEnumerator StartAudioInteraction() {

			m_audioSource.clip = m_quotes[m_currentIndex];
			m_currentIndex++;
			m_currentIndex = m_currentIndex % m_quotes.Count;

			m_audioSource.Play();
			MusicManager.Instance.MuffleMusic(0.15f, 0.5f);

			while (m_audioSource.isPlaying) {

				if (!MusicManager.Instance.IsOnMuffle())
					MusicManager.Instance.MuffleMusic(0.15f, 0.0f);

				yield return null;

			}

			MusicManager.Instance.CancelMuffle(1.0f);

			if (m_playerTransform != null)
				m_delayBetweenQuotesCoroutine = StartCoroutine(WaitForNextQuotes());

		}

		private void Update() {

			if (m_headTrackingRig == null)
				return;

			if (m_customHeadTrackingTarget == null)
				return;

			if (m_playerTransform == null) {

				float newLookAtValue = Mathf.SmoothDamp(
					m_headTrackingRig.weight, 0.0f, ref m_lookAtVelocity, m_lookAtPlayerSpeed);

				m_headTrackingRig.weight = newLookAtValue;

			} else {

				m_customHeadTrackingTarget.position = m_playerTransform.position;

				float newLookAtValue = Mathf.SmoothDamp(
					m_headTrackingRig.weight, 1.0f, ref m_lookAtVelocity, m_lookAtPlayerSpeed);

				m_headTrackingRig.weight = newLookAtValue;

			}

		}

	}

}