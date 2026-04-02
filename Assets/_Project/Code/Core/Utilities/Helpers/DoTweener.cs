using UnityEngine;
using DG.Tweening;
using System;

namespace MET.Core
{
    public static class DoTweener
    {
        /// <summary>
        /// Önce hedef scale'ýn biraz üstüne çýkar, sonra hedef scale'a geri döner.
        /// Ýþlem bitince onComplete action'ýný çaðýrýr.
        /// </summary>
        public static void ScaleUp(this Transform target, float targetScale, float punchAmount = 0.1f, float duration = 0.3f, Action onComplete = null)
        {
            target.DOKill();

            Sequence seq = DOTween.Sequence();
            seq.Append(target.DOScale(targetScale + punchAmount, duration * 0.5f).SetEase(Ease.OutQuad));
            seq.Append(target.DOScale(targetScale, duration * 0.5f).SetEase(Ease.InQuad));

            seq.OnComplete(() => onComplete?.Invoke());
        }
    }
}
