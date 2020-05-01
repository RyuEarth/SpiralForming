using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController {

	public LayerMask passengerMask;

	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	public float speed;
	public bool cyclic;
	public float waitTime;
	[Range(0,2)]
	public float easeAmount;
    [System.NonSerialized]
    public Vector3 velocity;

    int fromWaypointIndex;
	float percentBetweenWaypoints;
	float nextMoveTime;
    Player player;

    List<PassengerMovement> passengerMovement;
	Dictionary<Transform,Player> passengerDictionary = new Dictionary<Transform, Player>();
	
	public override void Start () {
		base.Start ();

		globalWaypoints = new Vector3[localWaypoints.Length];
		for (int i =0; i < localWaypoints.Length; i++) {
			globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}

	void Update () {

		UpdateRaycastOrigins ();

		velocity = CalculatePlatformMovement();

		CalculatePassengerMovement(velocity);

		MovePassengers (true);
		transform.Translate (velocity);
		MovePassengers (false);
	}

	float Ease(float x) {
		float a = easeAmount + 1;
		return Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(1-x,a));
	}
	
	Vector3 CalculatePlatformMovement() {

		if (Time.time < nextMoveTime) {
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

		if (percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex ++;

			if (!cyclic) {
				if (fromWaypointIndex >= globalWaypoints.Length-1) {
					fromWaypointIndex = 0;
					System.Array.Reverse(globalWaypoints);
				}
			}
			nextMoveTime = Time.time + waitTime;
		}

		return newPos - transform.position;
	}

	void MovePassengers(bool beforeMovePlatform) {
		foreach (PassengerMovement passenger in passengerMovement) {
			if (!passengerDictionary.ContainsKey(passenger.transform)) {
				passengerDictionary.Add(passenger.transform,passenger.transform.GetComponent<Player>());
			}

			if (passenger.moveBeforePlatform == beforeMovePlatform) {
                if (passenger.isParent)
                {
                    passengerDictionary[passenger.transform].controller.Move(Vector2.zero, passenger.standingOnPlatform, passenger.slidingOnPlatformHit);
                }
                else
                {
                    passengerDictionary[passenger.transform].controller.Move(passenger.velocity, passenger.standingOnPlatform, passenger.slidingOnPlatformHit);
                }
                passengerDictionary[passenger.transform].CalculateVelocityOnPlatform(passenger.velocity/Time.deltaTime);
			}
		}
	}

	void CalculatePassengerMovement(Vector3 velocity) {
		HashSet<Transform> movedPassengers = new HashSet<Transform> ();
		passengerMovement = new List<PassengerMovement> ();

		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

		// Vertically moving platform
		if (velocity.y != 0) {
			float rayLengthY = Mathf.Abs (velocity.y) + skinWidth;
			
			for (int i = 0; i < verticalRayCount; i ++) {
				Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLengthY, passengerMask);

				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);
						float pushX = (directionY == 1)?velocity.x:0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), directionY == 1, 0f, true));
					}
				}
			}
            float rayLengthX = skinWidth*2;
            if (velocity.x == 0)
            {
                for (int i = 0; i < horizontalRayCount; i++)
                {
                    Vector2 rayOriginLeft = raycastOrigins.bottomLeft;
                    rayOriginLeft += Vector2.up * (horizontalRaySpacing * i);
                    Vector2 rayOriginRight = raycastOrigins.bottomRight;
                    rayOriginRight += Vector2.up * (horizontalRaySpacing * i);
                    RaycastHit2D hitLeft = Physics2D.Raycast(rayOriginLeft, Vector2.right * -1, rayLengthX, passengerMask);
                    RaycastHit2D hitRight = Physics2D.Raycast(rayOriginRight, Vector2.right, rayLengthX, passengerMask);
                    if (hitLeft ^ hitRight)
                    {
                        RaycastHit2D hit = (hitLeft.collider != null) ? hitLeft : hitRight;
                        if (hit && hit.distance != 0)
                        {
                            if (!movedPassengers.Contains(hit.transform))
                            {
                                Debug.Log(hit.normal);
                                movedPassengers.Add(hit.transform);
                                float pushX = (directionY == 1) ? velocity.x : 0;
                                float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                                float slidingOnPlatformHit = Mathf.Sign(hit.normal.x);
                                passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, slidingOnPlatformHit, true));
                            }
                        }
                    }

                }
            }
        }

		// Horizontally moving platform
		if (velocity.x != 0) {
            
            float rayLengthX = Mathf.Abs (velocity.x) + skinWidth;
			
			for (int i = 0; i < horizontalRayCount; i ++) {
                Vector2 rayOriginLeft = raycastOrigins.bottomLeft;
                rayOriginLeft += Vector2.up * (horizontalRaySpacing * i);
                Vector2 rayOriginRight = raycastOrigins.bottomRight;
                rayOriginRight += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hitLeft = Physics2D.Raycast(rayOriginLeft, Vector2.right * -1, rayLengthX, passengerMask);
                RaycastHit2D hitRight = Physics2D.Raycast(rayOriginRight, Vector2.right, rayLengthX, passengerMask);
                bool isParent;
                if (hitLeft ^ hitRight)
                {
                    RaycastHit2D hit = (hitLeft.collider != null) ? hitLeft : hitRight;
                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            float hitNormal=Mathf.Sign(hit.normal.x);
                            movedPassengers.Add(hit.transform);
                            float pushX,pushY;
                            player=hit.transform.GetComponent<Player>();
                            
                            if (player.onGrabbingInput)
                            {
                                hit.transform.parent = transform;
                                isParent = true;
                            }
                            else
                            {
                                hit.transform.parent = null;
                                isParent = false;
                            }
                            pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                            pushY = velocity.y;
                            float slidingOnPlatformHit = Mathf.Sign(hit.normal.x);
                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, slidingOnPlatformHit, true,isParent));
                        }
                    }
                }
                else
                {
                    if(player != null)
                    {
                        if (!player.onGrabbingInput || player.controller.collisions.nextOnGround)
                        {
                            transform.DetachChildren();
                            isParent = false;
                        }
                    }
                }

            }
        }
        else
        {
            float rayLengthX = skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOriginLeft = raycastOrigins.bottomLeft;
                rayOriginLeft += Vector2.up * (horizontalRaySpacing * i);
                Vector2 rayOriginRight = raycastOrigins.bottomRight;
                rayOriginRight += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hitLeft = Physics2D.Raycast(rayOriginLeft, Vector2.right * -1, rayLengthX, passengerMask);
                RaycastHit2D hitRight = Physics2D.Raycast(rayOriginRight, Vector2.right, rayLengthX, passengerMask);
                if (hitLeft ^ hitRight)
                {
                    RaycastHit2D hit = (hitLeft.collider != null) ? hitLeft : hitRight;
                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX, pushY;
                            player = hit.transform.GetComponent<Player>();
                            float slidingOnPlatformHit = 0f;
                            if (player.onGrabbingInput &&!player.controller.collisions.left&&!player.controller.collisions.right)
                            {
                                Debug.Log("right:" + player.controller.collisions.left);
                                slidingOnPlatformHit = Mathf.Sign(hit.normal.x);
                            }
                            pushX = skinWidth * slidingOnPlatformHit;
                            pushY = velocity.y;
                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, slidingOnPlatformHit, true));
                        }
                    }
                }
            }
        }
        // Passenger on top of a horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0) {
			float rayLength = skinWidth * 2;
			
			for (int i = 0; i < verticalRayCount; i ++) {
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
				
				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains(hit.transform)) {
						movedPassengers.Add(hit.transform);
						float pushX = velocity.x;
						float pushY = velocity.y;
						
						passengerMovement.Add(new PassengerMovement(hit.transform,new Vector3(pushX,pushY), true, 0f, false));
					}
				}
			}
		}
	}

	struct PassengerMovement {
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;
        public float slidingOnPlatformHit;
        public bool isParent;
		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform,float _slidingOnPlatfomHit, bool _moveBeforePlatform ,bool _isParent=false) {
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
            slidingOnPlatformHit = _slidingOnPlatfomHit;
            isParent = _isParent;
		}
    }

	void OnDrawGizmos() {
		if (localWaypoints != null) {
			Gizmos.color = Color.red;
			float size = .3f;

			for (int i =0; i < localWaypoints.Length; i ++) {
				Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] : localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
	
}
