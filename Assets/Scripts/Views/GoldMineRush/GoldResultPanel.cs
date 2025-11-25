using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Views.GoldMineRush
{
    public class GoldResultPanel : MonoBehaviour
    {
        [Header("Refs")] [SerializeField] private Text _text;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Anim")] [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private float visibleSeconds = 2f;

        Coroutine co;

        public event Action OnEndAnimAction;

        public void ShowResult()
        {
            _text.text = $"SORRY, MAYBE NEXT TIME!";

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
