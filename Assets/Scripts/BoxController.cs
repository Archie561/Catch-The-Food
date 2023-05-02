using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    Vector3 _worldDepthOffset;

    float _moveBorderX = 4.0f;
    float _moveBorderY = 1.0f;

    void Start()
    {
        _worldDepthOffset = new Vector3(0, 0, Camera.main.transform.position.z - transform.position.z);
    }

    private void OnMouseDrag()
    {
        if (GameManager.isGameActive)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - _worldDepthOffset);

            if (transform.position.x > _moveBorderX)
                transform.position = new Vector3(_moveBorderX, transform.position.y, transform.position.z);
            if (transform.position.x < -_moveBorderX)
                transform.position = new Vector3(-_moveBorderX, transform.position.y, transform.position.z);
            if (transform.position.y > _moveBorderY)
                transform.position = new Vector3(transform.position.x, _moveBorderY, transform.position.z);
            if (transform.position.y < -_moveBorderY)
                transform.position = new Vector3(transform.position.x, -_moveBorderY, transform.position.z);
        }
    }
}
