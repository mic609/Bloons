// This script is neccesary for towers that can attack bloons without need to shoot to take damage. The bloons are neccesary for remembering what tower attacked them then

using UnityEngine;

public class BloonTowerReference : MonoBehaviour
{
    private ManageTower _towerThatAttacks; // this is mostly used for glue gunners who still pop bloons because of the glue

    public void SetTowerThatAttacks(ManageTower tower)
    {
        _towerThatAttacks = tower;
    }

    public void PopUpForTheTower(int popAmount)
    {
        _towerThatAttacks.GetComponent<ManageTower>().BloonsPoppedUp(popAmount);
    }

    public ManageTower GetTower()
    {
        return _towerThatAttacks;
    }
}
