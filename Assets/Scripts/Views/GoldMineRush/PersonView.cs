using System.Collections;
using Types;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Views.GoldMineRush
{
    public class PersonView : MonoBehaviour
    {
        [SerializeField] private GameObject _preview;

        [SerializeField] private Transform _miner;
        [SerializeField] private MultiplierView _multiplier;

        private float _currentX = 0f;
        private Coroutine _loop;

        public float CurrenX => _currentX;
        
        public void SetState(GoldMineCrashState state)
        {
            switch (state)
            {
                case GoldMineCrashState.StartState:
                    SetDefaultState();
                    break;
                case GoldMineCrashState.SpinningState:
                    SetSpinningState();
                    break;
                case GoldMineCrashState.NoMoneyState:
                    SetDefaultState();
                    break;
            }
        }

        private void SetDefaultState()
        {
            _preview.SetActive(true);
            _multiplier.UpdateValue(0f);
            
            StopRun();
            
            _miner.gameObject.SetActive(false);
        }

        private void SetSpinningState()
        {
            _preview.SetActive(false);
            _miner.gameObject.SetActive(true);
            
            StartRun();
        }

        private void StartRun()
        {
            if (_loop != null) StopCoroutine(_loop);
            
            _currentX = 0;
            _loop = StartCoroutine(Loop());
        }

        private void StopRun()
        {
            if (_loop != null) StopCoroutine(_loop);
            _loop = null;
            
            _multiplier?.UpdateValue(_currentX);
        }

        private IEnumerator Loop()
        {
            while (true)
            {
                float dt = BeatInterval(_currentX);
                yield return new WaitForSeconds(dt);
                Hit();
                _currentX += Random.value < 0.8f ? 0.1f : 0.2f;
                
                _multiplier.SetValue(_currentX);
            }
        }

        void Hit()
        {
            float k = Intensity(_currentX);
            StartCoroutine(MinerPunch(_miner, k));
        }

        private IEnumerator MinerPunch(Transform t, float k)
        {
            Vector3 s0 = t.localScale;
            Quaternion r0 = t.localRotation;
            float a = 6f * k;
            float dur = 0.12f;

            float t1 = 0f;
            while (t1 < dur)
            {
                t1 += Time.deltaTime;
                float e = t1 / dur;
                t.localScale = new Vector3(1.02f + 0.06f * k * (1 - e), 0.98f - 0.04f * k * (1 - e), 1);
                t.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(a, -a * 0.5f, e));
                yield return null;
            }

            float t2 = 0f;
            while (t2 < dur * 0.8f)
            {
                t2 += Time.deltaTime;
                float e = t2 / (dur * 0.8f);
                t.localScale = Vector3.Lerp(t.localScale, s0, e);
                t.localRotation = Quaternion.Lerp(t.localRotation, r0, e);
                yield return null;
            }

            t.localScale = s0;
            t.localRotation = r0;
        }

        float BeatInterval(float x) => Mathf.Clamp(1f / Mathf.Sqrt(Mathf.Max(1f, x)), 0.18f, 0.8f);
        float Intensity(float x) => Mathf.Clamp01(Mathf.Log10(Mathf.Max(1f, x)) / 1.2f);
    }
}