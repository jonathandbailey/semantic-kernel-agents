namespace Agents.Graph.v2
{
    public abstract class BaseEdge
    {
        public abstract Guid Id { get; }

        public abstract INode Target { get; }

        public abstract bool CanRoute(NodeState nodeState);
    }
}
