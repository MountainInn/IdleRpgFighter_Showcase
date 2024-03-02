using UnityEngine;

public class MobView : MonoBehaviour
{
    [SerializeField] ProgressBar healthBar;
    [SerializeField] ProgressBar attackTimerBar;


    public void Subscribe(Mob mob)
    {
        healthBar.Subscribe(mob.gameObject, mob.health);
        attackTimerBar.Subscribe(mob.gameObject, mob.attackTimer);
    }
}
