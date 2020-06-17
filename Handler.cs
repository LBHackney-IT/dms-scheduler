using Amazon.DatabaseMigrationService;
using Amazon.DatabaseMigrationService.Model;
using Amazon.Lambda.Core;
using System;
using System.Net;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DMSScheduler
{
    public class Handler
    {
        public void RestartDMSTask(Request request, ILambdaContext context)
        {
            try
            {
                LambdaLogger.Log($"Scheduled restart of replication task with ID {request.replicationTaskId}" +
                    $" in account {request.accountId} begins...");
                var client = new AmazonDatabaseMigrationServiceClient();
                var response = client.StartReplicationTaskAsync(new StartReplicationTaskRequest
                {
                    ReplicationTaskArn = $"arn:aws:dms:eu-west-2:{request.accountId}:task:{request.replicationTaskId}",
                    StartReplicationTaskType = StartReplicationTaskTypeValue.ReloadTarget
                });

                if (response.Result.HttpStatusCode != HttpStatusCode.OK)
                {
                    LambdaLogger.Log($"Request to restart task with ID {request.replicationTaskId} in account with ID {request.accountId} was not successful");
                    throw new AmazonDatabaseMigrationServiceException($"Request to restart replication has has failed with" +
                        $" status code {response.Result.HttpStatusCode}");
                }
            }
            catch(Exception ex)
            {
                LambdaLogger.Log($"An exception has occurred - {ex.Message} {ex.InnerException} at {ex.StackTrace}");
                throw ex;
            }
        }
    }

    public class Request
    {
        public string replicationTaskId {get; set;}
        public string accountId { get; set;}
    }
}