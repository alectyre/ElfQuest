using UnityEngine;
using UnityEditor;

public static class ScriptableObjectUtility {

	/*
	 * ProjectWindowUtil.CreateAsset() is a wonderful function that adds an asset to
	 * the current Project window folder, the way built-in Unity assets do. For some
	 * reason it is undocumented on the Unity doc site. Such secrets.
	 */

	public static void CreateAsset<T>() where T : ScriptableObject {
		var asset = ScriptableObject.CreateInstance<T>();
		ProjectWindowUtil.CreateAsset(asset, "New " + typeof(T).Name + ".asset");
	}

	/*
	 * Example use:
	 * 
	 * using UnityEngine;
	 * using UnityEditor;
	 *
	 * static class UnityIntegration {
 	 *
	 * [MenuItem("Assets/Create/YourScriptableObject")]
	 * public static void CreateYourScriptableObject() {
	 *		ScriptableObjectUtility.CreateAsset<YourScriptableObject>();
	 * }
	 * 
	 */
 
}

