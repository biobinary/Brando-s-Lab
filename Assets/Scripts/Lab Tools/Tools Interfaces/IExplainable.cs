using UnityEngine;

namespace BrandosLab.LabTools.Model {
    
    public interface IExplainable {

        public bool IsCanExplain();
        public AudioClip GetExplanationVoiceOverClip();

    }

}