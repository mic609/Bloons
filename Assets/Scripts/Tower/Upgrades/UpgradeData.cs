using UnityEngine;

[CreateAssetMenu(menuName = "New Upgrade")]
public class UpgradeData : ScriptableObject
{
    public int upgradeNumber;
    public int upgradeCost;

    [Header("Standard")]
    [TextArea] public string upgradeDescription;
    public float delay; // speed attack
    public float radius;

    [Header("Sprite")]
    public Sprite upgradeSprite;

    [Header("Splash Damage")]
    public float projectileArea;
    public int numberOfBloonsToPop;

    [Header("Pop through")]
    public int damage;

    [Header("Glue effects")]
    public float movementSpeedDecrease;
    public float glueLastingEffect;
    public int layersAffected;
    public float poppingSpeed;

    [Header("Monkey Ace")]
    public int bulletsAmount;

    [Header("Dartling Gunner")]
    public Vector3 borderVectorA;
    public Vector3 borderVectorB;
    public float rotationSpeed;

    [Header("Spike Factory")]
    public float timeAlive;
}
