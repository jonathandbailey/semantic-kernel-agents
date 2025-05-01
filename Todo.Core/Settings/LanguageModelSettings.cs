namespace Todo.Core.Settings;

public class LanguageModelSettings
{
    public required List<AzureOpenAiSettings> AzureOpenAiSettings { get; set; }
}