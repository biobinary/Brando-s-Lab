using UnityEngine;

// Only Works For Blend Tree Only
public class RandomizeAnimationBehaviour : StateMachineBehaviour {

	[SerializeField] private string m_parameterName = "";
	[SerializeField] private Vector2 m_randomizeIntervalRange = new Vector2(3.0f, 5.0f);
	[SerializeField] private int m_totalAnimations = 3;

	private float m_timeUntilNextRandomize = 0.0f;
	private bool m_isRandomizing = false;
	private float m_normalAnimationTimer = 0.0f;
	private int m_targetAnimationIndex = 0;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		ResetAnimation();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		
		if (!m_isRandomizing) {
			
			m_normalAnimationTimer += Time.deltaTime;

			if (m_normalAnimationTimer > m_timeUntilNextRandomize &&
				stateInfo.normalizedTime % 1 < 0.02f) {

				m_isRandomizing = true;
				m_targetAnimationIndex = Random.Range(1, m_totalAnimations + 1);
				m_targetAnimationIndex = m_targetAnimationIndex * 2 - 1;

				animator.SetFloat(m_parameterName, m_targetAnimationIndex - 1);

			}
		
		} else if (stateInfo.normalizedTime % 1 > 0.98f) {
			ResetAnimation();
		
		}

		animator.SetFloat(m_parameterName, m_targetAnimationIndex, 0.2f, Time.deltaTime);
	}

	private void ResetAnimation() {
		
		if (m_isRandomizing)
			m_targetAnimationIndex--;

		m_isRandomizing = false;
		m_normalAnimationTimer = 0.0f;

		m_timeUntilNextRandomize = Random.Range(
			m_randomizeIntervalRange.x,
			m_randomizeIntervalRange.y
		);

	}

}
