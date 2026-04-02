using UnityEngine;
using UnityEngine.UI;

namespace MET.UI
{
    public class CostumeDisplay : CostumeVisualizer
    {
        [SerializeField] private Image _image;

        public override void SetSprite()
        {
            _image.sprite = GetSprite();
        }
    }
}
