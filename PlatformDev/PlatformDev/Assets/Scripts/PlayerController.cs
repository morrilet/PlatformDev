using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerRaycastManager))]
public class PlayerController : MonoBehaviour 
{
	Vector2 velocity;
	public float moveSpeed; //Use acceleration graphs for this someday.
	PlayerRaycastManager raycastManager;

	struct PlayerInfo
	{
		public bool isJumping;
		public bool isJumping_Prev;
		public bool isFalling;
		public bool isFalling_Prev;
		public bool isCrouching;
		public bool isCrouching_Prev;
		public bool isTouchingWall;
		public bool isTouchingWall_Prev;
		public bool isTouchingGround;
		public bool isTouchingGround_Prev;

		public bool justJumped;
		public bool justFell;
		public bool justCrouched;
		public bool justTouchedWall;
		public bool justLanded;

		public void reset()
		{
			isJumping_Prev = isJumping;
			isFalling_Prev = isFalling;
			isCrouching_Prev = isCrouching;
			isTouchingWall_Prev = isTouchingWall;
			isTouchingGround_Prev = isTouchingGround;

			isJumping = false;
			isFalling = false;
			isCrouching = false;
			isTouchingWall = false;
			isTouchingGround = false;
		}
	}
	PlayerInfo playerInfo;

	void Start()
	{
		playerInfo = new PlayerInfo();
		raycastManager = this.GetComponent<PlayerRaycastManager> ();
	}

	void Update()
	{
		//The order of these statements is very important because of the structs in each of the classes being reset.
		//UpdateStates ();
		//Move ();

		UpdateVelocity ();
		Debug.Log ("UpdateVelocity() :: Velocity :: " + velocity);
		raycastManager.PerformUpdate (velocity);
		Debug.Log ("RaycastManager :: PerformUpdate(" + velocity + ")");
		CheckCollisions ();
		Debug.Log ("CheckCollsions() :: Velocity :: " + velocity);
		UpdateStates ();
		Debug.Log ("UpdateStates()\n\n");

		//Apply velocity to the gameObject...
		transform.position += (Vector3)velocity;
	}

	//Reads collision info from the raycast manager and interprets it as player info.
	private void UpdateStates()
	{
		playerInfo.reset ();

		//Basic Info...
		if (raycastManager.collisionInfo.bottomCollision) 
		{
			playerInfo.isTouchingGround = true;
		}
		if (raycastManager.collisionInfo.leftCollision || 
			raycastManager.collisionInfo.rightColision) 
		{
			playerInfo.isTouchingWall = true;
		}

		//Just landed...
		/*if (raycastManager.collisionInfo.bottomCollision &&
			!raycastManager.collisionInfo.bottomCollision_Prev) 
		{
		}*/

		//Just Jumped...

		//Just hit wall...

		//Just crouched...

	}

	//This is where velocity changes go. The result is a final velocity to be applied to the
	//players transform in the move method.
	private void UpdateVelocity()
	{
		//General movement...
		//Input will eventually be handled in an input manager.
		velocity = Vector2.zero;
		velocity.x += moveSpeed * Input.GetAxisRaw ("Horizontal");

		//Apply gravity... Can do this in another method if needed.
		velocity.y -= 0.25f; //Use an actual value for gravity later.
	}

	private void CheckCollisions()
	{
		//UpdateVelocity ();

		//Perform raycasts...
		Vector2 nearestHits = raycastManager.PerformUpdate(velocity);

		//Handle collisions.
		if (raycastManager.collisionInfo.bottomCollision && velocity.y < 0) 
		{
			//velocity.y = transform.position.y - nearestHits.y;
			velocity.y = nearestHits.y;
		}
		if (raycastManager.collisionInfo.leftCollision && velocity.x < 0) 
		{
			velocity.x = nearestHits.x;
		}
		if (raycastManager.collisionInfo.rightColision && velocity.x > 0) 
		{
			velocity.x = transform.position.x - nearestHits.x;
		}
		if (raycastManager.collisionInfo.topCollision && velocity.y > 0) 
		{
			velocity.y = transform.position.y - nearestHits.y;
		}

		//Apply velocity to the gameObject...
		//transform.position += (Vector3)velocity;
	}
}
