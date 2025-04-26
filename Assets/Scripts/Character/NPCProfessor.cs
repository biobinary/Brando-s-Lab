using System.Collections.Generic;
using UnityEngine;

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

		} else if(Instance != null && Instance != this) {
			Destroy(gameObject);

		}

		foreach(NPCMonologue monologue in monologues) {
			if( !m_monologuesDict.TryGetValue(monologue.name, out _) && monologue.voiceOverClip != null)
				m_monologuesDict.Add(monologue.name, monologue);
		}

	}

	private void Start() {
		// PlayMonologue("Introduction");
	}

	public void PlayMonologue( string monologueName, bool isOnlyPlayOnce = false ) {
		
		if (m_monologuesDict.TryGetValue(monologueName, out NPCMonologue monologue)) {

			if (m_npcAudioSource != null) {
				
				if( m_npcAudioSource.isPlaying)
					m_npcAudioSource.Stop();

				m_npcAudioSource.clip = monologue.voiceOverClip;
				m_npcAudioSource.Play();

			}
			
			if( isOnlyPlayOnce )
				m_monologuesDict.Remove(monologueName);
		
		}

	}

	public void PlayMonologue(AudioClip voiceOverClip) {

		if (voiceOverClip == null) return;
		if (m_npcAudioSource == null) return;

		if (m_npcAudioSource.isPlaying)
			m_npcAudioSource.Stop();

		m_npcAudioSource.clip = voiceOverClip;
		m_npcAudioSource.Play();

	}

	private void Update() {
		
		if(m_npcAudioSource == null) return;
		if(m_npcAnimator == null) return;

		if (m_npcAudioSource.isPlaying && !m_npcAnimator.GetBool("isExplaining"))
			m_npcAnimator.SetBool("isExplaining", true);

		else if (!m_npcAudioSource.isPlaying && m_npcAnimator.GetBool("isExplaining"))
			m_npcAnimator.SetBool("isExplaining", false);

	}

}
