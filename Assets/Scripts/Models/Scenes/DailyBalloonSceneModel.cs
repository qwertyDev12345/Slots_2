using UnityEngine;

namespace Models.Scenes
{
    public class DailyBalloonSceneModel
    {
        private float _targetBalloonSize;

        public float GetTargetBalloonSize()
        {
            _targetBalloonSize = Random.Range(0.4f, 1.0f);

            return _targetBalloonSize;
        }

        public int GetPrize()
        {
            return (int)(_targetBalloonSize * 1000);
        }

        public float GetBalloonSize(float sec)
        {
            return _targetBalloonSize - sec;
        }
    }
}