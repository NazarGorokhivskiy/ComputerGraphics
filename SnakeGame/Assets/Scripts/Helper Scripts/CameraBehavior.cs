using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {
    public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    private float CameraOffset = -1f;

    void Update() {
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, targeC:\Users\Nazar\Desktop\LPNU\5\Computer Graphics\SnakeGame\Assets\Scripts\Player Scripts\PlayerController.cst.position.y + CameraOffset, transform.position.z), 0.1f);;
    }
}