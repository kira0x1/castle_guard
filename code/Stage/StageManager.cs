namespace Kira;

using System;
using System.Threading.Tasks;

[Category("_Kira")]
public class StageManager : Component
{
    public Action OnNavGenerated;
    public Action OnNavGenerationBegin;

    public static StageManager Instance { get; set; }

    protected override void OnAwake()
    {
        Instance = this;
    }

    public void GenerateNav()
    {
        OnNavGenerationBegin?.Invoke();
        Scene.NavMesh.Generate(Scene.PhysicsWorld).ContinueWith(OnNavDone);
    }

    private void OnNavDone(Task<bool> res)
    {
        OnNavGenerated?.Invoke();
    }
}