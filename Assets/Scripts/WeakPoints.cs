using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "WeakPoints", menuName = "SO/Talents/WeakPoints")]
public class WeakPoints : Talent
{
    [SerializeField] float lifespan = 2;
    [SerializeField] List<Field> fields;

    [Serializable]
    public struct Field
    {
        public float chanceToAppearAfterClick;
        public float damageMult;
        public int cost;
    }

    public float chanceToAppearAfterAttack {get; protected set;}
    public float damageMult {get; protected set;}

    [Inject] Canvas canvas;
    [Inject]
    public void Construct(Character character,
                          Mob mob,
                          WeakPointView.Pool weakPointViewPool)
    {
        character.onAttackAnimEvent
            .AddListener(() =>
            {
                if (UnityEngine.Random.value < chanceToAppearAfterAttack)
                {
                    SpawnWeakPoint(canvas, weakPointViewPool);
                }
            });

        weakPointViewPool.onWeakPointClicked = () =>
        {
            Shoot(mob, character);
        };
    }

    void Shoot(Mob mob, Character character)
    {
        float damage = character.Stats.attackDamage * damageMult;

        character.InflictDamage(mob, damage);
    }

    void SpawnWeakPoint(Canvas canvas, WeakPointView.Pool weakPointViewPool)
    {
        WeakPointView view = weakPointViewPool.Spawn();

        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        Rect rect = rectTransform.rect;

        Vector3 canvasScale = Vector3.one * 0.4f;

        Vector3 halfSize = rect.size / 2;
        halfSize.Scale(canvasScale);

        Vector3 position = UnityEngine.Random.insideUnitCircle.xy_(5);
        position.Scale(halfSize * 0.8f);

        view.transform.position = position + halfSize;

        UniTask
            .WaitForSeconds(lifespan)
            .ContinueWith(() =>
            {
                if (view.gameObject.activeSelf)
                    weakPointViewPool.DisableButtonAndDespawn(view);
            })
            .Forget();
    }

    protected override void OnLevelUp(int level, Price price)
    {
        price.cost.Value = fields[level].cost;

        chanceToAppearAfterAttack = fields[level].chanceToAppearAfterClick;
        damageMult = fields[level].damageMult;
    }

    public override IObservable<string> ObserveDescription()
    {
        return
            Observable.Return("*BLANK*");
    }
}
