using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Views.General
{
    public class ResultPanel : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Text _text;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Colors")]
        [SerializeField] private Color redColor = new Color(0.85f, 0.1f, 0.1f);
        [SerializeField] private Color blackColor = new Color(0.1f, 0.1f, 0.1f);
        [SerializeField] private Color zeroColor = new Color(0.1f, 0.6f, 0.1f);

        [Header("Anim")]
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private float visibleSeconds = 2f;

        Coroutine co;

        public event Action OnEndAnimAction; 
        
        public void ShowResult(int number, bool isRed)
        {
            string colorName = number == 0 ? "GREEN" : (isRed ? "RED" : "BLACK");
            Color uiColor   = number == 0 ? zeroColor : (isRed ? redColor : blackColor);

            _text.text = $"Result: <color=#{ColorUtility.ToHtmlStringRGB(uiColor)}>{number} ({colorName})</color>";

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            
            if (co != null) StopCoroutine(co);
            co = StartCoroutine(FadeRoutine());
        }
        
        private IEnumerator FadeRoutine()
        {
            yield return Fade(0f, 1f, fadeDuration);
            yield return new WaitForSeconds(visibleSeconds);
            yield return Fade(1f, 0f, fadeDuration);
            
            OnEndAnimAction?.Invoke();
            
            gameObject.SetActive(false);
            co = null;
        }

        private IEnumerator Fade(float a, float b, float t)
        {
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
            float time = 0f;
            while (time < t)
            {
                time += Time.deltaTime;
                float k = Mathf.Clamp01(time / t);
                canvasGroup.alpha = Mathf.Lerp(a, b, k);
                yield return null;
            }
            canvasGroup.alpha = b;
        }
    }
}