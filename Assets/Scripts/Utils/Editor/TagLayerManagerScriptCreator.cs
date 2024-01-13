using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class TagLayerManagerScriptCreator
{

    [MenuItem("Custom Tools/Update Tag Layer Manager")]
    public static void CreateManagerScripts()
    {
        CreateTagManagerScript();
        CreateLayerManagerScript();
    }

    public static void CreateTagManagerScript()
    {
        CreateScript("TagManager.cs", WriteTagManager);
    }

    public static void CreateLayerManagerScript()
    {
        CreateScript("LayerManager.cs", WriteLayerManager);
    }
    private static void CreateScript(string scriptName, Action<string> writeFunction)
    {
        // Check if the script already exists
        string scriptPath = FindScriptPath(scriptName);

        if (scriptPath != null)
        {
            // If the script already exists, use its path
            writeFunction(scriptPath);
        }
        else
        {
            // If the script doesn't exist, use the default path
            string defaultScriptPath = Application.dataPath + "/Scripts/System/TagLayerManager/" + scriptName;

            if (!Directory.Exists(Path.GetDirectoryName(defaultScriptPath)))
            {
                // Create the necessary directories if they don't exist
                Directory.CreateDirectory(Path.GetDirectoryName(defaultScriptPath));
            }

            // Write the script to the default path
            writeFunction(defaultScriptPath);
        }
    }

    private static string FindScriptPath(string scriptName)
    {
        string[] allScriptPaths = Directory.GetFiles(Application.dataPath, scriptName, SearchOption.AllDirectories);

        if (allScriptPaths.Length > 0)
        {
            return allScriptPaths[0];
        }

        return null; // Script not found
    }

    private static void WriteTagManager(string path)
    {
        string[] tagNames = UnityEditorInternal.InternalEditorUtility.tags;

        // Create the new script
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine("using UnityEngine;");
        writer.WriteLine("");
        writer.WriteLine("namespace Game.Utils");
        writer.WriteLine("{");
        writer.WriteLine("    public static class TagManager");
        writer.WriteLine("    {");
        writer.WriteLine("        // Constant variable to store all the Tag names");
        writer.WriteLine("");

        // Add each tag name to the static variable
        for (int i = 0; i < tagNames.Length; i++)
        {
            string variableName = tagNames[i];
            variableName = variableName.Replace(" ", ""); // Remove all spaces

            writer.WriteLine($"        public const string {variableName} = \"{tagNames[i]}\";");
        }

        writer.WriteLine("");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        writer.Close();

        // Refresh the asset database to make sure the new script is recognized
        AssetDatabase.Refresh();
    }

    private static void WriteLayerManager(string path)
    {
        string[] layerNames = UnityEditorInternal.InternalEditorUtility.layers;

        // Create the new script
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine("using UnityEngine;");
        writer.WriteLine("");
        writer.WriteLine("namespace Game.Utils");
        writer.WriteLine("{");
        writer.WriteLine("    public static class LayerManager");
        writer.WriteLine("    {");
        writer.WriteLine("        // Constant variable to store all the Layers");
        writer.WriteLine("");

        // Add each tag name to the static variable
        for (int i = 0; i < layerNames.Length; i++)
        {
            int layerIndex = LayerMask.NameToLayer(layerNames[i]);

            string variableName = layerNames[i];
            variableName = variableName.Replace(" ", ""); // Remove all spaces

            writer.WriteLine($"        public const int {variableName} = {layerIndex};");
        }

        writer.WriteLine("");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        writer.Close();

        // Refresh the asset database to make sure the new script is recognized
        AssetDatabase.Refresh();
    }







}//class

