using System.Collections.Generic;
using Arunoki.Flow.Basics;

namespace Arunoki.Flow.Builders
{
    public class UpdatableContainer : HubContainer<IUpdatable>
    {
        protected readonly List<IUpdatable> Ordinary = new(16);
        protected readonly List<IUpdatable> Fixed = new(16);
        protected readonly List<IUpdatable> Late = new(16);

        public virtual void Update() => Update(Ordinary);

        public virtual void LateUpdate() => Update(Late);

        public virtual void FixedUpdate() => Update(Fixed);

        protected static void Update(List<IUpdatable> list)
        {
            for (var index = list.Count - 1; index >= 0; index--)
                list[index].Update();
        }

        protected override void OnElementAdded(IUpdatable element)
        {
            base.OnElementAdded(element);

            switch (element)
            {
                case IFixedUpdatable a:
                    Fixed.Add(a);
                    break;
                case ILateUpdatable b:
                    Late.Add(b);
                    break;
                default:
                    Ordinary.Add(element);
                    break;
            }
        }

        protected override void OnElementRemoved(IUpdatable element)
        {
            base.OnElementRemoved(element);

            switch (element)
            {
                case IFixedUpdatable a:
                    Fixed.Remove(a);
                    break;
                case ILateUpdatable b:
                    Late.Remove(b);
                    break;
                default:
                    Ordinary.Remove(element);
                    break;
            }
        }
    }
}
