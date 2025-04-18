using System;
using Coffee.UISoftMaskInternal;
using UnityEngine;

namespace Coffee.UISoftMask
{
    /// <summary>
    /// triggers an event when the view projection matrix of a Canvas in World Space render mode changes.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [ExecuteAlways]
    [AddComponentMenu("")]
    [Icon("Packages/com.coffee.softmask-for-ugui/Icons/SoftMaskIcon.png")]
    public class CanvasViewChangeTrigger : MonoBehaviour
    {
        private Canvas _canvas;
        private Action _checkViewProjectionMatrix;
        private int _lastCameraVpHash;
        private int _lastResHash;

        /// <summary>
        /// Called when the component is enabled.
        /// </summary>
        private void OnEnable()
        {
            hideFlags = UISoftMaskProjectSettings.hideFlagsForTemp;
            TryGetComponent(out _canvas);
            UIExtraCallbacks.onBeforeCanvasRebuild +=
                _checkViewProjectionMatrix ?? (_checkViewProjectionMatrix = CheckViewProjectionMatrix);
        }

        /// <summary>
        /// Called when the component is disabled.
        /// </summary>
        private void OnDisable()
        {
            UIExtraCallbacks.onBeforeCanvasRebuild -=
                _checkViewProjectionMatrix ?? (_checkViewProjectionMatrix = CheckViewProjectionMatrix);
        }

        /// <summary>
        /// Called when the component is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            _canvas = null;
            onCanvasViewChanged = null;
            _checkViewProjectionMatrix = null;
        }

        /// <summary>
        /// Event that is triggered when the view projection matrix changes.
        /// </summary>
        public event Action onCanvasViewChanged;

        private void CheckViewProjectionMatrix()
        {
            if (!_canvas) return;

            // Get the view and projection matrix of the Canvas.
            var prevHash = _lastCameraVpHash;
            _canvas.GetViewProjectionMatrix(out var vpMatrix);
            _lastCameraVpHash = vpMatrix.GetHashCode();

            var prevResHash = _lastResHash;
            var r = Screen.currentResolution;
            _lastResHash = new Vector2Int(r.width, r.height).GetHashCode();

            // The matrix has changed.
            if (prevHash != _lastCameraVpHash || prevResHash != _lastResHash)
            {
                Logging.Log(this, "ViewProjection changed.");
                onCanvasViewChanged?.Invoke();
            }
        }

        /// <summary>
        /// get or add a CanvasViewChangeTrigger component in the root Canvas.
        /// </summary>
        public static CanvasViewChangeTrigger Find(Transform transform)
        {
            // Find the root Canvas component.
            var rootCanvas = transform.GetRootComponent<Canvas>();

            // Get the CanvasViewChangeTrigger component if found, or add.
            return rootCanvas ? rootCanvas.GetOrAddComponent<CanvasViewChangeTrigger>() : null;
        }
    }
}
