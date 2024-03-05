using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/MobQueue",fileName ="MobQueue")]
public class MobQueue : ScriptableObject
{
    public List<QueueSegment> queueSegments;
    public float powerMultiplier = 1f;

    public IEnumerable<MobStatsSO> Next()
    {
        foreach (var segment in queueSegments)
        {
            for (int m = 0; m < segment.segmentLength; m++)
            {
                MobStatsSO so = segment.mobStats.GetRandom();

                MobStatsSO nextMobStats = Instantiate(so);

                nextMobStats.Multiply(segment.powerMultiplier * powerMultiplier);

                yield return nextMobStats;
            }
        }
    }

    [System.SerializableAttribute]
    public class QueueSegment
    {
        public List<MobStatsSO> mobStats;
        public int segmentLength;
        public float powerMultiplier = 1f;
    }
}
