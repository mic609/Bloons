using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class BloonEffects : MonoBehaviour
{
    [Header("Glue variables")]
    private float _glueLastingEffect; // how long bloon is being affected by glue
    private float _glueLastingEffectTimer = 0.0f;
    private float _movementSpeedDecrease;
    private int _layersThrough; // how much layers of bloon glue affects
    private float _poppingSpeed; // how fast is bloon being popped by glue

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

    public void SetGlueEffect(float movementSpeedDecrease, float glueLastingEffect, int layersThrough, float poppingSpeed)
    {
        if (!_hasGlueEffect)
        {
            _layersThrough = layersThrough;
            _glueLastingEffect = glueLastingEffect;
            _movementSpeedDecrease = movementSpeedDecrease;
            _poppingSpeed = poppingSpeed;

            gameObject.GetComponent<EnemyMovement>().SetMovementSpeed(movementSpeedDecrease, true);
            _standardSprite = gameObject.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<SpriteRenderer>().sprite = _glueBloonSprite;
        }
        _hasGlueEffect = true;

        if(_poppingSpeed > 0)
        {
            var isCeramicBloon = gameObject.GetComponent<BloonController>().IsCeramicBloon();

            // Destroying bloons with shield
            if (isCeramicBloon)
            {
                // Break the bloon shield
                StartCoroutine(PopShieldedBloonsWithGlue());

                var hitsLeft = gameObject.GetComponent<BloonController>().LayerDestroyed();

                if (hitsLeft == 0)
                    Invoke("PopBloonsWithGlue", _poppingSpeed);
            }
            else
                Invoke("PopBloonsWithGlue", _poppingSpeed);
        }
    }

    private void PopBloonsWithGlue()
    {
        Destroy(gameObject);
    }

    private IEnumerator PopShieldedBloonsWithGlue()
    {
        for (int i = 0; i < _layersThrough; i++)
        {
            yield return new WaitForSeconds(_poppingSpeed);
            gameObject.GetComponent<BloonController>().DestroyLayeredEnemy(null, 1);
        }
    }

    public void RemoveGlueEffect()
    {
        gameObject.GetComponent<EnemyMovement>().SetMovementSpeed(0f, false);

        if(!gameObject.GetComponent<BloonController>().IsCeramicBloon())
            gameObject.GetComponent<SpriteRenderer>().sprite = _standardSprite;
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<BloonController>().ReturnUnGluedCeramicSprite();
        }

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

    public float GetPoppingSpeed()
    {
        return _poppingSpeed;
    }
}
