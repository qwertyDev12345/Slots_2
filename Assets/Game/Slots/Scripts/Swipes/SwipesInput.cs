using System;
using UnityEngine;

namespace Slots.Swipes
{
    public class SwipesInput : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            GameObject gameObject = new GameObject("[SwipesInput]");
            gameObject.AddComponent<SwipesInput>();
            DontDestroyOnLoad(gameObject);
        }

        public static event Action<SwipeDirection> OnSwiped = null;

        public Vector2 _lastPointPosition =  Vector2.zero;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                _lastPointPosition = Input.mousePosition;

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 currentPosition = Input.mousePosition;

                float horizontalLength = Mathf.Abs(currentPosition.x - _lastPointPosition.x);
                float verticalLength = Mathf.Abs(currentPosition.y - _lastPointPosition.y);

                if (horizontalLength < 100 && verticalLength < 100)
                    return;

                if (horizontalLength > verticalLength)
                {
                    float move = currentPosition.x - _lastPointPosition.x;

                    if (move > 0)
                        CallOnSwipe(SwipeDirection.Right);
                    else
                        CallOnSwipe(SwipeDirection.Left);
                }
                else
                {
                    float move = currentPosition.y - _lastPointPosition.y;

                    if (move > 0)
                        CallOnSwipe(SwipeDirection.Up);
                    else
                        CallOnSwipe(SwipeDirection.Down);
                }
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.W))
                CallOnSwipe(SwipeDirection.Up);

            else if (Input.GetKeyDown(KeyCode.A))
                CallOnSwipe(SwipeDirection.Left);

            else if (Input.GetKeyDown(KeyCode.D))
                CallOnSwipe(SwipeDirection.Right);

            else if (Input.GetKeyDown(KeyCode.S))
                CallOnSwipe(SwipeDirection.Down);
#endif
        }

        private void CallOnSwipe(SwipeDirection direction) =>
            OnSwiped?.Invoke(direction);
    }
}