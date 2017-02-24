using UnityEngine;
using System.Collections;

public class JumpEquationTester : MonoBehaviour 
{
	public float jumpHeight;
	public float jumpTimeToApex;
	public float gravity;

	private float jumpTimer;
	private bool isJumping;
	private Vector2 velocity;

	void Start()
	{
		isJumping = false;
		jumpTimer = 0.0f;
		velocity = Vector2.zero;
		jumpTimeToApex *= 51.0f;
	}

	void FixedUpdate()
	{
		//Update velocity...
		//velocity = Vector2.zero;
		//if (!isJumping)
			//velocity.y += gravity; //Apply gravity
		//else
		velocity.y += -((2.0f * jumpHeight) / jumpTimeToApex) / jumpTimeToApex;

		if (!isJumping) //Jump state stuff
		{
			isJumping = true;
			jumpTimer = 0.0f;

			//Set to initial velocity...
			velocity.y = (2.0f * jumpHeight) / jumpTimeToApex;
		}

		if (isJumping) //Jump velocity stuff
		{
			//float initialVelocity = 0.0f; //This is the initial velocity of the jump.
			//initialVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
			//initialVelocity = (2.0f * jumpHeight) / jumpTimeToApex;
			//float tempGravity = -initialVelocity / jumpTimeToApex;

			jumpTimer += Time.deltaTime;
			//velocity.y += -2.0f * (jumpTimer - jumpTimeToApex);
			//velocity.y += jumpHeight - Mathf.Pow(jumpTimer - jumpTimeToApex, 2.0f);
			//velocity.y += initialVelocity + gravity * (jumpTimer);
			//velocity.y += initialVelocity - (initialVelocity / jumpHeight) * (jumpTimer);

			Debug.Log ("Jumping... P :: " + transform.position.y + " V :: " + velocity.y + " T :: " + jumpTimer);
		}
		//End of velocity updates.

		//Keep the jumper from falling off-screen...
		if (transform.position.y <= 0)
		{
			transform.position = new Vector3 (transform.position.x, 0.0f, transform.position.z);
			if(velocity.y < 0)
			{
				velocity.y = 0.0f;
				isJumping = false; //Reset jumping state when we hit the "ground".
			}
		}

		//Apply velocity...
		transform.position += (Vector3)velocity;// * Time.deltaTime;
	}
}
