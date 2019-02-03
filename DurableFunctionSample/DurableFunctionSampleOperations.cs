using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DurableFunctionSample
{
    public static class DurableFunctionSampleOperations
    {
        /// <summary>
        /// Orchestration�֐�
        /// </summary>
        [FunctionName("DurableFunctionSampleOperations")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("HelloFunction", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("HelloFunction", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("HelloFunction", "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        /// <summary>
        /// Hello�֐� (���������s���֐�)
        /// </summary>
        [FunctionName("HelloFunction")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"[�������s] Saying hello to {name}.");
            return $"Hello {name}!";
        }

        /// <summary>
        /// �֐��̃G���h�|�C���g
        /// </summary>
        [FunctionName("start")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Orchestrator�֐��̊J�n
            var instanceId = await starter.StartNewAsync("DurableFunctionSampleOperations", null);
            log.LogInformation($"[�����J�n] Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}