using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Level Difficulty")]
public class LevelDifficultyData : ScriptableObject
{
    public int moneyAtTheBeginning;
    public int numberOfLevels;
    public float upgradeCost; // how much reduced or increased ar costs of monkeys or upgrades
    public int numberOfLifes;
    public float bloonSpeedIncrease; // how faster bloons move in harder mods
    [TextArea] public string difficultyDescription;
}
