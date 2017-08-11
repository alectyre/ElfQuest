using UnityEngine;
using System.Collections;

/*
 * Draws some useful shapes with Debug.DrawLine()
 * 
 * FYI this stuff only intended to work in 2D games with standard camera orientation 
 */

public class DebugScript : MonoBehaviour {

	private const float DEFAULT_LIFESPAN = 0.001f;

	public static void DrawRect(Rect rect, Color color)
	{
		DrawRect(rect, color, DEFAULT_LIFESPAN);
	}

	public static void DrawRect(Rect rect, Color color, float lifespan)
	{
		Vector3 upperLeft = new Vector3(rect.x, rect.y, 0);
		Vector3 upperRight = new Vector3(rect.x + rect.width, rect.y, 0);
		Vector3 lowerRight = new Vector3(rect.x + rect.width, rect.y + rect.height, 0);
		Vector3 lowerLeft = new Vector3(rect.x, rect.y + rect.height, 0);

		Debug.DrawLine(upperLeft, upperRight, color, lifespan);
		Debug.DrawLine(upperRight, lowerRight, color, lifespan);
		Debug.DrawLine(lowerRight, lowerLeft, color, lifespan);
		Debug.DrawLine(lowerLeft, upperLeft, color, lifespan);
	}

	public static void DrawCircle(int vertexCount, float radius, Vector3 offset, Color color)
	{
		DrawCircle(vertexCount, radius, offset, color, DEFAULT_LIFESPAN);
	}

	public static void DrawCircle(int vertexCount, float radius, Vector3 offset, Color color, float lifespan)
	{
		DrawArc(vertexCount, radius, 360, 0, offset, color, lifespan);
	}

	public static void DrawArc(int vertexCount, float radius, float degrees, float rotation, Vector3 offset, Color color)
	{
		DrawArc(vertexCount, radius, degrees, rotation, offset, color, DEFAULT_LIFESPAN);
	}

	public static void DrawArc(int vertexCount, float radius, float degrees, float rotation, Vector3 offset, Color color, float lifespan)
	{
		vertexCount = 100;
		
		Vector3[] points = new Vector3[vertexCount];
		
		float stepSize = (degrees/180)*Mathf.PI / (vertexCount - 1); //Minus one before first step is zero!
		float theta = 0f;
		
		for(int i = 0; i < vertexCount; i++ )
		{
			// Calculate position of point
			float x = (radius) * Mathf.Cos(theta);
			float y = (radius) * Mathf.Sin(theta);
			
			// Set the position of this point
			Vector3 pos = new Vector3(x, y, 1);
			points[i] = pos;
			theta += stepSize;
		}
		
		for(int i = 0; i < points.Length; i++)
		{
			//Rotate around origin by rotation
			points[i] = Quaternion.Euler(0,0,rotation) * points[i];
			
			//Offset by offset
			points[i] += offset;
		}
		
		for(int i = 0; i < points.Length-1; i++)
		{
			Vector3 startPoint = points[i];
			
			Vector3 endPoint = points[i+1];
			
			Debug.DrawLine(startPoint, endPoint, color, lifespan);
		}

	}

	public static void DrawCross(Vector3 location, float size, Color color)
	{
		DrawCross(location, size, color, DEFAULT_LIFESPAN);
	}

	public static void DrawCross(Vector3 location, float size, Color color, float lifespan)
	{
		//Horizontal line
		Vector3 x1 = new Vector3(location.x - size/2, location.y, location.z);
		Vector3 x2 = new Vector3(location.x + size/2, location.y, location.z);
		Debug.DrawLine(x1, x2, color, lifespan);

		//Vertical line
		Vector3 y1 = new Vector3(location.x, location.y - size/2, location.z);
		Vector3 y2 = new Vector3(location.x, location.y + size/2, location.z);
		Debug.DrawLine(y1, y2, color, lifespan);
	}
}
