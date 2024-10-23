namespace Kira;

using System;
using System.Threading.Tasks;

[Category("_Kira")]
public class StageManager : Component
{
    public Action OnNavGenerated;
    public static StageManager Instance { get; set; }

    protected override void OnAwake()
    {
        Instance = this;
    }

    public void GenerateNav()
    {
        Scene.NavMesh.Generate(Scene.PhysicsWorld).ContinueWith(OnNavDone);
    }

    private void OnNavDone(Task<bool> res)
    {
        Log.Info("nav done");
        Log.Info(res.Status);
        OnNavGenerated?.Invoke();
    }
}