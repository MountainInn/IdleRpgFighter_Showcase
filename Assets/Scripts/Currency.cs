using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "Currency", menuName = "SO/Currency")]
public class Currency : ScriptableObject
{
    public Sprite sprite;
    public ReactiveProperty<int> amount;
}
