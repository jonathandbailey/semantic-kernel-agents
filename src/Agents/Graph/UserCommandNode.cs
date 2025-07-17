using Todo.Core.Users;

namespace Agents.Graph
{
    public class UserCommandNode(string name, IUserMessageSender userMessageSender) : INode
    {
        public async Task<NodeState> InvokeAsync(NodeState state)
        {
            await userMessageSender.CommandAsync(new AssistantCommand
            {
                Name = "VacationPlanUpdate", VacationPlanId = state.VacationPlanId
            });
            
            return state;
        }

        public string Name { get; } = name;
        public Guid Id { get; } = Guid.NewGuid();
    }
}
