using System.IO;
using System.IO.Compression;
using System.Net;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Compilation;

namespace Nextribe
{
	public class FirebaseDownloader
	{
		private const string FirebaseUrl =
		"https://firebase.google.com/download/unity";

		private const string ZipPath = "Temp/firebase_sdk.zip";
		private const string ExtractPath = "Temp/firebase_sdk";

		private static void AddScriptingDefine(string define)
		{
			var target = EditorUserBuildSettings.selectedBuildTargetGroup;

#if UNITY_ANDROID
			string defines = PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android);
#endif
#if UNITY_IOS
        string defines = PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android);
#endif

			if (!defines.Contains(define))
			{
				defines = string.IsNullOrEmpty(defines)
					? define
					: defines + ";" + define;
#if UNITY_ANDROID
				PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android, defines);
#endif
#if UNITY_IOS

			PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android, defines);
#endif
				Debug.Log("Added scripting define: " + define);
			}
			else
			{
				Debug.Log("Scripting define already present: " + define);
			}
		}

		private static void OnCompilationFinished(object obj)
		{
			CompilationPipeline.compilationFinished -= OnCompilationFinished;
			AddScriptingDefine("NEXTRIBE_FIREBASE");
		}

		[MenuItem("NexTribe/Setup/Install Firebase SDK")]
		public static void DownloadAndInstall()
		{
			try
			{
				Debug.Log("Starting Firebase SDK download...");

				if (!Directory.Exists("Temp"))
					Directory.CreateDirectory("Temp");

				// Download Firebase SDK
				using (WebClient client = new WebClient())
				{
					client.DownloadFile(FirebaseUrl, ZipPath);
				}

				Debug.Log("Firebase SDK downloaded.");

				// Clean previous extraction
				if (Directory.Exists(ExtractPath))
					Directory.Delete(ExtractPath, true);

				// Extract zip
				ZipFile.ExtractToDirectory(ZipPath, ExtractPath);

				Debug.Log("Firebase SDK extracted.");

				string unityPackageFolder = Path.Combine(
					ExtractPath,
					"firebase_unity_sdk"
				);

				// Import required Firebase packages
				ImportPackage("FirebaseAnalytics.unitypackage", unityPackageFolder);
				ImportPackage("FirebaseCrashlytics.unitypackage", unityPackageFolder);
				ImportPackage("FirebaseMessaging.unitypackage", unityPackageFolder);
				ImportPackage("FirebaseRemoteConfig.unitypackage", unityPackageFolder);

				CompilationPipeline.compilationFinished += OnCompilationFinished;

				Debug.Log("Firebase installation completed successfully.");

				// Update .gitignore if present
				UpdateGitIgnore();
			}
			catch (System.Exception e)
			{
				Debug.LogError("Firebase installation failed: " + e.Message);
			}
		}

		private static void ImportPackage(string packageName, string folder)
		{
			string packagePath = Path.Combine(folder, packageName);

			if (File.Exists(packagePath))
			{
				Debug.Log("Importing package: " + packageName);
				AssetDatabase.ImportPackage(packagePath, false);
			}
			else
			{
				Debug.LogWarning("Package not found: " + packageName);
			}
		}

		private static void UpdateGitIgnore()
		{
			string projectRoot = Directory.GetParent(Application.dataPath).FullName;
			string gitignorePath = Path.Combine(projectRoot, ".gitignore");

			if (!File.Exists(gitignorePath))
			{
				Debug.Log("No .gitignore found at project root. Skipping update.");
				return;
			}

			string[] entries =
			{
			"Assets/Firebase",
			"Assets/ExternalDependencyManager",
			"Temp/firebase_sdk",
			"Temp/firebase_sdk.zip"
		};

			HashSet<string> lines = new HashSet<string>(File.ReadAllLines(gitignorePath));
			bool modified = false;

			foreach (var entry in entries)
			{
				if (!lines.Contains(entry))
				{
					lines.Add(entry);
					modified = true;
				}
			}

			if (modified)
			{
				File.WriteAllLines(gitignorePath, lines);
				Debug.Log(".gitignore updated with Firebase ignore rules.");
			}
			else
			{
				Debug.Log(".gitignore already contains Firebase rules.");
			}
		}
	}
}