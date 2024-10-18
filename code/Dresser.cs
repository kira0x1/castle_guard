namespace Kira;

public sealed class Dresser : Component, Component.ExecuteInEditor
{
    [Property] public SkinnedModelRenderer Target { get; set; }
    [Property] public List<Clothing> Clothes { get; set; } = new List<Clothing>();

    [Property, Range(0, 1)]
    public float HairTint { get; set; } = 0f;

    private bool isDirty { get; set; }
    private ClothingContainer container;

    protected override void OnValidate()
    {
        isDirty = true;
    }

    protected override void OnUpdate()
    {
        if (!isDirty) return;

        isDirty = false;
        UpdateClothing();
    }

    public void AddEntry(Clothing c)
    {
        if (c == null) return;

        foreach (var clothingEntry in container.Clothing)
        {
            if (!c.CanBeWornWith(clothingEntry.Clothing))
            {
                Log.Warning($"Conflicted clothes \"{c.Title}\" and \"{clothingEntry.Clothing.Title}\"");
                return;
            }
        }

        if (c.Category == Clothing.ClothingCategory.Hair || c.Category == Clothing.ClothingCategory.Facial)
        {
            c.TintDefault = HairTint;
        }

        ClothingContainer.ClothingEntry ce = new ClothingContainer.ClothingEntry(c);
        container.Clothing.Add(ce);
    }

    public void UpdateClothing()
    {
        container = new ClothingContainer();

        foreach (Clothing c in Clothes)
        {
            AddEntry(c);
        }

        container.Apply(Target);

        foreach ((string name, int value) in container.GetBodyGroups())
        {
            Target.SetBodyGroup(name, value);
        }
    }
}