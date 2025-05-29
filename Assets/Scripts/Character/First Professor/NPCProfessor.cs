using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BrandosLab.Playgrounds;
using BrandosLab.Playgrounds.Objectives;
using BrandosLab.World;

namespace BrandosLab.Character {

	public class NPCProfessor : MonoBehaviour {

		[System.Serializable]
		public class NPCMonologue {
			public string name;
			public AudioClip voiceOverClip;
		}

		public static NPCProfessor Instance;

		[Header("Animation Settings")]
		[SerializeField] private Animator m_npcAnimator;

		[Header("Audio Settings")]
		[SerializeField] private AudioSource m_npcAudioSource;

		[Header("NPC Monologues")]
		[SerializeField] private List<NPCMonologue> monologues = new List<NPCMonologue>();
		private Dictionary<string, NPCMonologue> m_monologuesDict = new Dictionary<string, NPCMonologue>();

		private Coroutine m_monologueProgress = null;
		private bool m_isContinueProgress = false;

		[HideInInspector]
		public bool isSpeaking {

			get {

				if (m_npcAudioSource != null)
					return m_npcAudioSource.isPlaying;

				return false;

			}

		}

		private void Awake() {

			if (Instance == null) {
				Instance = this;

			} else if (Instance != null && Instance != this) {
				Destroy(gameObject);
				return;

			}

			foreach (NPCMonologue monologue in monologues) {
				if (!m_monologuesDict.TryGetValue(monologue.name, out _) && monologue.voiceOverClip != null)
					m_monologuesDict.Add(monologue.name, monologue);
			}

			PlaygroundEnvironmentManager.Instance.OnLoadEnvironment += OnHandleLoadNewEnvironment;

		}

		private void OnTriggerEnter(Collider other) {

			if (!other.CompareTag("Player"))
				return;

			if (m_isContinueProgress) {
				m_monologueProgress = StartCoroutine(StartMonologueProgress());
				m_isContinueProgress = false;
			}

		}

		private void OnTriggerExit(Collider other) {

			if (!other.CompareTag("Player"))
				return;

			if (m_monologueProgress != null) {

				StopCoroutine(m_monologueProgress);
				m_monologueProgress = null;

				MusicManager.Instance.CancelMuffle(1.0f);

				if (m_npcAudioSource != null)
					m_npcAudioSource.Pause();

				m_isContinueProgress = true;

			}

		}

		private void OnHandleLoadNewEnvironment(PlaygroundEnvironment env) {

			if (env == null)
				return;

			PlaygroundObjective objective = env.GetObjective();
			if (objective == null)
				return;

			PlayMonologue(objective.GetIntroductionMonologueName(), true);

		}

		private IEnumerator Start() {

			yield return new WaitForSeconds(5.0f);

			if (m_npcAudioSource != null) {
				if (!m_npcAudioSource.isPlaying)
					PlayMonologue("Introduction");
			}

		}

		public void PlayMonologue(string monologueName, bool isOnlyPlayOnce = false) {

			if (m_monologuesDict.TryGetValue(monologueName, out NPCMonologue monologue)) {

				if (m_npcAudioSource != null) {

					if (m_monologueProgress != null)
						StopCoroutine(m_monologueProgress);

					if (m_npcAudioSource.isPlaying)
						m_npcAudioSource.Stop();

					m_npcAudioSource.clip = monologue.voiceOverClip;
					m_monologueProgress = StartCoroutine(StartMonologueProgress());

				}

				if (isOnlyPlayOnce)
					m_monologuesDict.Remove(monologueName);

			}

		}

		public void PlayMonologue(AudioClip voiceOverClip) {

			if (voiceOverClip == null) return;
			if (m_npcAudioSource == null) return;

			if (m_monologueProgress != null)
				StopCoroutine(m_monologueProgress);

			if (m_npcAudioSource.isPlaying)
				m_npcAudioSource.Stop();

			m_npcAudioSource.clip = voiceOverClip;
			m_monologueProgress = StartCoroutine(StartMonologueProgress());

		}

		private IEnumerator StartMonologueProgress() {

			m_npcAudioSource.Play();
			MusicManager.Instance.MuffleMusic(0.15f, 0.5f);

			while (m_npcAudioSource.isPlaying) {

				if (!MusicManager.Instance.IsOnMuffle())
					MusicManager.Instance.MuffleMusic(0.15f, 0.0f);

				yield return null;

			}

			MusicManager.Instance.CancelMuffle(1.0f);
			m_monologueProgress = null;

		}

		private void Update() {

			if (m_npcAudioSource == null) return;
			if (m_npcAnimator == null) return;

			if (m_npcAudioSource.isPlaying && !m_npcAnimator.GetBool("isExplaining"))
				m_npcAnimator.SetBool("isExplaining", true);

			else if (!m_npcAudioSource.isPlaying && m_npcAnimator.GetBool("isExplaining"))
				m_npcAnimator.SetBool("isExplaining", false);

		}

	}

}