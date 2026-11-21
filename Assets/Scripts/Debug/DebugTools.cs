using System.Linq;
using UnityEngine;

/// <summary>
/// Unelte de testare (cheat-uri). Implicit funcționează în orice build.
/// Bifează „Only In Development Build” ca să le limitezi la Editor și Development Build.
///
/// Personajul selectat este cel pe care ai dat clic (același ca ținta bulelor).
///   F1      – arată / ascunde panoul
///   Delete  – omoară personajul selectat
///   H       – vindecă personajul selectat la HP maxim
///   M       – mana plină
///   K       – omoară inamicii din apropierea echipei (lupta curentă)
///   N       – omoară toți inamicii din camerele obișnuite (nu și Boss-ul)
///   B       – omoară Boss-ul
///   L       – omoară toată echipa (test Game Over)
///   G       – God mode pentru echipă (HP-ul aliaților nu scade)
///   T       – viteza jocului x1 / x3
/// Fiecare acțiune scrie o linie „[DEBUG]” în Console / Player.log, utilă ca dovadă.
/// </summary>
public class DebugTools : MonoBehaviour
{
    public float nearbyRadius = 40f;

    [Tooltip("Debifat: panoul merge în orice build. Bifat: doar în Editor și în Development Build.")]
    public bool onlyInDevelopmentBuild = false;

    [Tooltip("Panoul este vizibil la pornire (F1 îl ascunde / arată).")]
    public bool showPanelOnStart = true;

    private bool showPanel = true;
    private bool godMode;
    private bool fastTime;
    private Spelling spelling;
    private Transform team;
    private string lastAction = "-";

    private void Awake()
    {
        if (onlyInDevelopmentBuild && !Debug.isDebugBuild)
        {
            Destroy(gameObject);
            return;
        }
        showPanel = showPanelOnStart;
    }

    private void Start()
    {
        spelling = FindAnyObjectByType<Spelling>();
        var movement = FindAnyObjectByType<CharacterMovement>();
        if (movement != null) team = movement.transform;
    }

    private Character Selected => spelling != null ? spelling.toUseOn : null;

    private bool IsAlly(Character c) => team != null && c.transform.IsChildOf(team);

    private bool IsBoss(Character c) => c.CompareTag("Boss");

    private Character[] Alive() => FindObjectsByType<Character>().Where(c => c != null && c.CurrentHealth > 0).ToArray();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) showPanel = !showPanel;

        if (Input.GetKeyDown(KeyCode.Delete) && Selected != null)
        {
            var c = Selected;
            Log($"Delete: omorât {c.name} (HP {c.CurrentHealth}/{c.MaxHealth})");
            Kill(c);
        }

        if (Input.GetKeyDown(KeyCode.H) && Selected != null)
        {
            Selected.CurrentHealth = Selected.MaxHealth;
            Log($"H: {Selected.name} vindecat la {Selected.MaxHealth} HP");
        }

        if (Input.GetKeyDown(KeyCode.M) && spelling != null)
        {
            spelling.AddMana(spelling.MaxMana);
            Log("M: mana plină");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Vector3 center = team != null ? team.position : Vector3.zero;
            var targets = Alive().Where(c => !IsAlly(c) && Vector3.Distance(c.transform.position, center) <= nearbyRadius).ToArray();
            foreach (var c in targets) Kill(c);
            Log($"K: omorâți {targets.Length} inamici din apropiere");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            var targets = Alive().Where(c => !IsAlly(c) && !IsInBossRoom(c)).ToArray();
            foreach (var c in targets) Kill(c);
            Log($"N: omorâți {targets.Length} inamici din camerele obișnuite");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            var bosses = Alive().Where(c => !IsAlly(c) && IsBoss(c)).ToArray();
            foreach (var c in bosses) Kill(c);
            Log($"B: omorâți {bosses.Length} Boss");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            var allies = Alive().Where(IsAlly).ToArray();
            foreach (var c in allies) Kill(c);
            Log($"L: omorâți {allies.Length} aliați (test Game Over)");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            godMode = !godMode;
            Log("G: God mode " + (godMode ? "ON" : "OFF"));
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            fastTime = !fastTime;
            Time.timeScale = fastTime ? 3f : 1f;
            Log("T: viteza jocului x" + Time.timeScale);
        }
    }

    private void LateUpdate()
    {
        if (!godMode) return;
        foreach (var c in Alive().Where(IsAlly))
            c.CurrentHealth = c.MaxHealth;
    }

    private static bool IsInBossRoom(Character c)
    {
        for (var t = c.transform; t != null; t = t.parent)
            if (t.name.StartsWith("BOSSROOM")) return true;
        return false;
    }

    // Moartea fără damage: același efect ca în Character.TakeDamage când HP ajunge la 0
    // (bara de viață este copil al personajului și dispare odată cu el).
    private static void Kill(Character c)
    {
        if (c == null) return;
        c.CurrentHealth = 0;
        Destroy(c.gameObject);
    }

    private void Log(string message)
    {
        lastAction = $"{Time.time:F1}s  {message}";
        Debug.Log("[DEBUG] " + message);
    }

    private void OnGUI()
    {
        if (!showPanel) return;

        var c = Selected;
        string selected = c != null
            ? $"{c.name}  HP {c.CurrentHealth}/{c.MaxHealth}  bulă: {(c.activeBubble != null ? c.activeBubble.Name : "-")}  {(IsAlly(c) ? "aliat" : "inamic")}"
            : "nimic (clic pe un personaj)";

        GUILayout.BeginArea(new Rect(10, 10, 430, 290), GUI.skin.box);
        GUILayout.Label("<b>DEBUG TOOLS</b>  (F1 ascunde)", new GUIStyle(GUI.skin.label) { richText = true });
        GUILayout.Label("Selectat: " + selected);
        GUILayout.Label($"Mana: {(spelling != null ? spelling.currentMana : 0)}   God mode: {(godMode ? "ON" : "OFF")}   Viteză: x{Time.timeScale}");
        GUILayout.Space(4);
        GUILayout.Label("Delete omoară selectatul   H vindecă   M mana plină");
        GUILayout.Label("K omoară inamicii din lupta curentă   N toate camerele");
        GUILayout.Label("B omoară Boss-ul   L omoară echipa (Game Over)");
        GUILayout.Label("G God mode   T viteză x1/x3");
        GUILayout.Space(4);
        GUILayout.Label("Ultima acțiune: " + lastAction);
        GUILayout.EndArea();
    }
}
