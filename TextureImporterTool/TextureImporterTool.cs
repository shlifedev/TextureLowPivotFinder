using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureImporterTool
{
    static string initalPath = null;
    public static bool IsCenterMode = false;
    public static List<string> findedFilePath = new List<string>();

    public static bool IsReadable = false; 
    public static void ExampleRun()
    {
        FindAllFiles("/Sprites/idle", true, OnFindFile);
        var array = findedFilePath.ToArray();
        Debug.Log(array.Length);
        LoadAndModify(array);
        findedFilePath.Clear();
    }
    public static void OnFindFile(string file)
    {
        findedFilePath.Add(file);
    }
    /// <summary>
    /// 폴더의 트리구조 탐색후 파일정보 얻어오자.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isAssetPath"></param>
    /// <param name="action"></param>
    public static void FindAllFiles(string path, bool isAssetPath, System.Action<string> action = null, bool clearList = false)
    {
        if (clearList) findedFilePath.Clear();
        if (isAssetPath) path = UnityEngine.Application.dataPath + path;
        DirectoryInfo di = new DirectoryInfo(path);
        var dirs = di.GetDirectories();
        foreach (var dir in dirs)
        {
            FindAllFiles(dir.FullName, false, OnFindFile);
        }

        foreach (var file in di.GetFiles())
        {
            if (file.Extension == ".png")
            {
                var t = file.FullName;
                var splits = t.Split('\\');
                bool write = false;
                string writeBuffer = null;
                foreach (var data in splits)
                {
                    if (data == "Assets")
                    {
                        write = true;
                    }

                    if (write)
                    {
                        writeBuffer += data + "/";
                    }
                }
                writeBuffer = writeBuffer.Remove(writeBuffer.Length - 1, 1);
                action?.Invoke(writeBuffer);
            }
        }


    }

    public static void LoadAndModify(string assetPath)
    {
        var p = new GameObject();
        
        LoadAndModify(new string[] { assetPath });
    }
    public static void LoadAndModify(string[] assetPaths)
    {
        TextureImporter ti;
        var lowPivot = FindLowPivot(assetPaths); 
        foreach (var assetPath in assetPaths)
        {
            Texture2D assetT2D = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
            ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (ti != null)
            {
                //readable 처리
                ti.isReadable = true;
                
                //텍스쳐 세팅정보 읽어오기
                TextureImporterSettings texSettings = new TextureImporterSettings();
                ti.ReadTextureSettings(texSettings);
                texSettings.spriteAlignment = (int)SpriteAlignment.Custom;
                ti.SetTextureSettings(texSettings);

                //y피봇적용
                var yPivot = lowPivot;
                float xPivot = ti.spritePivot.x;
                if (IsCenterMode)
                {
                    xPivot = 0.5f;
                }
                ti.spritePivot = new Vector2(xPivot, yPivot); 
                ti.isReadable = false; 
            }
        } 
        AssetDatabase.Refresh();
        findedFilePath.Clear();

    }

    /// <summary>
    /// 가장 낮은 피봇찾기.
    /// </summary> 
    public static float FindLowPivot(string[] assetPaths)
    {
        float retValue = float.MaxValue;
        TextureImporter ti;
        foreach (var assetPath in assetPaths)
        {
            Texture2D assetT2D = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;
            ti = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (ti != null)
            {
                //readable 처리
                ti.isReadable = true;
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                //y피봇적용
                var yPivot = GetYEndPixel(assetT2D).Item1;
                if (retValue > yPivot) retValue = yPivot;
                ti.isReadable = false; 
            }
        }

        return retValue;
    } 

    public static float custom_y_pos = -1;
    static (float, bool) GetYEndPixel(Texture2D texture2D)
    {
        if(custom_y_pos != -1)
        {
            return (custom_y_pos, true);
        }
        else 
        {
            var colors = texture2D.GetPixels(); //픽셀 가져옴
            int cnt = 0; //카운트계산
            foreach (var data in colors)
            {
                if (data.a != 0)
                {
                    var p = (float)cnt / (float)texture2D.height; //  처음 발견되는 transparent 값이 0인 픽셀 위치의 배열번호에서 height나눠서 위치구하기.
                    return (p / texture2D.height, true);
                }
                cnt++;
            }
            return (0.5f, false);
        }
  
       
    }
}
