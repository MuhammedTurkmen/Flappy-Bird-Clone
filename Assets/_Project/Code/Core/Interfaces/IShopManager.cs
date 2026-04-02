using MET.UI;

namespace MET.Core.Types
{
    public interface IShopManager
    {
        bool IsCharacterSelectionValid { get; }

        public BirdCostumeSAO GetCostumeSet();
    }
}
