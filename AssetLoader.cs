using BepInEx;
using UnityEngine;

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
            //once we finish loading all of the files, we need to take our files from our temporary database and put them in the games database
            foreach (KeyValuePair<string, Sprite[]> keyValuePair in cached_assets_list)
            {
                //SpriteTextureLoader.cached_sprite_list is the secoundary database which stores textures
                if (SpriteTextureLoader.cached_sprite_list.ContainsKey(keyValuePair.Key))//check if the secoundary data base already stores this texture, and if so, override it
                {
                    SpriteTextureLoader.cached_sprite_list[keyValuePair.Key] = keyValuePair.Value;
                }
                else//if it doesnt, simply add it
                {
                    SpriteTextureLoader.cached_sprite_list.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
        private static void loadTexture(string pPath)
        {
            string FileName = System.IO.Path.GetFileNameWithoutExtension(pPath); //gets the name of the texture without the extention,
            //for example if it was "Texture1.png" we would get Texture1
            byte[] Bytes = File.ReadAllBytes(pPath); //all of the bytes stored in the file, which represent a bitmap image
            string Path = pPath.Remove(0, pPath.IndexOf("/"+AssetFolder) + AssetFolder.Length+2).Replace('\\', '/'); //we cut the path to the path that is only necessary, for example
            //if the mod was contained in steamapps/common/worldbox/Bepinex/plugins/YourMod/AssetFolder/SubFolder/Texture2, we cut this to SubFolder/Texture2
            addSpriteList(Path, FileName, Bytes); //load the sprite and add it to our temporary data base
        }
        public static void addSpriteList(string pPathID, string pSpriteName, byte[] pBytes)
        {
            Texture2D texture2D = new Texture2D(1, 1); //create a texture
            texture2D.filterMode = FilterMode.Point; //it should be set to Point mode to not be blurry
            if (texture2D.LoadImage(pBytes)) //load all of the bytes, and convert them into an image, this function is a extention from "UnityEngine.imageconversionmodel.dll" stored in the managed folder
            {
                Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height); //create a rectangle which is the size of the image
                Vector2 vector = new Vector2(0.5f, 0.5f); //get the center of the image
                Sprite sprite = Sprite.Create(texture2D, rect, vector, 1f); //create a sprite by using the texture and the rectangle
                sprite.name = pSpriteName; //set its name
                if (!cached_assets_list.ContainsKey(pPathID)) //check if our database already has this file, if it does override, if not add
                {
                    cached_assets_list.Add(pPathID, new Sprite[] { sprite });
                }
                else
                {
                    cached_assets_list[pPathID] = cached_assets_list[pPathID].Concat(new Sprite[] { sprite }).ToArray<Sprite>();
                }
            }
        }
        public static Dictionary<string, Sprite[]> cached_assets_list = new Dictionary<string, Sprite[]>();

    }
}
