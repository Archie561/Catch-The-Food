using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    GameManager _gameManager;
    Rigidbody _foodRigidbody;

    [SerializeField]
    int _catchScoreAmount;
    [SerializeField]
    int _missScoreAmount;
    [SerializeField]
    AudioClip _goodSound;
    [SerializeField]
    AudioClip _badSound;

    int _timeBonus = 3;

    float _horizontalSpawnBound = 5.0f;
    float _spawnPointY = 5.0f;
    float _spawnPointZ = -1.0f;
    float _torqueForce = 20.0f;

    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _foodRigidbody = GetComponent<Rigidbody>();

        transform.position = GenerateRandomPosition();
        _foodRigidbody.AddTorque(Random.Range(-_torqueForce, _torqueForce), Random.Range(-_torqueForce, _torqueForce), Random.Range(-_torqueForce, _torqueForce));
    }

    private Vector3 GenerateRandomPosition()
    {
        return new Vector3(UnityEngine.Random.Range(-_horizontalSpawnBound, _horizontalSpawnBound), _spawnPointY, _spawnPointZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObjectDestroyer") && GameManager.isGameActive)
        {
            _gameManager.UpdateScore(_missScoreAmount);
            if (CompareTag("FreshFood"))
            {
                Camera.main.GetComponent<AudioSource>().PlayOneShot(_badSound);
            }
        }
        else if (other.CompareTag("CollisionDetect") && GameManager.isGameActive)
        {
            _gameManager.UpdateScore(_catchScoreAmount);
            if (CompareTag("FreshFood"))
            {
                _gameManager.UpdateTime(_timeBonus);
                Camera.main.GetComponent<AudioSource>().PlayOneShot(_goodSound);
            }
            else if (CompareTag("NotFood"))
            {
                Camera.main.GetComponent<AudioSource>().PlayOneShot(_badSound);
            }
        }
        Destroy(gameObject);
    }
}
