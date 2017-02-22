using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerRaycastManager))]
public class PlayerController : MonoBehaviour 
{
	public float moveSpeed; //Use acceleration graphs for this someday.

	public float gravity;

	//Jumping
	public AnimationCurve jumpGraph;
	public float jumpHeight;
	public float jumpTime;
	private float jumpTimer;

	Vector2 velocity;
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
		Debug.Log ("VEL: " + velocity);
		//Debug.Log ("UpdateVelocity() :: Velocity :: " + velocity);
		raycastManager.PerformUpdate (velocity);
		//Debug.Log ("RaycastManager :: PerformUpdate(" + velocity + ")");
		CheckCollisions ();
		//Debug.Log ("CheckCollsions() :: Velocity :: " + velocity);
		UpdateStates ();
		//Debug.Log ("UpdateStates()\n\n");

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

		//Jumping
		if (Input.GetKeyDown (KeyCode.Space) && !playerInfo.isJumping) 
		{
			//StartCoroutine (Jump ((velY) => { this.velocity.y = velY; })); //Used a closure here to allow velocity.y to be edited from the coroutine.
			//StartCoroutine(Jump());
			playerInfo.isJumping = true;
			Debug.Log ("JUMP");
		}
		if (playerInfo.isJumping) 
		{
			Jump ();
		} 
		else
		{
			jumpTimer = 0.0f;
		}
		//Apply gravity... Can do this in another method if needed.
		velocity.y -= gravity;
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
			velocity.y = -nearestHits.y;

			//Best to put this elsewhere...
			if (playerInfo.isJumping && playerInfo.isJumping_Prev)
				playerInfo.isJumping = false;
			if (playerInfo.isFalling)
				playerInfo.isFalling = false;

			Debug.Log ("Bottom Collision!");
		}
		if (raycastManager.collisionInfo.leftCollision && velocity.x < 0) 
		{
			velocity.x = -nearestHits.x;
		}
		if (raycastManager.collisionInfo.rightColision && velocity.x > 0) 
		{
			velocity.x = nearestHits.x;
		}
		if (raycastManager.collisionInfo.topCollision && velocity.y > 0) 
		{
			velocity.y = nearestHits.y;
			Debug.Log ("Top Collision!");
		}

		//Apply velocity to the gameObject...
		//transform.position += (Vector3)velocity;
	}

	private void Jump()
	{
		if (jumpTimer < jumpTime) 
		{
			velocity.y += jumpGraph.Evaluate (jumpTimer / jumpTime) / jumpTime;

			jumpTimer += Time.deltaTime;
		}
	}

	/*
	private IEnumerator Jump()//System.Action<float> velY)
	{
		jumpTimer = 0.0f;
		playerInfo.isJumping = true;

		while (jumpTimer < jumpTime)
		{
			//velY(jumpGraph.Evaluate (jumpTimer / jumpTime));
			velocity.y = jumpGraph.Evaluate(jumpTimer / jumpTime);
			Debug.Log ("Eval: " + jumpGraph.Evaluate(jumpTimer / jumpTime));
			//Debug.Log ("NewVel: " + velocity);
			jumpTimer += Time.deltaTime;
			yield return null;
		}
		playerInfo.isJumping = false;
	}
	*/
}
