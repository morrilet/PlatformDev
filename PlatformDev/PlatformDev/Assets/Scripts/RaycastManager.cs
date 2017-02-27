using UnityEngine;
using System.Collections;

public class RaycastManager : MonoBehaviour 
{
	struct CollisionInfo
	{
		public bool topCollision, topCollision_Prev;
		public bool bottomCollision, bottomCollision_Prev;
		public bool rightCollision, rightCollision_Prev;
		public bool leftCollision, leftCollision_Prev;

		public void reset()
		{
			topCollision_Prev = topCollision;
			bottomCollision_Prev = bottomCollision;
			rightCollision_Prev = rightCollision;
			leftCollision_Prev = leftCollision;

			topCollision = false;
			bottomCollision = false;
			rightCollision = false;
			leftCollision = false;
		}
	}
	CollisionInfo collisionInfo;

	public float maxClimbAngle;

	public int numHorizontalRaycasts;
	public int numVerticalRaycasts;
	public float skinWidth;

	private float objectWidth, objectHeight; //The width and height of the object we're attached to.

	void Start()
	{
		collisionInfo = new CollisionInfo ();
	}

	void FixedUpdate()
	{
		objectWidth  = this.GetComponent<Renderer> ().bounds.size.x;
		objectHeight = this.GetComponent<Renderer> ().bounds.size.y;

		//These will be called from move, which will be called here with the players intended velocity.
		PerformHorizontalRaycasts (1.0f);
		PerformVerticalRaycasts (1.0f);

		collisionInfo.reset ();
	}

	void Move(out Vector2 velocity)
	{
		velocity = Vector2.zero;
	}

	void PerformHorizontalRaycasts(float distance)
	{
		//Raycast to the left...
		for (int i = 0; i < numHorizontalRaycasts; i++)
		{
			RaycastHit2D hit = new RaycastHit2D();

			//Find the origin of the raycast...
			Vector2 origin = Vector2.zero;
			origin.x = transform.position.x + ((-objectWidth / 2.0f) + skinWidth);
			origin.y = transform.position.y + (((float)i / ((float)numHorizontalRaycasts - 1.0f)) * (objectHeight - 2.0f * skinWidth)) - (0.5f * objectHeight) + skinWidth;

			Vector2 direction = new Vector2(-1, 0); //Left

			hit = Physics2D.Raycast(origin, direction, distance);

			if(hit.transform == null)
			{
				//For testing...
				Debug.DrawLine(new Vector3(origin.x, origin.y - 0.05f, 0), new Vector3(origin.x, origin.y + 0.05f, 0), Color.red);
				Debug.DrawLine((Vector3)origin, (Vector3)origin + (Vector3)direction * distance, Color.red);
			}
			else
			{
				collisionInfo.leftCollision = true;

				//For testing...
				Debug.DrawLine(new Vector3(origin.x, origin.y - 0.05f, 0), new Vector3(origin.x, origin.y + 0.05f, 0), Color.yellow);
				Debug.DrawLine((Vector3)origin, (Vector3)origin + (Vector3)direction * distance, Color.yellow);
			}
		}

		//Raycast to the right...
		for (int i = 0; i < numHorizontalRaycasts; i++)
		{
			RaycastHit2D hit = new RaycastHit2D();
			
			//Find the origin of the raycast...
			Vector2 origin = Vector2.zero;
			origin.x = transform.position.x + ((objectWidth / 2.0f) - skinWidth);
			origin.y = transform.position.y + (((float)i / ((float)numHorizontalRaycasts - 1.0f)) * (objectHeight - 2.0f * skinWidth)) - (0.5f * objectHeight) + skinWidth;
			
			Vector2 direction = new Vector2(1, 0); //Right
			
			hit = Physics2D.Raycast(origin, direction, distance);
			
			if(hit.transform == null)
			{
				//For testing...
				Debug.DrawLine(new Vector3(origin.x, origin.y - 0.05f, 0), new Vector3(origin.x, origin.y + 0.05f, 0), Color.red);
				Debug.DrawLine((Vector3)origin, (Vector3)origin + (Vector3)direction * distance, Color.red);
			}
			else
			{
				collisionInfo.rightCollision = true;

				//For testing...
				Debug.DrawLine(new Vector3(origin.x, origin.y - 0.05f, 0), new Vector3(origin.x, origin.y + 0.05f, 0), Color.yellow);
				Debug.DrawLine((Vector3)origin, (Vector3)origin + (Vector3)direction * distance, Color.yellow);
			}
		}
	}

	void PerformVerticalRaycasts(float distance)
	{
		//Raycast up...
		for (int i = 0; i < numHorizontalRaycasts; i++)
		{
			RaycastHit2D hit = new RaycastHit2D();
			
			//Find the origin of the raycast...
			Vector2 origin = Vector2.zero;
			origin.x = transform.position.x + (((float)i / ((float)numVerticalRaycasts - 1.0f)) * (objectWidth - 2.0f * skinWidth)) - (0.5f * objectWidth) + skinWidth;
			origin.y = transform.position.y + ((objectHeight / 2.0f) - skinWidth);
			
			Vector2 direction = new Vector2(0, 1); //Up
			
			hit = Physics2D.Raycast(origin, direction, distance);
			
			if(hit.transform == null)
			{
				//For testing...
				Debug.DrawLine(new Vector3(origin.x - 0.05f, origin.y, 0), new Vector3(origin.x + 0.05f, origin.y, 0), Color.red);
				Debug.DrawLine((Vector3)origin, (Vector3)origin + (Vector3)direction * distance, Color.red);
			}
			else
			{
				collisionInfo.topCollision = true;

				//For testing...
				Debug.DrawLine(new Vector3(origin.x - 0.05f, origin.y, 0), new Vector3(origin.x + 0.05f, origin.y, 0), Color.yellow);
				Debug.DrawLine((Vector3)origin, (Vector3)origin + (Vector3)direction * distance, Color.yellow);
			}
		}

		//Raycast down...
		for (int i = 0; i < numHorizontalRaycasts; i++)
		{
			RaycastHit2D hit = new RaycastHit2D();
			
			//Find the origin of the raycast...
			Vector2 origin = Vector2.zero;
			origin.x = transform.position.x + (((float)i / ((float)numVerticalRaycasts - 1.0f)) * (objectWidth - 2.0f * skinWidth)) - (0.5f * objectWidth) + skinWidth;
			origin.y = transform.position.y + ((-objectHeight / 2.0f) + skinWidth);
			
			Vector2 direction = new Vector2(0, -1); //Down
			
			hit = Physics2D.Raycast(origin, direction, distance);
			
			if(hit.transform == null)
			{
				//For testing...
				Debug.DrawLine(new Vector3(origin.x - 0.05f, origin.y, 0), new Vector3(origin.x + 0.05f, origin.y, 0), Color.red);
				Debug.DrawLine((Vector3)origin, (Vector3)origin + (Vector3)direction * distance, Color.red);
			}
			else
			{
				collisionInfo.bottomCollision = true;

				//For testing...
				Debug.DrawLine(new Vector3(origin.x - 0.05f, origin.y, 0), new Vector3(origin.x + 0.05f, origin.y, 0), Color.yellow);
				Debug.DrawLine((Vector3)origin, (Vector3)origin + (Vector3)direction * distance, Color.yellow);
			}
		}
	}
}



















