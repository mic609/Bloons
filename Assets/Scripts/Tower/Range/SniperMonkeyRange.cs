using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Enemy in range")]
    [SerializeField] private LayerMask _layerMask;

    private List<Transform> _enemies;
}
