using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .15f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;
    Vector2 externalVelocity;
    Vector2 jumpVelocity;
	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;
    [NonSerialized]
    public bool onGrabbingInput;
    [NonSerialized]
    public int grabbingTime;

    public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

    [NonSerialized]
	public Controller2D controller;

	Vector2 directionalInput;
	bool wallSliding;
	int wallDirX;
    int jumpInputTime;
    bool isJumping;


	void Start() {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	void Update() {
		CalculateVelocity ();
        HandleWallSliding ();

		controller.Move (velocity * Time.deltaTime, directionalInput);
        
        if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }else {
				velocity.y = 0;
			}
            if ((controller.collisions.below||controller.collisions.slidingOnPlatformHit!=0)&&jumpInputTime>0)
            {
                jumpInputTime = 0;
                if (controller.collisions.nextOnGround)
                {
                    externalVelocity = Vector2.zero;
                    controller.collisions.nextOnGround = false;
                }
            }
		}
        if (controller.collisions.right || controller.collisions.left)
        {
            externalVelocity = Vector2.zero;
        }
        isJumping = false;

    }

	public void SetDirectionalInput (Vector2 input) {
		directionalInput = input;
	}
    public void OnGrabInput()
    {
        onGrabbingInput = true ;
    }
	public void OnJumpInputDown() {
        jumpInputTime++;
		if (wallSliding) {
			if (wallDirX == directionalInput.x) {
				jumpVelocity.x = -wallDirX * wallJumpClimb.x+externalVelocity.x;
				jumpVelocity.y = wallJumpClimb.y+externalVelocity.y;
			}
			else if (directionalInput.x == 0) {
				jumpVelocity.x = -wallDirX * wallJumpOff.x+externalVelocity.x;
				jumpVelocity.y = wallJumpOff.y+externalVelocity.y;
			}
			else {
				jumpVelocity.x = -wallDirX * wallLeap.x+externalVelocity.x;
				jumpVelocity.y = wallLeap.y+externalVelocity.y;
			}
        }

		if (controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { // not jumping against max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			} else {
				velocity.y = maxJumpVelocity+externalVelocity.y;
                velocity.x = velocity.x+externalVelocity.x;
			}
		}
        if(wallSliding || controller.collisions.below)
        {
            isJumping = true;

        }
        else
        {
            isJumping = false;
        }
	}

    public void SetVelocityGrabbingPlatform(Vector2 velocity)
    {
        this.velocity.x = velocity.x;
        this.velocity.y = velocity.y;
    }
	public void OnJumpInputUp() {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}
		
	void HandleWallSliding() {
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;

		if (((controller.collisions.left || controller.collisions.right)||wallSliding) && !controller.collisions.below && velocity.y < 0) {
            if (wallDirX == directionalInput.x||onGrabbingInput)
            {
                wallSliding = true;
                if (onGrabbingInput)
                {
                    velocity.y = 0;
                    grabbingTime++;
                    if (directionalInput.y > 0)
                    {
                        velocity.y = wallSlideSpeedMax * 1.5f;
                    }
                    else if(directionalInput.y<0)
                    {
                        velocity.y = -wallSlideSpeedMax * 1.5f;
                    }
                }
                else
                {
                    if (velocity.y < -wallSlideSpeedMax)
                    {
                        velocity.y = -wallSlideSpeedMax;
                    }
                }

            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;
                wallSliding = true;
                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
        onGrabbingInput = false;
    }
    public void CalculateVelocityOnPlatform(Vector2 velocity)
    {
        if (!controller.collisions.nextOnGround&&(controller.collisions.below||controller.collisions.slidingOnPlatformHit!=0))
        {
            if (velocity.y >= 0)
            {
                externalVelocity = velocity;
            }
            else
            {
                externalVelocity = Vector2.zero;
            }
        }
    }

	void CalculateVelocity() {
        float targetVelocityX= directionalInput.x*moveSpeed;
        float externalVelocityDirection = Mathf.Sign(externalVelocity.x);
        float accelerationTime=0;
        if (wallSliding && !isJumping)
        {
            externalVelocity = Vector2.zero;
        }
        else if (wallSliding && isJumping)
        {
            velocity = externalVelocity;
        }


        if (controller.collisions.below && !isJumping )
        {
            externalVelocity.x = 0;
        }else if(controller.collisions.below && isJumping)
        {
            velocity.x = externalVelocity.x;
        }

        if (!controller.collisions.below && !isJumping && !wallSliding)
        {
            accelerationTime = accelerationTimeAirborne;
            if (externalVelocity.x != 0)
            {
                if (directionalInput.x == externalVelocityDirection * -1)
                {
                    externalVelocity.x = 0;
                }
                velocity.x = externalVelocity.x;
                Debug.Log(externalVelocity.x);
            }
        }
        else
        {
            accelerationTime = accelerationTimeGrounded;
        }

        if (jumpVelocity != Vector2.zero)
        {
            velocity.x += jumpVelocity.x;
            velocity.y += jumpVelocity.y;
            jumpVelocity = Vector2.zero;
        }
        if (onGrabbingInput && wallSliding && directionalInput.y > 0)
        {
            externalVelocity.x = 0;
            velocity.x = externalVelocity.x;
        }
        //Debug.Log("externalVelocity.x:" + externalVelocity.x);
        //Debug.Log("isJumping:" + isJumping);
        velocity.x = Mathf.SmoothDamp (velocity.x, externalVelocity.x+targetVelocityX, ref velocityXSmoothing, accelerationTime);
        velocity.y += gravity * Time.deltaTime;
	}
}
