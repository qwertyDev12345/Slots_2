using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Views.GoldMineRush
{
    public class MultiplierView : MonoBehaviour
    {
        [SerializeField] Text label;
        [SerializeField] AnimationCurve pop = AnimationCurve.EaseInOut(0,1, 0.2f,1.25f);
        [SerializeField] float popTime = 0.2f;

        public void SetValue(float x)
        {
            UpdateValue(x);
            
            StopAllCoroutines();
            StartCoroutine(PopRoutine());
        }

        public void UpdateValue(float x)
        {
            label.text = string.Format(CultureInfo.InvariantCulture, "X{0:0.0}", x);
        }

        private IEnumerator PopRoutine()
        {
            Vector3 baseScale = transform.localScale;
            float t = 0f;
            while (t < popTime)
            {
                t += Time.deltaTime;
                float k = pop.Evaluate(t / popTime);
                transform.localScale = baseScale * k;
                yield return null;
            }
            transform.localScale = baseScale;
        }
    }
}