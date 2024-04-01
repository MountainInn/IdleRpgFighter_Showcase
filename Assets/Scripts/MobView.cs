using UnityEngine;

public class MobView : MonoBehaviour
{
    [SerializeField] protected ProgressBar healthBar;
    [SerializeField] protected ProgressBar attackTimerBar;
    [SerializeField] protected ProgressBar energyBar;

    public void Subscribe(Rock rock)
    {
        healthBar.Subscribe(rock.gameObject, rock.health);
        // attackTimerBar.Subscribe(rock.gameObject, rock.attackTimer);
    }

    public void SubscribeGulagTimer(Gulag gulag, Volume timer)
    {
        attackTimerBar.Subscribe(gulag.gameObject, timer);
    }

    public void Subscribe(Mob mob)
    {
        healthBar.Subscribe(mob.gameObject, mob.health);
        energyBar.Subscribe(mob.gameObject, mob.energy);
    }
}
