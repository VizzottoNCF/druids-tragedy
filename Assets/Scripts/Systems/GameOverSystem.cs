using UnityEngine;
public class GameOverSystem : Singleton<GameOverSystem>
{
    [SerializeField] private GameObject gameOverPanel;
    public bool IsGameOver { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        gameOverPanel.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (IsGameOver) return; // hero can "die" several times in one multi-hit attack
        IsGameOver = true;

        ActionSystem.Instance.Halt();
        gameOverPanel.SetActive(true);
    }
}