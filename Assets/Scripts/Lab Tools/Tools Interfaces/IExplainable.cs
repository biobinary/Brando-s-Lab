using UnityEngine;

public interface IExplainable {

    public bool IsCanExplain();
    public AudioClip GetExplanationVoiceOverClip();

}
