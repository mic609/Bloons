using System.Collections;
using UnityEngine;

public class BloonEffects : MonoBehaviour
{
    [Header("Glue variables")]
    private float _glueLastingEffect; // how long bloon is being affected by glue
    private float _glueLastingEffectTimer = 0.0f;
    private float _movementSpeedDecrease;
    private int _layersThrough; // how much layers of bloon glue affects
    [SerializeField] private float _poppingSpeed; // how fast is bloon being popped by glue

    [Header("Sprites")]
    [SerializeField] private Sprite _glueBloonSprite;
    private Sprite _standardSprite;

    private bool _hasGlueEffect = false;

    private void Update()
    {
        if (_hasGlueEffect)
        {
            _glueLastingEffectTimer += Time.deltaTime;
            if (_glueLastingEffectTimer >= _glueLastingEffect)
                RemoveGlueEffect();
        }
    }

    public void SetGlueEffect(float movementSpeedDecrease, float glueLastingEffect, int layersThrough)
    {
        if (!_hasGlueEffect)
        {
            _layersThrough = layersThrough;
            _glueLastingEffect = glueLastingEffect;
            _movementSpeedDecrease = movementSpeedDecrease;

            gameObject.GetComponent<EnemyMovement>().SetMovementSpeed(movementSpeedDecrease, true);
            _standardSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<SpriteRenderer>().sprite = _glueBloonSprite;
        }
        _hasGlueEffect = true;
    }

    public void RemoveGlueEffect()
    {
        gameObject.GetComponent<EnemyMovement>().SetMovementSpeed(0f, false);
        gameObject.GetComponent<SpriteRenderer>().sprite = _standardSprite;
        _hasGlueEffect = false;
        _glueLastingEffectTimer = 0.0f;
    }

    public int GetGlueLayersThrough()
    {
        return _layersThrough;
    }

    public void SetGlueLayersThrough(int layersThrough)
    {
        _layersThrough = layersThrough;
    }

    public float GetGlueLastingEffect()
    {
        return _glueLastingEffect;
    }

    public float GetMovementSpeedDecrease()
    {
        return _movementSpeedDecrease;
    }

    public bool HasGlueEffect()
    {
        return _hasGlueEffect;
    }
}
