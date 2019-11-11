using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class TITWindow : EditorWindow
{
    string path = null;
    ObjectField field = null;
    Button build = null;
    [MenuItem("HamsterLibs/Tools/LowPivotChanger")]
    public static void ShowExample()
    {
        TITWindow wnd = GetWindow<TITWindow>();
        wnd.titleContent = new GUIContent("TITWindow");
    }

    private void OnChange(ChangeEvent<Object> e)
    {
        path = AssetDatabase.GetAssetPath(e.newValue); 
        if(AssetDatabase.IsValidFolder(path))
        {
            
        }
        else
        {
            field.value = null;
            EditorUtility.DisplayDialog("Error!", "this is not folder!!", "ok..");
        }
    }
    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/TextureTool/Editor/TITWindow.uxml");
        VisualElement uxml = visualTree.CloneTree();
        root.Add(uxml);

        field = uxml.Query().Name("folder-field").Build().First() as ObjectField;
        field.objectType = typeof(Object);
        field.RegisterValueChangedCallback(OnChange);
        build = uxml.Query().Name("build").Build().First() as Button;
        build.RegisterCallback<MouseUpEvent>(x => {
            TextureImporterTool.FindAllFiles(path.Replace("Assets/", "/"), true, TextureImporterTool.OnFindFile);
            TextureImporterTool.LoadAndModify(TextureImporterTool.findedFilePath.ToArray());
        });
    }
}