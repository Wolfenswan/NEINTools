using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{
    [SerializeField] [Tooltip("Drag and Drop the desired sprites onto the variable name")] Sprite[] _spriteList;
    SpriteRenderer _renderer;
 
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        Sprite newSprite = PickRandomSprite();
        ChangeSprite(newSprite);
    }

    Sprite PickRandomSprite() {
        int idx = Random.Range(0, _spriteList.Length);
        Sprite newSprite = _spriteList[idx];
        return newSprite;
    }

    void ChangeSprite(Sprite newSprite)
    {
        _renderer.sprite = newSprite;
    }
}