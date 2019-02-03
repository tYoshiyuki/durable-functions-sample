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
        /// Orchestration関数
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
        /// Hello関数 (実処理を行う関数)
        /// </summary>
        [FunctionName("HelloFunction")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"[処理実行] Saying hello to {name}.");
            return $"Hello {name}!";
        }

        /// <summary>
        /// 関数のエンドポイント
        /// </summary>
        [FunctionName("start")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Orchestrator関数の開始
            var instanceId = await starter.StartNewAsync("DurableFunctionSampleOperations", null);
            log.LogInformation($"[処理開始] Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}