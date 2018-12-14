using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private const float MoveForce = 8f;
    private const float RunMultiplier = 1.5f;
    private const float JumpForce = 4f;

    private bool _onGround;
    
    private enum MovementState {
        IDLE,
        WALKING,
        RUNNING,
        JUMPING,
        DASHING,
        ATTACKING,
        ROLLING
    }

    private MovementState _movementState = MovementState.IDLE;

    private Rigidbody2D _rigidbody2D;

    private void Awake() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update() {
        var moveCalc = new Vector2();
        
        // Walk left
        if (Input.GetKey(KeyCode.A)) {
            _movementState = MovementState.WALKING;
            
            moveCalc = Vector2.left * MoveForce;
        }
        // Walk right
        else if (Input.GetKey(KeyCode.D)) {
            _movementState = MovementState.WALKING;
            
            moveCalc = Vector2.right * MoveForce;
        }

        // Run, if you're already walking that is
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (_movementState == MovementState.WALKING) {
                _movementState = MovementState.RUNNING;

                moveCalc *= RunMultiplier;
            }
        }

        // Jump
        if (Input.GetKey(KeyCode.Space)) {
            _movementState = MovementState.JUMPING;

            // moveCalc += Vector2.up * JumpForce;
            if (_onGround) {
                _rigidbody2D.velocity = Vector2.up * JumpForce;
            }
        }

        // Stop running, geez
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
            _movementState = MovementState.IDLE;
        }
        
        _rigidbody2D.AddForce(moveCalc);

        print(_movementState);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground")) {
            // print(other);

            if (_movementState == MovementState.JUMPING || _movementState == MovementState.ROLLING) {
                _movementState = MovementState.IDLE;
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