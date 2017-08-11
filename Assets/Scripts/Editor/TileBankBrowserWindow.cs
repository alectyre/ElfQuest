using UnityEngine;
using System.Collections;
using UnityEditor;

public class TileBankBrowserWindow : EditorWindow {

	TileBank tileBank = null;

	bool showTileBank = false;

	[MenuItem ("Window/TileBank Browser")]
	static void Init () {
		// Get existing open window or if none, make a new one:		
		TileBankBrowserWindow window  = (TileBankBrowserWindow)EditorWindow.GetWindow (typeof (TileBankBrowserWindow));
		window.Show();
	}
	
	void OnGUI () {
		GUILayout.Label ("Here are Yo Banks", EditorStyles.boldLabel);

		float originalWidthOccupied = 5f;
		float widthOccupied = originalWidthOccupied;
		float widthOfTile = 32f;

		float originalHeightOccupied = 24;
		float heightOccupied = originalHeightOccupied;
		float heightOfTile = 32f;

		Vector2 padding = new Vector2(2,2);

		if(tileBank != null)
		{
			float foldoutWidth = position.width;
			float foldoutHeight = 16;

			showTileBank = EditorGUI.Foldout(new Rect(widthOccupied,heightOccupied,foldoutWidth,foldoutHeight),showTileBank, tileBank.name);

			widthOccupied = originalWidthOccupied = widthOccupied + 15f;
			heightOccupied += foldoutHeight + padding.y;

			if(showTileBank)
			{
				foreach(GameObject obj in tileBank.tiles)
				{
					if(obj == null)
						continue;

					SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

					if(spriteRenderer != null)
					{
						DrawTextureGUI(new Vector2(widthOccupied, heightOccupied), 
						               spriteRenderer.sprite, 
						               new Vector2(widthOfTile, heightOfTile));

						widthOccupied += widthOfTile + padding.x;

						if(widthOccupied + widthOfTile + padding.x > position.width)
						{
							heightOccupied += heightOfTile + padding.y;
							widthOccupied = originalWidthOccupied;
						}
					}
				}
			}
		}

		GUILayout.Space(heightOccupied);

		GUILayout.FlexibleSpace();

		tileBank = (TileBank)EditorGUILayout.ObjectField(tileBank , 
		                                                 typeof(TileBank), 
		                                                 false,
		                                                 GUILayout.MaxWidth(200f));

		GUILayout.Space(10f);

		if(GUILayout.Button("Create new Tile Bank", GUILayout.ExpandWidth(false)))
			ScriptableObjectUtility.CreateAsset<TileBank>();

		/*
		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
		myBool = EditorGUILayout.Toggle ("Toggle", myBool);
		myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
		EditorGUILayout.EndToggleGroup ();
		*/
	}

	void DrawTextureGUI(Vector2 position, Texture2D texture, Vector2 size)
	{
		if(texture == null)
		{
			Debug.Log("NO TEXTURE FUCK");
			return;
		}

		Vector2 actualSize = size;
		
		actualSize.y *= (texture.width / texture.width);
		GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), texture, new Rect(0,0,1,1));
	}

	void DrawTextureGUI(Vector2 position, Sprite sprite, Vector2 size)
	{
		Rect spriteRect = new Rect(sprite.rect.x / sprite.texture.width, sprite.rect.y / sprite.texture.height,
		                           sprite.rect.width / sprite.texture.width, sprite.rect.height / sprite.texture.height);
		Vector2 actualSize = size;
		
		actualSize.y *= (sprite.rect.height / sprite.rect.width);
		GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + (size.y - actualSize.y) / 2, actualSize.x, actualSize.y), sprite.texture, spriteRect);
	}
	
}
