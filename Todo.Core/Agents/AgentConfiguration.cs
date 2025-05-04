using Microsoft.SemanticKernel;
using Todo.Core.Settings;

namespace Todo.Core.Agents;

public class AgentConfiguration
{
    public PromptTemplateConfig Template { get; init; } = new();

    public AgentSettings Settings { get; init; } = new();

    public IPromptTemplateFactory PromptTemplateFactory { get; set; } = new KernelPromptTemplateFactory();
}