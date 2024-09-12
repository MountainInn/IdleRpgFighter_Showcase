using UnityEditor;
using UnityEngine;

[CustomPreview(typeof(CombatantTemplate))]
public class CombatantTemplatePreview : ObjectPreview
{
    PreviewRenderUtility previewRender;
    GameObject previewCharacter;
    CombatantTemplate template;

    float x, y, z;
    float intensity;

    public void OnDisable()
    {
        Cleanup();
    }

    public void OnDestroy()
    {
        Cleanup();
    }

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewSettings()
    {
        // x = EditorGUILayout.Slider( x, -180, 180, GUILayout.Width( 100f ) );
        // y = EditorGUILayout.Slider( y, -180, 180, GUILayout.Width( 100f ) );
        // z = EditorGUILayout.Slider( z, -180, 180, GUILayout.Width( 100f ) );
        // intensity = EditorGUILayout.Slider( intensity, 1, 10, GUILayout.Width( 100f ) );
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        template ??= (target as CombatantTemplate);
        previewRender ??= new();
        previewCharacter ??= GameObject.Instantiate(TemplateEditorData.instance.modularCharacterPrefab,
                                                    Vector3.zero,
                                                    Quaternion.identity);

        template.ApplyTemplate(previewCharacter);

        previewRender.BeginPreview(r, background);

        previewRender.AddSingleGO(previewCharacter);

        previewRender.camera.backgroundColor = Color.gray;
        previewRender.camera.clearFlags = CameraClearFlags.Color;
        previewRender.camera.transform.position = new Vector3( 0f, 0.85f, 8.5f);
        previewRender.camera.transform.rotation = Quaternion.Euler(0, 180, 0);
        previewRender.camera.nearClipPlane = 0.3f;
        previewRender.camera.farClipPlane = 15f;

        previewRender.lights[0].type = LightType.Directional;
        previewRender.lights[0].intensity = 2;
        previewRender.lights[0].transform.rotation = Quaternion.Euler(85, -160, 0);

        previewRender.Render();

        previewRender.EndAndDrawPreview(r);
    }

    public override void Cleanup()
    {

        if (previewRender != null)
        {
            previewRender.Cleanup();
            previewRender = null;
        }

        if (previewCharacter != null)
        {
            GameObject.DestroyImmediate( previewCharacter );
        }

        base.Cleanup();
    }
}
