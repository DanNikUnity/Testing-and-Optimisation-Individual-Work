using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Meniu: Tools > Bubble Wizard > Create Graphics Settings Panel.
/// Construiește în scena deschisă un buton „Setări” și panoul de setări grafice, deja legate între ele.
/// </summary>
public static class CreateGraphicsSettingsPanel
{
    private const string PanelName = "GraphicsSettingsPanel";

    [MenuItem("Tools/Bubble Wizard/Create Graphics Settings Panel")]
    public static void Create()
    {
        Canvas canvas = FindOrCreateCanvas();
        EnsureEventSystem();

        Transform existing = canvas.transform.Find(PanelName);
        if (existing != null)
        {
            if (!EditorUtility.DisplayDialog("Panou existent",
                    "Există deja un GraphicsSettingsPanel în Canvas. Îl înlocuiesc?", "Da", "Nu"))
                return;
            Undo.DestroyObjectImmediate(existing.gameObject);
            Transform oldButton = canvas.transform.Find("SettingsButton");
            if (oldButton != null) Undo.DestroyObjectImmediate(oldButton.gameObject);
        }

        var res = new TMP_DefaultControls.Resources
        {
            standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
            background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"),
            inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd"),
            knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd"),
            checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd"),
            dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd"),
            mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd")
        };

        // ---- Panoul ----
        var panel = new GameObject(PanelName, typeof(RectTransform), typeof(Image));
        Undo.RegisterCreatedObjectUndo(panel, "Create Graphics Settings Panel");
        panel.transform.SetParent(canvas.transform, false);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(560, 380);
        var panelImage = panel.GetComponent<Image>();
        panelImage.sprite = res.background;
        panelImage.type = Image.Type.Sliced;
        panelImage.color = new Color(0.08f, 0.09f, 0.13f, 0.95f);

        var layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 24, 24);
        layout.spacing = 14;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        CreateText(panel.transform, "Title", "Setări grafice", 34, FontStyles.Bold, TextAlignmentOptions.Center, 50);

        TMP_Dropdown quality = CreateRow(panel.transform, "Quality", "Calitate", res);
        TMP_Dropdown resolution = CreateRow(panel.transform, "Resolution", "Rezoluție", res);
        TMP_Dropdown screenMode = CreateRow(panel.transform, "ScreenMode", "Mod ecran", res);

        GameObject closeGo = TMP_DefaultControls.CreateButton(res);
        closeGo.name = "CloseButton";
        closeGo.transform.SetParent(panel.transform, false);
        closeGo.GetComponentInChildren<TMP_Text>().text = "Închide";
        closeGo.GetComponentInChildren<TMP_Text>().fontSize = 24;
        var closeLe = closeGo.AddComponent<LayoutElement>();
        closeLe.preferredHeight = 48;

        // ---- Componenta și legăturile ----
        var settings = panel.AddComponent<GraphicsSettingsPanel>();
        var so = new SerializedObject(settings);
        so.FindProperty("panelRoot").objectReferenceValue = panel;
        so.FindProperty("qualityDropdown").objectReferenceValue = quality;
        so.FindProperty("resolutionDropdown").objectReferenceValue = resolution;
        so.FindProperty("screenModeDropdown").objectReferenceValue = screenMode;
        so.FindProperty("closeButton").objectReferenceValue = closeGo.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        // ---- Butonul „Setări” din colțul dreapta-sus ----
        GameObject openGo = TMP_DefaultControls.CreateButton(res);
        openGo.name = "SettingsButton";
        Undo.RegisterCreatedObjectUndo(openGo, "Create Settings Button");
        openGo.transform.SetParent(canvas.transform, false);
        var openRect = openGo.GetComponent<RectTransform>();
        openRect.anchorMin = openRect.anchorMax = openRect.pivot = new Vector2(1f, 1f);
        openRect.anchoredPosition = new Vector2(-20, -20);
        openRect.sizeDelta = new Vector2(160, 48);
        openGo.GetComponentInChildren<TMP_Text>().text = "Setări";
        openGo.GetComponentInChildren<TMP_Text>().fontSize = 24;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            openGo.GetComponent<Button>().onClick, settings.Toggle);

        // Panoul e desenat peste restul UI și pornește închis.
        panel.transform.SetAsLastSibling();
        panel.SetActive(false);

        Selection.activeGameObject = panel;
        EditorUtility.SetDirty(canvas.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
        Debug.Log("[Settings] Panoul de setări a fost creat. Salvează scena (Ctrl+S).");
    }

    private static TMP_Dropdown CreateRow(Transform parent, string name, string label, TMP_DefaultControls.Resources res)
    {
        var row = new GameObject(name + "Row", typeof(RectTransform));
        row.transform.SetParent(parent, false);
        var h = row.AddComponent<HorizontalLayoutGroup>();
        h.spacing = 16;
        h.childControlWidth = true;
        h.childControlHeight = true;
        h.childForceExpandWidth = false;
        h.childForceExpandHeight = true;
        row.AddComponent<LayoutElement>().preferredHeight = 48;

        var text = CreateText(row.transform, name + "Label", label, 24, FontStyles.Normal, TextAlignmentOptions.MidlineLeft, 48);
        text.GetComponent<LayoutElement>().preferredWidth = 170;

        GameObject ddGo = TMP_DefaultControls.CreateDropdown(res);
        ddGo.name = name + "Dropdown";
        ddGo.transform.SetParent(row.transform, false);
        var ddLe = ddGo.AddComponent<LayoutElement>();
        ddLe.flexibleWidth = 1;
        ddLe.preferredHeight = 48;

        var dd = ddGo.GetComponent<TMP_Dropdown>();
        dd.captionText.fontSize = 22;
        dd.itemText.fontSize = 20;
        // Lista derulantă mai înaltă, ca să încapă toate rezoluțiile.
        dd.template.sizeDelta = new Vector2(dd.template.sizeDelta.x, 260);
        return dd;
    }

    private static TMP_Text CreateText(Transform parent, string name, string value, float size,
        FontStyles style, TextAlignmentOptions align, float height)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = value;
        t.fontSize = size;
        t.fontStyle = style;
        t.alignment = align;
        t.color = Color.white;
        go.AddComponent<LayoutElement>().preferredHeight = height;
        return t;
    }

    private static Canvas FindOrCreateCanvas()
    {
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include))
        {
            if (c.isRootCanvas && c.renderMode != RenderMode.WorldSpace && c.gameObject.name == "Canvas")
                return c;
        }
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include))
        {
            if (c.isRootCanvas && c.renderMode != RenderMode.WorldSpace)
                return c;
        }

        var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(go, "Create Canvas");
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        Undo.RegisterCreatedObjectUndo(go, "Create EventSystem");
    }
}
