using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[HideMonoScript]
public class ColorBlink : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Color _blinkColor = new Color(1, 1, 1, 0);
    [SerializeField]
    private float _delay = 0.25f;
    [SerializeField]
    private bool _activeOnEnable = false;

    private CancellationTokenSource _cancellationTokenSource;

    public void StartBlink()
    {
        if (_cancellationTokenSource == null)
        {
            CTSUtility.Reset(ref _cancellationTokenSource);
            BlinkAsync(_cancellationTokenSource.Token).Forget();
        }

        async UniTaskVoid BlinkAsync(CancellationToken cancellationToken)
        {
            Color originalColor = _image.color;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _image.color = _blinkColor;

                    await UniTask.WaitForSeconds(_delay, cancellationToken: cancellationToken);

                    _image.color = originalColor;

                    await UniTask.WaitForSeconds(_delay, cancellationToken: cancellationToken);
                }
            }
            catch 
            {
                if (!_image)
                    return;
            }

            _image.color = originalColor;
        }
    }

    public void StopBlink()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);
    }

    private void OnEnable()
    {
        if (_activeOnEnable)
            StartBlink();
    }

    private void OnDisable()
    {
        CTSUtility.Clear(ref _cancellationTokenSource);   
    }
}
