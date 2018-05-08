using System;
using System.Configuration;
using System.Collections.Generic;
using AsPartitionProcessing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace fAsPartitionProcessing
{
    public static class fAsPartitionProcessingTimer
    {

        private static string _modelConfigurationIDs;

        [FunctionName("fAsPartitionProcessingTimer")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function (fAsPartitionProcessingTimer) started at: {DateTime.Now}");

            try
            {
                /* read from ASPP_ConfigurationLoggingDB */
                List<ModelConfiguration> modelsConfig = InitializeFromDatabase();

                /* loop through Model Config */
                foreach (ModelConfiguration modelConfig in modelsConfig)
                {
                    /* grab user/pw for AzureAS authentication */
                    String azure_AppID = System.Environment.GetEnvironmentVariable("CUSTOMCONNSTR_AzureAS_SvcPrincipal_AppID");
                    String azure_AppKey = System.Environment.GetEnvironmentVariable("CUSTOMCONNSTR_AzureAS_SvcPrincipal_AppKey");

                    /* apparently you can do it this way as well */
                    modelConfig.UserName = azure_AppID;
                    modelConfig.Password = azure_AppKey;

                    /* perform processing */
                    PartitionProcessor.PerformProcessing(modelConfig, ConfigDatabaseHelper.LogMessage);
                }
            }
            catch (Exception e)
            {
                log.Info($"C# Timer trigger function (fAsPartitionProcessingTimer) exception: {e.ToString()}");
            }

            log.Info($"C# Timer trigger function (fAsPartitionProcessingTimer) finished at: {DateTime.Now}");

        }
        public static List<ModelConfiguration> InitializeFromDatabase()
        {
            ConfigDatabaseConnectionInfo connectionInfo = new ConfigDatabaseConnectionInfo();

            connectionInfo.Server = System.Environment.GetEnvironmentVariable("CUSTOMCONNSTR_connstr_ASPP_ConfigurationLogging_SERVER");
            connectionInfo.Database = System.Environment.GetEnvironmentVariable("CUSTOMCONNSTR_connstr_ASPP_ConfigurationLogging_DB");
            connectionInfo.UserName = System.Environment.GetEnvironmentVariable("CUSTOMCONNSTR_connstr_ASPP_ConfigurationLogging_USER");
            connectionInfo.Password = System.Environment.GetEnvironmentVariable("CUSTOMCONNSTR_connstr_ASPP_ConfigurationLogging_PW");

            return ConfigDatabaseHelper.ReadConfig(connectionInfo, _modelConfigurationIDs);
        }
    }
}