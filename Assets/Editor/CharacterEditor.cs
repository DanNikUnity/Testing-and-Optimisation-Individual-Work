using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Character character = (Character)target;

        character.MaxHealth = EditorGUILayout.IntField("Max Health", character.MaxHealth);
        character.CurrentHealth = EditorGUILayout.IntField("Current Health", character.CurrentHealth);
        character.Damage = EditorGUILayout.IntField("Damage", character.Damage);
        character.DontAttack = EditorGUILayout.Toggle("Don't Attack", character.DontAttack);
        character.isPhysical = EditorGUILayout.Toggle("is Physical", character.isPhysical);

        character.projectilePivot = (Transform)EditorGUILayout.ObjectField("Projectile Pivot", character.projectilePivot, typeof(Transform), true);
        character.healthBarPrefab = (GameObject)EditorGUILayout.ObjectField("Health Bar Prefab", character.healthBarPrefab, typeof(GameObject), true);
        character.selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Selected Prefab", character.selectedPrefab, typeof(GameObject), true);
        character.healthBarOffset = EditorGUILayout.Vector3Field("Health Bar Offset", character.healthBarOffset); // Добавляем смещение

        string[] classOptions = new string[] { "Knight", "Archer", "Mage", "Player" };
        int index;

        if (character.isKnight) index = 0;
        else if (character.isArcher) index = 1;
        else if (character.isMage) index = 2;
        else if (character.isPlayer) index = 3;
        else index = 0;

        index = EditorGUILayout.Popup("Character Class", index, classOptions);
        character.SetClass(classOptions[index]);

        if (character.isMage)
        {
            character.MaxMana = EditorGUILayout.IntField("Max Mana", character.MaxMana);
            character.CurrentMana = EditorGUILayout.IntField("Current Mana", character.CurrentMana);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(character); // Помечаем объект как изменённый
        }
    }
}
