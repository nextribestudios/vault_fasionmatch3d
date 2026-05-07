using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Compilation;

[InitializeOnLoad]
public static class FirebaseDefineValidator
{
	private const string BaseDefine = "NEXTRIBE_FIREBASE";

	private static readonly string[] FeatureDefines =
	{
		"NEXTRIBE_FB_ANALYTICS",
		"NEXTRIBE_FB_MESSAGING",
		"NEXTRIBE_FB_CRASHLYTICS",
		"NEXTRIBE_FB_FIRESTORE",
		"NEXTRIBE_FB_REMOTE_CONFIG"
	};

	static FirebaseDefineValidator() { }

	[MenuItem("Tools/Force Recompile")]
	public static void Recompile()
	{
		CompilationPipeline.RequestScriptCompilation();
	}

	[MenuItem("Tools/Setup/Validate Firebase Defines")]
	public static void Validate()
	{
		var target = UnityEditor.Build.NamedBuildTarget.Android;
		string defines = PlayerSettings.GetScriptingDefineSymbols(target);

		var defineSet = new HashSet<string>(defines.Split(';'));

		bool firebaseInstalled = IsFirebaseInstalled();

		// Case 1: Base define invalid -> wipe everything
		if (!firebaseInstalled && defineSet.Contains(BaseDefine))
		{
			Debug.LogWarning("Firebase not installed but defines present. Cleaning up...");

			defineSet.Remove(BaseDefine);
			foreach (var d in FeatureDefines)
				defineSet.Remove(d);

			ApplyDefines(defineSet);
			return;
		}

		// Case 2: Feature defines without base -> fix
		if (!defineSet.Contains(BaseDefine))
		{
			bool modified = false;

			foreach (var d in FeatureDefines)
			{
				if (defineSet.Remove(d))
					modified = true;
			}

			if (modified)
			{
				Debug.LogWarning("Feature defines found without base Firebase define. Fixing...");
				ApplyDefines(defineSet);
				return;
			}
		}

		// Case 3: Validate each feature against actual SDK
		ValidateFeature(ref defineSet, "NEXTRIBE_FB_ANALYTICS", "FirebaseAnalytics");
		ValidateFeature(ref defineSet, "NEXTRIBE_FB_MESSAGING", "FirebaseMessaging");
		ValidateFeature(ref defineSet, "NEXTRIBE_FB_CRASHLYTICS", "FirebaseCrashlytics");
		ValidateFeature(ref defineSet, "NEXTRIBE_FB_FIRESTORE", "FirebaseStorage");
		ValidateFeature(ref defineSet, "NEXTRIBE_FB_REMOTE_CONFIG", "FirebaseRemoteConfig");

		ApplyDefines(defineSet);
	}

	static bool IsFirebaseInstalled()
	{
		return HasType("Firebase.FirebaseApp");
	}

	static bool HasType(string typeName)
	{
		foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
		{
			if (assembly.GetType(typeName) != null)
				return true;
		}
		return false;
	}

	static void ValidateFeature(ref HashSet<string> defineSet, string define, string folderName)
	{
		bool installed = Directory.Exists($"Assets/Firebase/{folderName}");

		if (!installed && defineSet.Contains(define))
		{
			Debug.LogWarning($"{define} present but {folderName} not installed. Removing.");
			defineSet.Remove(define);
		}
	}

	static void ApplyDefines(HashSet<string> defineSet)
	{
		var target = UnityEditor.Build.NamedBuildTarget.Android;
		string result = string.Join(";", defineSet);

		PlayerSettings.SetScriptingDefineSymbols(target, result);
	}
}