using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AnimationController))]
public class Prompt : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private TextMeshProUGUI _checkText;
    [SerializeField] private RawImage _approvalIndicator;
    private PromptData _promptData;

    private AnimationController _animationController;
    [SerializeField] private AnimationClip _expandAnimation;
    [SerializeField] private AnimationClip _shrinkAnimation;

    private CancellationTokenSource _expandCancellationTokenSource = null;
    private CancellationTokenSource _shrinkCancellationTokenSource = null;

    private void Awake()
    {
        _animationController = GetComponent<AnimationController>();
    }

    public void SetData(PromptData promptData)
    {
        _checkText.text = string.Empty;
        _promptData = promptData;
        SetPrompt(promptData.PromptHeader);
        SetApproval(promptData.IndicatorType);
    }

    public int GetSeverity()
    {
        if (_promptData == null) return 0;
        return _promptData.GetSeverity();
    }

    private async void SetPrompt(string promptText)
    {
        await _promptText.WriteText(promptText, 0.05f);
    }

    private void SetApproval(IndicatorTypes indicatorType)
    {
        Texture2D texture = indicatorType.GetIndicator();
        if (texture != null)
        {
            _approvalIndicator.texture = texture;
        }

        if (indicatorType == IndicatorTypes.Check)
        {
            _promptData.SetRandomCheck();
        }
    }

    private async void Expand()
    {
        _animationController.PlayAnimationClip(out float time, _expandAnimation);

        _shrinkCancellationTokenSource?.Cancel();
        _expandCancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _expandCancellationTokenSource.Token;

        try {
            await Task.Delay(Mathf.RoundToInt(time * 1000), token);
            PromptData.PromptCheck check = _promptData.GetCurrentCheck();
            await _checkText.WriteText(check.CheckText, 0.005f, token);
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled. No action needed.
        }
        finally
        {
            _expandCancellationTokenSource.Dispose();
            _expandCancellationTokenSource = null;
        }
    }

    private async void Shrink()
    {
        _expandCancellationTokenSource?.Cancel();
        _shrinkCancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _shrinkCancellationTokenSource.Token;

        try
        {
            await _checkText.ClearText(0.0025f, token);
            _animationController.PlayAnimationClip(out float time, _shrinkAnimation);
            await Task.Delay(Mathf.RoundToInt(time * 1000), token);
        }
        catch (OperationCanceledException)
        {
            // Operation was cancelled. No action needed.
        }
        finally
        {
            _shrinkCancellationTokenSource.Dispose();
            _shrinkCancellationTokenSource = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _promptData.Expandable)
        {
            Expand();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _promptData.Expandable)
        {
            Shrink();
        }
    }
}
