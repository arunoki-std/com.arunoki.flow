using UnityEngine;

namespace Arunoki.Flow.Sample
{
    [DisallowMultipleComponent]
    public sealed class FsmTestController : MonoBehaviour
    {
        private FlowHub hub;

        private FsmEntity entity;

        private void Awake()
        {
            entity = new FsmEntity();
            hub = new FlowHub(entity);
        }

        private void OnEnable()
        {
            UnityEngine.Debug.Log($"\n\n");
            UnityEngine.Debug.Log($"\t ACTIVATE ({nameof(FsmTestController)})");

            hub.Activate();
        }

        private void OnDisable()
        {
            UnityEngine.Debug.Log($"\t DEACTIVATE ({nameof(FsmTestController)})");
            hub.Deactivate();
        }

        private void Update()
        {
            entity.Update();
        }
    }
}
