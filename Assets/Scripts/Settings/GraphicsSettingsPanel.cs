using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panou de setări grafice: preset Quality, rezoluție și mod ecran.
/// Se atașează pe obiectul panoului. Modificările se aplică imediat.
/// Unity salvează singur rezoluția, modul ecran și nivelul Quality între sesiuni.
/// </summary>
public class GraphicsSettingsPanel : MonoBehaviour
{
    [Header("Referințe UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private Button closeButton;

    [Header("Comportament")]
    [Tooltip("Oprește timpul jocului cât timp panoul este deschis.")]
    [SerializeField] private bool pauseGameWhileOpen = true;

    private readonly List<Vector2Int> _resolutions = new List<Vector2Int>();
    private float _previousTimeScale = 1f;

    // Ordinea opțiunilor din dropdown-ul de mod ecran.
    private static readonly FullScreenMode[] ScreenModes =
    {
        FullScreenMode.FullScreenWindow,     // Fullscreen (borderless)
        FullScreenMode.ExclusiveFullScreen,  // Fullscreen exclusiv (doar Windows)
        FullScreenMode.Windowed
    };

    private static readonly string[] ScreenModeLabels =
    {
        "Fullscreen",
        "Fullscreen exclusiv",
        "Windowed"
    };

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

    private void Awake()
    {
        if (panelRoot == null) panelRoot = gameObject;

        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
        if (closeButton != null) closeButton.onClick.AddListener(Close);

        BuildQualityOptions();
        BuildResolutionOptions();
        BuildScreenModeOptions();
    }

    // ---------- Deschidere / închidere ----------

    public void Open()
    {
        if (IsOpen) return;
        panelRoot.SetActive(true); // la prima activare rulează Awake și se construiesc listele
        RefreshCurrentValues();
        if (pauseGameWhileOpen)
        {
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
    }

    public void Close()
    {
        if (!IsOpen) return;
        panelRoot.SetActive(false);
        if (pauseGameWhileOpen) Time.timeScale = _previousTimeScale;
    }

    public void Toggle()
    {
        if (IsOpen) Close(); else Open();
    }

    // ---------- Construirea opțiunilor ----------

    private void BuildQualityOptions()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(QualitySettings.names.ToList());
    }

    private void BuildResolutionOptions()
    {
        _resolutions.Clear();
        foreach (var r in Screen.resolutions)
        {
            var size = new Vector2Int(r.width, r.height);
            if (!_resolutions.Contains(size)) _resolutions.Add(size);
        }

        // Rezoluțiile din matricea de compatibilitate, dacă monitorul le suportă și lipsesc din listă.
        AddIfFits(new Vector2Int(2560, 1440));
        AddIfFits(new Vector2Int(1920, 1080));
        AddIfFits(new Vector2Int(1280, 720));

        _resolutions.Sort((a, b) => b.x != a.x ? b.x.CompareTo(a.x) : b.y.CompareTo(a.y));

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(_resolutions.Select(r => $"{r.x} x {r.y}").ToList());
    }

    private void AddIfFits(Vector2Int size)
    {
        var display = Screen.currentResolution;
        bool fits = size.x <= display.width && size.y <= display.height;
        if (fits && !_resolutions.Contains(size)) _resolutions.Add(size);
    }

    private void BuildScreenModeOptions()
    {
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(ScreenModeLabels.ToList());
    }

    /// <summary>Afișează în dropdown-uri valorile active în acest moment.</summary>
    private void RefreshCurrentValues()
    {
        qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());

        var current = new Vector2Int(Screen.width, Screen.height);
        int resIndex = _resolutions.IndexOf(current);
        if (resIndex < 0)
        {
            _resolutions.Add(current);
            resolutionDropdown.AddOptions(new List<string> { $"{current.x} x {current.y}" });
            resIndex = _resolutions.Count - 1;
        }
        resolutionDropdown.SetValueWithoutNotify(resIndex);

        int modeIndex = System.Array.IndexOf(ScreenModes, Screen.fullScreenMode);
        if (modeIndex < 0) modeIndex = 0; // MaximizedWindow (macOS) -> Fullscreen
        screenModeDropdown.SetValueWithoutNotify(modeIndex);

        qualityDropdown.RefreshShownValue();
        resolutionDropdown.RefreshShownValue();
        screenModeDropdown.RefreshShownValue();
    }

    // ---------- Aplicarea modificărilor ----------

    private void OnQualityChanged(int index)
    {
        // true = aplică și setările costisitoare (anti-aliasing etc.) imediat.
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log($"[Settings] Quality: {QualitySettings.names[index]}");
    }

    private void OnResolutionChanged(int index)
    {
        ApplyScreen(_resolutions[index], ScreenModes[screenModeDropdown.value]);
    }

    private void OnScreenModeChanged(int index)
    {
        ApplyScreen(_resolutions[resolutionDropdown.value], ScreenModes[index]);
    }

    private static void ApplyScreen(Vector2Int size, FullScreenMode mode)
    {
        Screen.SetResolution(size.x, size.y, mode);
        Debug.Log($"[Settings] Ecran: {size.x}x{size.y}, {mode}");
    }
}
