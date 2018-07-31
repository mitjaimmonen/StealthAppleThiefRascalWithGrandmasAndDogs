using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMover {

    void Init(float moveSpeed, float turnSpeed);
    void Move(float amount);
    void Turn(float amount);
    void UpdateSpeed(float moveSpeed);
    void UpdateSpeed(float moveSpeed, float turnSpeed);

    void Move(Vector3 direction);
    void Turn(Vector3 target);
}
