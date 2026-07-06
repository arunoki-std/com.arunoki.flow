using System;

namespace Arunoki.Flow
{
    public partial class FlowHub
    {
        protected override void OnInit()
        {
            Contexts.Set.TryAdd(Contexts.Root);

            base.OnInit();
        }

        public override void Reset()
        {
            Events.Reset();

            base.Reset();
        }

        public bool Register(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var result = false;

            for (var i = 0; i < Containers.Count; i++)
            {
                var container = Containers[i];
                result = (container.IsConsumable(entity) && container.Register(entity)) || result;
            }

            return result;
        }

        public virtual void Remove(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            for (var i = 0; i < Containers.Count; i++)
            {
                var container = Containers[i];
                if (container.IsConsumable(entity))
                    container.Remove(entity);
            }
        }

        public virtual void RemoveAll()
        {
            Events.ClearAll();

            for (var i = 0; i < Containers.Count; i++)
                Containers[i].RemoveAll();
        }

        public bool TryFind<T>(out T container)
            where T : IHubContainer
        {
            for (var i = 0; i < Containers.Count; i++)
            {
                if (Containers[i] is T element)
                {
                    container = element;
                    return true;
                }
            }

            container = default;
            return false;
        }

        public bool IsConsumable(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            for (var i = 0; i < Containers.Count; i++)
                if (Containers[i].IsConsumable(entity))
                    return true;

            return false;
        }
    }
}
