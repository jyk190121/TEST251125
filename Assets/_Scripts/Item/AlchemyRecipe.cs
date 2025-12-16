using UnityEngine;

// === 연금술 레시피 ===
[CreateAssetMenu(fileName = "NewAlchemyRecipe", menuName = "Recipe/Alchemy Recipe")]
public class AlchemyRecipe : Recipe
{
    [Header("연금술 특화")]
    [Tooltip("한 번에 만들 포션 개수")]
    public int quantityPerCraft = 1;
}
