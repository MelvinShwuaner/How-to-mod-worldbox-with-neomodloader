using BepInEx;

namespace MyMod
{
    public class AssetLoader
    {
        const string AssetFolder = "Folder where my assets are stored"; //the folder in our mod which contains the assets
        public static void AddAllAssets()
        {
            string AssetPath = Paths.PluginPath + AssetFolder; //get the full file path of the folder where our assets are stored
            LoadAssetsInFolder(AssetPath); //load the file path
        }
        public static void LoadAssetsInFolder(string pPath) //loads all the files DIRECTLY in a folder, directories need to have this function called on them
        {
            //for the sake of demonstration, lets assume the asset folder has a file in it called "Texture1" and a
            //subfolder called "SubFolder" containing a file called "Texture2"
            string[] files = Directory.GetFiles(pPath); //get all of the file paths which are directly in the folder, aka it only gets Texture1
            
            foreach (string FilePath in files) //go through the list of files
            {
                if (!FilePath.Contains(".json")) //if the file is not a JSON FILE, json files are explained in chapter 5
                {
                    loadTexture(FilePath); //load the texture, adding it to our temporary database which we will use later
                }
            }
            string[] directories = Directory.GetDirectories(pPath); //get all of the subfolders, in this case we get the path "SubFolder"
            foreach (string DirectoryPath in directories)
            {
                LoadAssetsInFolder(DirectoryPath); //we then recursively load this folder, repeating the process, basically the code repeats this function but
                //on the subfolder, getting "Texture2" and then repeat on any subfolders (if there are any) in the subfolder!
            }
        }
        private static void loadTexture(string pPath)
        {
            byte[] Bytes = File.ReadAllBytes(pPath); //all of the bytes stored in the file, which represent a bitmap image
            string Path = pPath.Remove(0, pPath.IndexOf("/"+AssetFolder) + AssetFolder.Length+2).Replace('\\', '/'); //we cut the path to the path that is only necessary, for example
            //if the mod was contained in steamapps/common/worldbox/Bepinex/plugins/YourMod/AssetFolder/SubFolder/Texture2, we cut this to SubFolder/Texture2
            SpriteTextureLoader.addSprite(Path, Bytes); //load the sprite and add it to the game's secoundary data base
        }
    }
}
