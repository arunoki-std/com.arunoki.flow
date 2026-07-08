using System;
using System.Collections.Generic;
using Arunoki.Flow.Collections.Utilities;
using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
    /// <summary>
    /// </summary>
    ///
    /// <typeparam name="TContext"></typeparam>
    public partial class StateMachine<TContext>
    {
        internal List<List<StateNode<TContext>>> Ancestors { get; } = new(8);
        internal Dictionary<Type, StateNode<TContext>> NodesCache { get; } = new(16);
        internal List<StateNode<TContext>> Nodes { get; } = new(16);

        private StateNode<TContext> currentRoot;
        private bool nodesReady;

        protected virtual void TrySetupNodes()
        {
            if (nodesReady)
                return;
            nodesReady = true;

            var parents = new List<StateNode<TContext>>(NodesCache.Count);
            var children = new List<StateNode<TContext>>(NodesCache.Count);

            foreach (var node in Nodes)
            {
                if (node.State.IsSubstate())
                    children.Add(node);
                else
                    parents.Add(node);
            }

            foreach (var childNode in children)
            {
                childNode.State.IsSubstateOf(out var parentType);

                bool parentFound = false;

                foreach (var parentNode in parents)
                {
                    if (parentNode.IsTypeParent(parentType))
                    {
                        parentNode.AddChild(childNode, childNode.State.IsDefault());
                        parentFound = true;
                        break;
                    }
                }

                if (!parentFound)
                    throw new StateMachineException($"Parent for state '{childNode}' not found.");
            }

            foreach (var node in parents)
            {
                if (node.IsRoot())
                    node.SetSiblings(0, Ancestors);
            }

            foreach (var node in Nodes)
                if (node.State is IInitializable state && !state.IsInitialized())
                    state.Initialize();
        }

        public void AddState<TState>()
            where TState : IState<TContext>, new()
        {
            CreateNode(typeof(TState));
        }

        public void AddState(IState<TContext> state)
        {
            CreateNode(state.GetType(), state);
        }

        public void AddStatesFrom(object stateSource)
        {
            if (stateSource == null)
                throw new ArgumentNullException(nameof(stateSource));
            if (stateSource is IDummy)
                return;

            foreach (Type stateType in stateSource.GetType().GetNestedTypes<IState<TContext>>())
                CreateNode(stateType);
        }

        private void CreateNode(Type stateType)
        {
            if (NodesCache.ContainsKey(stateType))
                return;

            CreateNode(stateType, CreateState(stateType));
        }

        private void CreateNode(Type stateType, IState<TContext> state)
        {
            var node = new StateNode<TContext>(stateType.Name, state);
            NodesCache.Add(stateType, node);
            Nodes.Add(node);
        }

        private IState<TContext> CreateState(Type stateType)
        {
            IState<TContext> state;
            try
            {
                state = (IState<TContext>)Activator.CreateInstance(stateType);
            }
            catch (InvalidCastException)
            {
                throw new StateMachineException(
                    $"Can't create state '{stateType}'. Class doesn't implement '{nameof(IState<TContext>)}'."
                );
            }

            if (Guard.IsNull(state.Context))
                state.Context = Context;
            return state;
        }

        private bool TryGetNode(Type stateType, out StateNode<TContext> node)
        {
            for (var siblingIndex = 0; siblingIndex < Ancestors.Count; siblingIndex++)
            {
                var list = Ancestors[siblingIndex];
                for (var i = 0; i < list.Count; i++)
                {
                    node = list[i];

                    if (IsAssignableOrEquals(node.State.GetType(), stateType))
                        return true;
                }
            }

            node = null;
            return false;
        }

        /// First default state without parent would be defined as root state.
        private StateNode<TContext> GetDefaultRoot()
        {
            foreach (var node in Nodes)
            {
                if (node.IsRoot() && node.State.IsDefault())
                    return node;
            }

            throw StateMachineException.RootIsNotDefined(this, "Default root state not found.");
        }

        protected static bool IsAssignableOrEquals(Type stateType, Type typeOrInterface) =>
            ReferenceEquals(stateType, typeOrInterface)
            || typeOrInterface.IsAssignableFrom(stateType);
    }
}
