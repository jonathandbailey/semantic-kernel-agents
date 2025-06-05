using Microsoft.SemanticKernel;
using Todo.Application.Settings;

namespace Todo.Application.Agents;

public class AgentConfiguration
{
    public PromptTemplateConfig Template { get; init; } = new();

    public AgentSettings Settings { get; init; } = new();

    public IPromptTemplateFactory PromptTemplateFactory { get; set; } = new KernelPromptTemplateFactory();
}