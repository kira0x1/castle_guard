namespace Kira;

[Category("_Kira")]
public class EnemyStageSystem : GameObjectSystem
{
    private StageManager stageManager;
    private List<EnemyController> enemies = new List<EnemyController>();

    public EnemyStageSystem(Scene scene) : base(scene)
    {
        Listen(Stage.SceneLoaded, 10, OnSceneLoaded, "SceneLoaded");
    }

    private void OnSceneLoaded()
    {
        stageManager = Scene.GetAllComponents<StageManager>().FirstOrDefault();
        enemies = Scene.GetAllComponents<EnemyController>().ToList();

        if (!stageManager.IsValid())
        {
            Log.Warning("StageManager not found!");
            return;
        }

        foreach (EnemyController enemyController in enemies)
        {
            // Remove listener on enemy death?
            stageManager.OnNavGenerationBegin += enemyController.OnNavGenerationBegin;
            stageManager.OnNavGenerated += enemyController.OnNavGenerated;
        }
    }
}