using System.Collections.Generic;

namespace Arunoki.Flow.Basics
{
    public class ServiceWithElements<TElement> : BaseService
        where TElement : class
    {
        protected internal readonly List<TElement> Elements;

        public ServiceWithElements()
            : this(new(8)) { }

        public ServiceWithElements(List<TElement> elements)
        {
            Elements = elements;
        }

        protected override void OnInit()
        {
            base.OnInit();

            for (var i = Elements.Count - 1; i >= 0; i--)
                if (Elements[i] is IInitializable initializer && !initializer.IsInitialized())
                    initializer.Initialize();
        }

        public override void Reset()
        {
            base.Reset();

            for (var i = Elements.Count - 1; i >= 0; i--)
                if (Elements[i] is IResettable resettable && resettable.AutoReset())
                    resettable.Reset();
        }

        protected override void OnStarted()
        {
            base.OnStarted();

            for (var i = Elements.Count - 1; i >= 0; i--)
                if (Elements[i] is IStartable starter && !starter.IsStarted())
                    starter.Start();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            for (var i = Elements.Count - 1; i >= 0; i--)
                if (
                    Elements[i] is IService service
                    && !service.IsActive()
                    && service is not IManualService
                )
                    service.Activate();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            for (var i = Elements.Count - 1; i >= 0; i--)
                if (
                    Elements[i] is IService service
                    && service.IsActive()
                    && service is not IManualService
                )
                    service.Deactivate();
        }
    }
}
