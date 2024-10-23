namespace Kira;

[Category("_Kira")]
public class EnemyManager : Component
{
    private StageManager stageManager;
    private List<EnemyController> enemies = new List<EnemyController>();

    protected override void OnAwake()
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
            stageManager.OnNavGenerated += enemyController.OnNavGenerated;
        }
    }
}