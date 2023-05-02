using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    GameManager _gameManager;

    [SerializeField]
    AudioClip _goodSound;

    int _missIgnoreTime = 7;
    int _scoreModifierTime = 10;
    int _scoreModifier = 2;
    int _timeBonus = 7;

    float _horizontalSpawnBound = 5.0f;
    float _spawnPointY = 6.5f;
    float _spawnPointZ = -1.0f;

    public enum PowerupType { scoreBooster, missIgnore, timeAdder }

    void Start()
    {
        _gameManager =  GameObject.Find("GameManager").GetComponent<GameManager>();
        transform.position = GenerateRandomPosition();
    }

    private Vector3 GenerateRandomPosition()
    {
        return new Vector3(UnityEngine.Random.Range(-_horizontalSpawnBound, _horizontalSpawnBound), _spawnPointY, _spawnPointZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CollisionDetect") && GameManager.isGameActive)
        {
            Camera.main.GetComponent<AudioSource>().PlayOneShot(_goodSound);
            if (CompareTag("ScoreBooster"))
            {
                _gameManager.StartCoroutine(_gameManager.SetScoreModifier(_scoreModifier, _scoreModifierTime));
            }
            else if (CompareTag("MissIgnore"))
            {
                _gameManager.StartCoroutine(_gameManager.SetMissIgnore(_missIgnoreTime));
            }
            else if (CompareTag("TimeAdder"))
            {
                _gameManager.StartCoroutine(_gameManager.SetTimeAdder(_timeBonus, 2));
            }
        }
        Destroy(gameObject);
    }
}
