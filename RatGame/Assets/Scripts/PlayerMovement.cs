using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour {
    private const float MoveForce = 8f;
    private const float RunMultiplier = 1.5f;
    private const float JumpForce = 4f;
    private const float RollJumpForce = 2f;
    private const float RollMultiplier = 2f;
    private const int RollAmount = 2;

    private bool _onGround;
    private int _facingMultiplier = 1;

    private enum MovementState {
        Idle,
        Walking,
        Running,
        Jumping,
        Dashing,
        Attacking,
        Rolling
    }

    private MovementState _movementState = MovementState.Idle;

    private Rigidbody2D _rigidbody2D;
    private Transform _transform;

    private void Awake() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
    }

    void Update() {
        var moveCalc = new Vector2();

        // Walk left
        if (Input.GetKey(KeyCode.A)) {
            _movementState = MovementState.Walking;

            _facingMultiplier = -1;
        }
        // Walk right
        else if (Input.GetKey(KeyCode.D)) {
            _movementState = MovementState.Walking;

            _facingMultiplier = 1;
        }

        if (_movementState == MovementState.Walking) {
            moveCalc = new Vector2(_facingMultiplier, 0) * MoveForce;
        }

        // Run, if you're already walking that is
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (_movementState == MovementState.Walking) {
                _movementState = MovementState.Running;

                moveCalc *= RunMultiplier;
            }
        }

        // Stop running, geez
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
            _movementState = MovementState.Idle;
        }

        // Don't jump! Err, or in this case, do!
        if (Input.GetKey(KeyCode.W)) {
            _movementState = MovementState.Jumping;

            // moveCalc += Vector2.up * JumpForce;
            if (_onGround) {
                _rigidbody2D.velocity = Vector2.up * JumpForce;
            }
        }

        // Roll around but not quite at the speed of sound
        if (Input.GetKey(KeyCode.Space)) {
            _movementState = MovementState.Rolling;

            // Roll along the ground, like Dark Souls
            if (_onGround) {
                _rigidbody2D.velocity = Vector2.up * RollJumpForce;
                moveCalc *= RollMultiplier;
            }
            // Roll in the air
            else {
                moveCalc *= RollMultiplier;
            }
        }

        if (_movementState == MovementState.Rolling) {
            // TODO: Rotate the player whilst they roll
        }

        _rigidbody2D.AddForce(moveCalc);

        print(_movementState);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground")) {
            // print(other);

            switch (_movementState) {
                case MovementState.Rolling:
                    _rigidbody2D.AddForce(new Vector2(MoveForce * 10 * RollMultiplier * _facingMultiplier, 0f));
                    goto case MovementState.Jumping;

                case MovementState.Jumping:
                    _movementState = MovementState.Idle;
                    break;
            }

            _onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground")) {
            _onGround = false;
        }
    }
}