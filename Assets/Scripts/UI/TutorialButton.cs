using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour {

	public PlaygroundTutorials.Tutorial tutorial;

	[Header("Component References")]
	[SerializeField] private TextMeshProUGUI m_tutorialNameLabel;
	[SerializeField] private Toggle m_button;

	private TutorialInfoMenu m_tutorialMenu;

	private void Awake() {

		m_button.onValueChanged.AddListener((value) => {
			if( m_tutorialMenu != null ) {
				m_tutorialMenu.OnHandleTutorialButtonPressed(this);
			}
		});

	}

	private void Start() {
		m_tutorialNameLabel.text = tutorial.tutorialName;
	}

	public void SetupButton( TutorialInfoMenu menu, PlaygroundTutorials.Tutorial tutorial ) {

		m_tutorialMenu = menu;

		this.tutorial = tutorial;
		m_tutorialNameLabel.text = this.tutorial.tutorialName;

	}

}
