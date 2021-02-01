using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

/// <summary>
/// Enable file sharing post build processor for iOS (Xcode)
/// </summary>
public class EnableFileSharingPostBuildProcessor 
{ 
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget _buildTarget, string pathToBuiltProject)
    {
        // Checks if building for iOS
        if (_buildTarget == BuildTarget.iOS)
        {
            // Get plist
            string path = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(path));

            // Get root
            PlistElementDict rootDict = plist.root;

            rootDict.SetBoolean("UIFileSharingEnabled", true);

            // Write to file
            File.WriteAllText(path, plist.WriteToString());
        }
    }

}
