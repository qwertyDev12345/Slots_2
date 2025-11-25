using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Slots.Data.Styles;

namespace Slots.Game.Machine
{
    public class LineDrawer : MonoBehaviour
    {
        private const float DRAWING_SPRITE_TIME = 0.5f;

        [SerializeField] private GameObject _linePrefab = null;

        private List<Color> _lineColors = new List<Color>();

        private List<LineRenderer> _lineRenderers = new List<LineRenderer>();
        private List<Coroutine> _drawCoroutines = new List<Coroutine>();

        private StyleSet.LineRendererSet _styleSet;

        private void Awake()
        {
            FieldGenerator fieldGenerator = GetComponentInParent<FieldGenerator>();

            _styleSet = fieldGenerator.GetStyleSet(StyleSetGroup.Line).GetLineRendererSet();

            _lineColors = _styleSet.colors.ToList();
        }

        public void DrawLine(Line line)
        {
            Vector3 offset = (-transform.forward * (0.1f + (0.01f * _drawCoroutines.Count)));

            List<Vector3> path = new List<Vector3>();

            foreach (var slot in line.Slots)
                path.Add(slot.transform.position + offset);

            Vector3 leftPoint = line.Slots[0].transform.TransformPoint(new Vector3(-25f, 0f, 0f));
            
            path.Insert(0, leftPoint + offset);

            Vector3 rightPoint = line.Slots.Last().transform.TransformPoint(new Vector3(25f, 0f, 0f));

            path.Insert(path.Count, rightPoint + offset);

            _drawCoroutines.Add(StartCoroutine(DrawLineCoroutine(path)));
        }

        public void ClearLines()
        {
            foreach (LineRenderer lineRenderer in _lineRenderers)
            {
                lineRenderer.material.DOFade(0f, 0.2f).OnComplete(() =>
                    Destroy(lineRenderer.gameObject)).Play();
            }

            foreach (Coroutine drawCoroutine in _drawCoroutines)
                StopCoroutine(drawCoroutine);

            _lineRenderers = new List<LineRenderer>();
            _drawCoroutines = new List<Coroutine>();
        }

        public Color GetRandomBrightColor()
        {
            Color color = _lineColors[0];

            _lineColors.RemoveAt(0);
            _lineColors.Insert(_lineColors.Count, color);

            return color;
        }

        private IEnumerator DrawLineCoroutine(List<Vector3> path)
        {
            GameObject lineObject = Instantiate(_linePrefab, transform);

            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();

            Color color = GetRandomBrightColor();

            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            lineRenderer.material.SetTexture("_MainTex", _styleSet.sprite.texture);

            _lineRenderers.Add(lineRenderer);

            lineRenderer.positionCount = 0;

            float currentLerp = 0f;

            while (currentLerp < path.Count)
            {
                currentLerp += Time.deltaTime / DRAWING_SPRITE_TIME * path.Count;

                int currentIndex = (int)currentLerp;

                int readyCount = Mathf.Clamp(currentIndex + 1, 0, path.Count);

                float localProgress = currentLerp - currentIndex;

                lineRenderer.positionCount = readyCount;
                lineRenderer.SetPositions(path.GetRange(0, readyCount).ToArray());

                if (currentIndex < path.Count - 1)
                {
                    Vector3 lerpPosition = Vector3.Lerp(path[currentIndex], path[currentIndex + 1], localProgress);

                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, lerpPosition);
                }

                yield return null;
            }

            lineRenderer.positionCount = path.Count;
            lineRenderer.SetPositions(path.ToArray());
        }
    }
}