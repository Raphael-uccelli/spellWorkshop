using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "SpellWorkshop/Spell")]
public class SpellData : ScriptableObject
{
    public string spellName = "New Spell";
    public Element element = Element.None;
    public int damage = 10;
    public float projectileSpeed = 10f;
    public bool isPiercing = false;
    public bool hasExplosion = false;
    public float explosionRadius = 3f;
    public bool hasBounce = false;
    public int maxBounces = 3;
}