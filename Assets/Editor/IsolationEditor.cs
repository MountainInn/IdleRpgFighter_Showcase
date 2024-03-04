using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class IsolationEditor : Editor
{
    protected List<GameObject> previews = new();

    protected bool showBaseInspector;
    protected bool isEditing;

    protected abstract void OnStartEditing();
    protected abstract IEnumerable<GameObject> InitPreviews();
    protected abstract Vector3 GetSceneViewLookAtPosition();

    protected abstract void ConcreteOnSceneGUI();
    protected abstract void UpdatePreviewsOnSceneGUI();

    protected abstract void ConcreteOnInspectorGUI();
    protected abstract void UpdatePreviewsOnInspectorGUI();

    protected abstract void FinalizeTargetObject();

    protected void StartEditing()
    {
        OnStartEditing();

        previews = InitPreviews().ToList();

        SceneVisibilityManager.instance.Isolate(previews.ToArray(), true);

        SceneView.lastActiveSceneView.LookAt( GetSceneViewLookAtPosition() );
        SceneView.duringSceneGui += OnSceneGUI;

        isEditing = true;
    }


    protected void StopEditing()
    {
        FinalizeTargetObject();

        previews.DestroyAllImmediate();
        previews.Clear();

        SceneVisibilityManager.instance.ExitIsolation();

        SceneView.duringSceneGui -= OnSceneGUI;

        isEditing = false;

        OnInspectorGUI();
    }

    public void OnSceneGUI(SceneView sceneView)
    {
        if (!isEditing) return;

        serializedObject.Update();

        ConcreteOnSceneGUI();

        serializedObject.ApplyModifiedProperties();

        UpdatePreviewsOnSceneGUI();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        showBaseInspector = EditorGUILayout.Toggle("Show Base Inspector", showBaseInspector);

        if (showBaseInspector)
            base.OnInspectorGUI();

        EditorGUILayout.Space();

        ConcreteOnInspectorGUI();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        serializedObject.Update();

        if (!isEditing)
        {
            if (GUILayout.Button("Start Editing"))
            {
                StartEditing();
            }

            return;
        }
        else
        {
            if (GUILayout.Button("Stop Editing"))
            {
                StopEditing();
                return;
            }
        }

        UpdatePreviewsOnInspectorGUI();
    }
}
