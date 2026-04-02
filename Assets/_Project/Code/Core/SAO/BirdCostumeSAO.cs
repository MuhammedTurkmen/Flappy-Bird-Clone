using UnityEngine;

namespace MET.UI
{
    [CreateAssetMenu(fileName = "New Costume", menuName = "Bird/Costume")]
    public class BirdCostumeSAO : ScriptableObject
    {
        public int ID; 
        public int Point;
        public Sprite[] Sprites;
    }
}