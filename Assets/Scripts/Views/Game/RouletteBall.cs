using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace Views.Game
{
    public class RouletteBall : MonoBehaviour
    {
        public Transform wheel;
        public float radiusStart = 2.05f;
        public float radiusFinish = 1.65f;
        [Range(-10f, 10f)] public float zeroOffsetDeg = 0f;

        public float spinDuration = 3.0f;
        public int minRevolutions = 3;
        public int maxRevolutions = 6;

        public bool rotateWheel = true;
        public float wheelRevPerSec = 1.0f;

        private readonly int[] wheelNumbers =
        {
            0, 32, 15, 19, 4, 21, 2, 25, 17, 34,
            6, 27, 13, 36, 11, 30, 8, 23, 10, 5,
            24, 16, 33, 1, 20, 14, 31, 9, 22, 18,
            29, 7, 28, 12, 35, 3, 26
        };

        private float sectorSize;
        private float angleTopCW;
        private Coroutine spinCo;

        public event Action<int> OnEndSpinAction; 

        void Awake()
        {
            sectorSize = 360f / wheelNumbers.Length;
        }
        
        void Update()
        {
            if (rotateWheel && spinCo != null)
                wheel.Rotate(0f, 0f, wheelRevPerSec * 360f * Time.deltaTime);
        }

        public void SpinRandom()
        {
            int idx = Random.Range(0, wheelNumbers.Length);
            SpinToNumber(wheelNumbers[idx]);
        }

        private void SpinToNumber(int number)
        {
            if (spinCo != null) StopCoroutine(spinCo);
            spinCo = StartCoroutine(SpinRoutine(number));
        }

        private IEnumerator SpinRoutine(int number)
        {
            float startAngle = angleTopCW;
            float startRadius = radiusStart;

            int targetIndex = System.Array.IndexOf(wheelNumbers, number);
            if (targetIndex < 0) targetIndex = 0;

            float targetCenter = targetIndex * sectorSize + sectorSize * 0.5f + zeroOffsetDeg;
            targetCenter = Mathf.Repeat(targetCenter, 360f);

            int revolutions = Random.Range(minRevolutions, maxRevolutions + 1);
            float deltaToTarget = DeltaCW(startAngle, targetCenter);
            float totalDelta = revolutions * 360f + deltaToTarget;

            float t = 0f;
            float dur = Mathf.Max(0.01f, spinDuration);

            while (t < 1f)
            {
                t += Time.deltaTime / dur;
                float e = EaseOutCubic(Mathf.Clamp01(t));
                angleTopCW = Mathf.Repeat(startAngle + totalDelta * e, 360f);
                float r = Mathf.Lerp(startRadius, radiusFinish, e);
                SetBallPosition(angleTopCW, r);
                yield return null;
            }

            angleTopCW = targetCenter;
            SetBallPosition(angleTopCW, radiusFinish);
            
            OnEndSpinAction?.Invoke(number);
            
            spinCo = null;
        }

        private void SetBallPosition(float angleFromTopCW, float r)
        {
            float angleFromXccw = 90f - angleFromTopCW;
            float rad = angleFromXccw * Mathf.Deg2Rad;
            Vector3 center = wheel.position;
            Vector3 pos = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * r;
            transform.position = pos;
        }

        private float DeltaCW(float a, float b)
        {
            float d = Mathf.Repeat(b - a, 360f);
            return d < 0f ? d + 360f : d;
        }

        private float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - x, 3f);
    }
}