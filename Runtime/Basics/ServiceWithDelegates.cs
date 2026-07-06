using System;

namespace Arunoki.Flow.Basics
{
    public class ServiceWithDelegates : IInitializable, IStartable, IService, IResettable
    {
        private int initStep = -1;
        private int startStep = -1;
        private int activeStep = -1;

        private bool CanInit() => initStep < 0;

        private bool CanStart() => startStep < 0;

        private bool CanActivate() => activeStep < 0;

        private bool CanDeactivate() => activeStep > 0;

        public Action OnActivate = delegate { };
        public Action OnDeactivate = delegate { };
        public Action OnReset = delegate { };
        public Action OnStart = delegate { };
        public Action OnInit = delegate { };

        protected internal bool IsInitialized() => initStep > 0;

        public bool IsActive() => activeStep > 0;

        bool IStartable.IsStarted() => startStep > 0;

        bool IInitializable.IsInitialized() => IsInitialized();

        public void Initialize()
        {
            if (CanInit())
            {
                initStep = 0;
                OnInit?.Invoke();
                initStep = 1;
            }
        }

        public void Start()
        {
            Initialize();
            Activate();

            if (CanStart())
            {
                startStep = 0;
                OnStart?.Invoke();
                startStep = 1;
            }
        }

        public void Activate()
        {
            Initialize();
            if (CanActivate())
            {
                activeStep = 0;
                OnActivate?.Invoke();
                activeStep = 1;
            }
        }

        public void Deactivate()
        {
            if (CanDeactivate())
            {
                activeStep = 0;
                OnDeactivate?.Invoke();
                activeStep = 1;
            }
        }

        public void Reset()
        {
            Deactivate();

            startStep = -1;
            OnReset?.Invoke();
        }

        public virtual bool AutoReset() => true;

        public virtual void Dispose()
        {
            OnInit = null;
            OnActivate = null;
            OnDeactivate = null;
            OnReset = null;
            OnStart = null;
        }
    }
}
