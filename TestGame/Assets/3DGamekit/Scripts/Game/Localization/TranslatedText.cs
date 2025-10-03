using TMPro;
using UnityEngine;

namespace Gamekit3D
{
    public class TranslatedText : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public string phraseKey;
        public bool setTextOnStart = true;

        private void Awake()
        {
            if (text == null)
                text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            if (setTextOnStart)
                text.text = Translator.Instance[phraseKey];
        }

        public void SetText()
        {
            text.text = Translator.Instance[phraseKey];
        }
    }
}