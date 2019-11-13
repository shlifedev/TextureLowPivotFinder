using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections;
 
public class TITWindow : EditorWindow
{
    static string guid = "b1d10f2b1b2da384a9942068ba9dea80";
    string path = null;
    ObjectField field = null;
    Button build = null;
    FloatField customYField = null;
    [MenuItem("HamsterLibs/Tools/LowPivotChanger")]
    public static void ShowExample()
    {
        TITWindow wnd = GetWindow<TITWindow>();
        wnd.titleContent = new GUIContent("TITWindow");
    }

    private void OnChange(ChangeEvent<Object> e)
    {
        path = AssetDatabase.GetAssetPath(e.newValue);
        if (AssetDatabase.IsValidFolder(path))
        {

        }
        else
        {
            field.value = null;
            EditorUtility.DisplayDialog("Error!", "this is not folder!!", "ok..");
        }
    }


    private void OnChange(ChangeEvent<float> e)
    {
        TextureImporterTool.custom_y_pos = e.newValue;
    }

    [MenuItem("HamsterLibs/Tool/CopyGUID")]
    public static void A()
    {
        var clipboard = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Selection.activeObject));
        var textEditor = new TextEditor(); 
        textEditor.text = clipboard;
        textEditor.SelectAll();
        textEditor.Copy();

        Debug.Log(clipboard);
    }
    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // Import UXML 

            var guidPath = AssetDatabase.GUIDToAssetPath(guid);
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(guidPath);
        VisualElement uxml = visualTree.CloneTree();
        root.Add(uxml);

        field = uxml.Query().Name("folder-field").Build().First() as ObjectField;
        customYField = uxml.Query().Name("custom-y-pos").Build().First() as FloatField;
        customYField.RegisterValueChangedCallback(OnChange);
        field.objectType = typeof(Object);
        field.RegisterValueChangedCallback(OnChange);
        build = uxml.Query().Name("build").Build().First() as Button;
        build.RegisterCallback<MouseUpEvent>(x => { 
            TextureImporterTool.FindAllFiles(path.Replace("Assets/", "/"), true, TextureImporterTool.OnFindFile);
            TextureImporterTool.LoadAndModify(TextureImporterTool.findedFilePath.ToArray());
        });
    }
}