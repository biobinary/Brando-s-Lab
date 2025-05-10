using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Experiment Tutorial Infos", menuName = "Scriptable Objects/Experiment Tutorial Infos")]
public class PlaygroundTutorials : ScriptableObject {

    [System.Serializable]
    public class Tutorial {

		[Tooltip("The display name of the tutorial")]
		public string tutorialName;

		[Tooltip("The video or audio clip associated with the tutorial")]
		public VideoClip tutorialVideo;

	}

    [SerializeField] private List<Tutorial> m_tutorialList;

	public IReadOnlyList<Tutorial> GetAllTutorials() => m_tutorialList;

}
