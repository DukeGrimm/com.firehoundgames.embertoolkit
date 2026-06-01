using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.PackageManager;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace EmberToolkit.Editor.Behaviours
{
    public static class CreateEmberBehaviourMenu
    {
        private const string trueTemplatePath = "Packages/com.firehoundgames.embertoolkit/Editor/Behaviours/EmberBehaviour.txt";
        private const string MenuPath = "Assets/Create/Scripting/EmberBehaviour";
        private const int MenuPriority = 10; // lower = higher in the menu

        // Template contains a CLASSNAME token that will be replaced with the chosen filename (without extension).
        //

        [MenuItem(MenuPath, false, MenuPriority)]
        public static void Create()
        {
            // Find the active folder path where the user right-clicked
            string path = "Assets";
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            string defaultFileName = "NewEmberBehaviour.cs";    
            string fullPath = Path.Combine(path, defaultFileName);

            // Create the action instance that runs when renaming is completed
            CreateEmberBehaviourEndNameEdit action = ScriptableObject.CreateInstance<CreateEmberBehaviourEndNameEdit>();
            string templatePath = GetTemplatePath();
            if (string.IsNullOrEmpty(templatePath))
            {
                Debug.LogError("Could not find EmberBehaviour template. Please ensure the template file exists at: " + trueTemplatePath);
                return;
            }

            // Trigger the native Unity rename overlay in the project window
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                EntityId.None,
                action,
                fullPath,
                (Texture2D)EditorGUIUtility.IconContent("cs Script Icon").image,
                templatePath
            );
        }

        // validation method
        [MenuItem(MenuPath, true)]
        public static bool CreateValidation()
        {
            // Allow creation when project has any selection or fallback to Assets
            return true;
        }

        private static string GetSelectedPathOrFallback()
        {
            string path = "Assets";
            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path)) continue;
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }
            return path.Replace("\\", "/");
        }

        private static string GetTemplatePath()
        {
            try
            {
                var pkg = PackageInfo.FindForAssetPath("Packages/com.firehoundgames.embertoolkit");
                if (pkg != null)
                {
                    string candidatePath = Path.Combine(pkg.resolvedPath, "Runtime", "Editor", "Behaviours", "EmberBehaviour.txt");
                    if (File.Exists(candidatePath))
                    {
                        return candidatePath;
                    }
                }
            }
            catch
            {
                Debug.LogWarning("Could not find EmberToolkit package. Falling back to default template path.");
            }
            return null;
        }

        // Find the best namespace for the given asset folder path (e.g. "Assets/.../Folder" or "Packages/...").
        // Walks upward from the folder and looks for a *.asmdef file. If found,
        // attempts to read "rootNamespace" from the asmdef JSON; falls back to the "name" property.
        private static string FindNamespaceForAssetFolder(string assetFolderPath)
        {
            if (string.IsNullOrEmpty(assetFolderPath))
                return null;

            // Normalize separators
            assetFolderPath = assetFolderPath.Replace('\\', '/');

            // Convert to filesystem path
            string fsFolder;
            if (assetFolderPath.StartsWith("Assets"))
            {
                var relative = assetFolderPath.Substring("Assets".Length).TrimStart('/', '\\');
                fsFolder = Path.Combine(Application.dataPath, relative).Replace('/', Path.DirectorySeparatorChar);
            }
            else if (assetFolderPath.StartsWith("Packages"))
            {
                var projectRoot = Path.GetDirectoryName(Application.dataPath);
                var relative = assetFolderPath.Substring("Packages".Length).TrimStart('/', '\\');
                fsFolder = Path.Combine(projectRoot, "Packages", relative).Replace('/', Path.DirectorySeparatorChar);
            }
            else
            {
                // fallback assume project-relative
                var projectRoot = Path.GetDirectoryName(Application.dataPath);
                fsFolder = Path.Combine(projectRoot, assetFolderPath).Replace('/', Path.DirectorySeparatorChar);
            }

            var dir = new DirectoryInfo(fsFolder);
            var projectRootDir = new DirectoryInfo(Path.GetDirectoryName(Application.dataPath));

            // Walk upward until we reach the project root's parent (covers Packages too)
            while (dir != null)
            {
                // Check for asmdef files in current directory
                var asmdefs = dir.GetFiles("*.asmdef", SearchOption.TopDirectoryOnly);
                if (asmdefs.Length > 0)
                {
                    foreach (var asm in asmdefs)
                    {
                        try
                        {
                            var json = File.ReadAllText(asm.FullName);
                            var rootNs = GetJsonStringProperty(json, "rootNamespace");
                            if (!string.IsNullOrWhiteSpace(rootNs))
                                return rootNs.Trim();

                            var name = GetJsonStringProperty(json, "name");
                            if (!string.IsNullOrWhiteSpace(name))
                                return name.Trim();
                        }
                        catch
                        {
                            // ignore and try next asmdef
                        }
                    }
                }

                // stop condition: if we've reached a parent outside project root and not in Packages, stop
                // Allow walking up through Packages as well; stop once dir.Parent is null
                dir = dir.Parent;
            }

            return null;
        }

        // Very small helper to extract a string property from simple JSON content.
        // Not a full JSON parser but adequate for small asmdef files.
        private static string GetJsonStringProperty(string json, string propertyName)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(propertyName))
                return null;

            var prop = $"\"{propertyName}\"";
            var idx = json.IndexOf(prop, System.StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return null;

            var colon = json.IndexOf(':', idx + prop.Length);
            if (colon < 0) return null;

            // find first quote after colon
            var firstQuote = json.IndexOf('"', colon + 1);
            if (firstQuote < 0) return null;
            var secondQuote = json.IndexOf('"', firstQuote + 1);
            if (secondQuote < 0) return null;

            return json.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
        }

        // EndNameEditAction invoked after the user finishes typing the file name in the Project window.
        private class CreateEmberBehaviourEndNameEdit : AssetCreationEndAction
        {
            public override void Action(EntityId entityId, string pathName, string resourceFile)
            {
                // Read the text file template contents
                string templateText = File.ReadAllText(resourceFile);

                // Extract the class name from the final asset path chosen by the user
                string className = Path.GetFileNameWithoutExtension(pathName).Replace(" ", "");

                // Replace our token with the actual class name
                templateText = templateText.Replace("#SCRIPTNAME#", className);

                // Determine namespace based on the asmdef found for the chosen folder
                string assetFolder = Path.GetDirectoryName(pathName).Replace('\\', '/'); // e.g. "Assets/Some/Folder"
                string ns = FindNamespaceForAssetFolder(assetFolder);

                if (!string.IsNullOrWhiteSpace(ns))
                {
                    // If the template contains a namespace token, use it
                    if (templateText.Contains("#NAMESPACE#"))
                    {
                        templateText = templateText.Replace("#NAMESPACE#", ns);
                    }
                    else
                    {
                        // If file already contains a namespace declaration, do nothing
                        if (!Regex.IsMatch(templateText, @"\bnamespace\b"))
                        {
                            // Attempt to wrap the class in the namespace.
                            // Find the first occurrence of the class declaration for the generated class.
                            var classPattern = $@"(^\s*(public\s+)?(partial\s+)?(sealed\s+)?(abstract\s+)?class\s+{Regex.Escape(className)}\b)";
                            var m = Regex.Match(templateText, classPattern, RegexOptions.Multiline);
                            if (m.Success)
                            {
                                int insertPos = m.Index;
                                // Insert namespace block before the class declaration
                                var namespaceHeader = $"namespace {ns}\n{{\n";
                                templateText = templateText.Substring(0, insertPos) + namespaceHeader + templateText.Substring(insertPos);
                                // Append closing brace at end of file
                                templateText = templateText + "\n}";
                            }
                            else
                            {
                                // If we couldn't find the class line, fall back to replacing a token if present,
                                // otherwise just prepend namespace and append closing brace.
                                templateText = $"namespace {ns}\n{{\n{templateText}\n}}";
                            }
                        }
                    }
                }

                // Write the finalized C# file to disk
                UTF8Encoding encoding = new UTF8Encoding(true, false);
                File.WriteAllText(pathName, templateText, encoding);

                // Import the newly created asset into the Unity project database
                AssetDatabase.ImportAsset(pathName);

                // Highlight and focus the newly created script asset
                Object obj = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
                ProjectWindowUtil.ShowCreatedAsset(obj);
            }
        }
    }
}