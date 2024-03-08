using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

[CreateAssetMenu(fileName = "WeakPoints", menuName = "SO/Talents/WeakPoints")]
public class WeakPoints : Talent
{
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


    [Inject]
    public void Construct(CharacterController characterController,
                          Character character,
                          Mob mob,
                          WeakPointView.Pool weakPointViewPool)
    {
        UnityEngine.UI.Button attackButton = characterController.AttackButton;

        character.onAttackAnimEvent
            .AddListener(() =>
            {
                if (UnityEngine.Random.value < chanceToAppearAfterAttack)
                {
                    SpawnWeakPoint(attackButton, weakPointViewPool);
                }
            });

        weakPointViewPool.onWeakPointClicked = () =>
        {
            Shoot(mob, character);

            attackButton.onClick.Invoke();
        };
    }

    void Shoot(Mob mob, Character character)
    {
        float damage = character.Stats.attackDamage * damageMult;

        character.InflictDamage(mob, damage);
    }

    static void SpawnWeakPoint(UnityEngine.UI.Button attackButton, WeakPointView.Pool weakPointViewPool)
    {
        WeakPointView view = weakPointViewPool.Spawn();

        RectTransform rectTransform = attackButton.GetComponent<RectTransform>();
        Rect rect = rectTransform.rect;

        Vector3 canvasScale = Vector3.one * 0.4f;

        Vector3 halfSize = rect.size / 2;
        halfSize.Scale(canvasScale);

        Vector3 position = UnityEngine.Random.insideUnitCircle.xy_(5);
        position.Scale(halfSize * 0.8f);

        view.transform.position = position + halfSize;
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
