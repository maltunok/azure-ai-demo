using Azure.AI.OpenAI;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;

namespace StoryCreator.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Question { get; set; } = String.Empty;
        public string ResponseContent { get; set; } = String.Empty;

        public void OnPost()
        {
            // Configure OpenAI client
            string openAIEndpoint = "https://your-openai-service-name.openai.azure.com/";
            string openAIKey = "your-openai-service-key";
            string openAIDeploymentName = "dyour-openai-deployment-name";

            OpenAIClient client = new(new Uri(openAIEndpoint), new AzureKeyCredential(openAIKey));

            // Configure search service
            string searchEndpoint = "https://your-azure-search-service-name.search.windows.net";
            string searchKey = "your-azure-search-service-key";
            string searchIndex = "your-serach-index-name";

            AzureCognitiveSearchChatExtensionConfiguration demoConfig = new()
            {
                SearchEndpoint = new Uri(searchEndpoint),
                Authentication = new OnYourDataApiKeyAuthenticationOptions(searchKey),
                IndexName = searchIndex
            };

            // Set up the AI chat query/completion
            ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages = { new ChatRequestUserMessage(Question) },
                AzureExtensionsOptions = new AzureChatExtensionsOptions
                {
                    Extensions = { demoConfig }
                },
                DeploymentName = openAIDeploymentName
            };

            // Send request to Azure OpenAI model
            ChatCompletions chatCompletionsResponse = client.GetChatCompletions(chatCompletionsOptions);

            ResponseContent = chatCompletionsResponse.Choices[0].Message.Content;
        }
    }
}