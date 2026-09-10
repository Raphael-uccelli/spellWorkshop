using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "SpellWorkshop/Spell")]
public class SpellData : ScriptableObject
{
    public string spellName = "New Spell";
    public int damage = 10;
    public float projectileSpeed = 10f;
    public bool isPiercing = false;
}