// Description of the difficulty

using UnityEngine;

[CreateAssetMenu(menuName = "New Level Difficulty")]
public class LevelDifficultyData : ScriptableObject
{
    public int moneyAtTheBeginning;
    public int numberOfLevels;
    public float upgradeCost; // tower sell price discount
    public int numberOfLifes;
    public float bloonSpeedIncrease; // how faster bloons move in harder mods
    [TextArea] public string difficultyDescription;
}
