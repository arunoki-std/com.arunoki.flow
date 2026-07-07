using System;
using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;
using Arunoki.Flow.Utilities;

namespace Arunoki.Flow.Builders
{
    public class PipelineContainer : HubContainer<IFlowPipeline>
    {
        protected virtual HandlersContainer Handlers => Hub.Handlers;

        public void Register<TPipeline>()
            where TPipeline : IFlowPipeline, new()
        {
            Register(Activator.CreateInstance(typeof(TPipeline)) as IFlowPipeline);
        }

        public void Remove<TPipeline>()
            where TPipeline : IFlowPipeline
        {
            Remove(typeof(TPipeline));
        }

        public void Remove(Type pipelineType)
        {
            foreach (IFlowPipeline pipeline in this)
            {
                if (pipeline.GetType() == pipelineType)
                {
                    Remove(pipeline);
                    break;
                }
            }
        }

        protected virtual void CreateHandlers(Type pipelineType, IFlowContext context)
        {
            var set = Handlers.KeySet.GetOrCreate(pipelineType);
            var handlerTypes = pipelineType.GetNestedTypes<IFlowHandler>();

            for (var i = 0; i < handlerTypes.Count; i++)
            {
                var handler = (IFlowHandler)Activator.CreateInstance(handlerTypes[i]);
                if (handler is IContextPart part && part.Get() == null)
                    part.Set(context);
                set.TryAdd(handler);
            }
        }

        protected override void OnElementAdded(IFlowPipeline pipeline)
        {
            base.OnElementAdded(pipeline);

            var context =
                pipeline as IFlowContext
                ?? (pipeline is IContextPart p && p.Get() != null ? p.Get() : Context);

            CreateHandlers(pipeline.GetType(), context);
        }

        protected override void OnElementRemoved(IFlowPipeline pipeline)
        {
            base.OnElementRemoved(pipeline);

            if (pipeline is IContextPart cxtPart)
                cxtPart.Set(null);
            if (pipeline is IHubPart hubPart)
                hubPart.Set(null);

            Handlers.KeySet.Clear(pipeline.GetType());
        }

        protected override void OnInit()
        {
            base.OnInit();

            var list = GetAllElements();
            for (var index = 0; index < list.Count; index++)
                if (list[index] is IInitializable pipeline && !pipeline.IsInitialized())
                    pipeline.Initialize();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var list = GetAllElements();
            for (var index = 0; index < list.Count; index++)
                if (list[index] is IActiveFlowPipeline pipeline)
                    pipeline.OnActivated();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            var list = GetAllElements();
            for (var index = 0; index < list.Count; index++)
                if (list[index] is IActiveFlowPipeline pipeline)
                    pipeline.OnDeactivated();
        }

        protected override bool IsMultiInstancesSupported() => !Flow.Utilities.Utils.IsDebug();

        public override int GetBuildOrder() => (int)FlowHub.BuildOrder.Pipelines;
    }
}
