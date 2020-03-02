﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Extensions.Logging;


namespace AzureFaultInjector
{
    class FINSG : FI
    {

        static string myNewRuleNameIn = "FuzzerBlockingRuleIn";
        static string myNewRuleNameOut = "FuzzerBlockingRuleOut";
        static int myNewRulePriority = 100;


        public FINSG (ILogger iLog, Microsoft.Azure.Management.Fluent.IAzure iAzure, string iTarget) : base(iLog,iAzure, iTarget)
        {
            myResource = iAzure.NetworkSecurityGroups.GetById(iTarget);
            
            myTargetType = "NSG";
        }


        protected override bool turnOn()
        {
            Microsoft.Azure.Management.Network.Fluent.INetworkSecurityGroup curNSG = (Microsoft.Azure.Management.Network.Fluent.INetworkSecurityGroup)myResource;

            try
            {
                log.LogInformation($"Removing new highest priority blocking rule to NSG {curNSG.Name}: In & Out");


                curNSG.Update().WithoutRule(myNewRuleNameIn).Apply();
                curNSG.Update().WithoutRule(myNewRuleNameOut).Apply();
                return true;
            }
            catch (Exception err)
            {
                log.LogError($"Error unblocking NSG {curSubName} -> {curTarget} -> {curNSG.Name}: {err}");
                return false;
            }

        }

        protected override bool turnOff()
        {
            Microsoft.Azure.Management.Network.Fluent.INetworkSecurityGroup curNSG = (Microsoft.Azure.Management.Network.Fluent.INetworkSecurityGroup)myResource;

            try
            {
                // Add a blocking rule
                log.LogInformation($"Adding new highest priority blocking rule to NSG {curNSG.Name}: Inbound");
                curNSG.Update().DefineRule(myNewRuleNameIn)
                    .DenyInbound()
                    .FromAnyAddress()
                    .FromAnyPort()
                    .ToAnyAddress()
                    .ToAnyPort()
                    .WithAnyProtocol()
                    .WithDescription("Temporary stop everything rule from Azure Fuzzer to simulate a cut network")
                    .WithPriority(100)
                    .Attach()
                    .Apply();
                log.LogInformation($"Adding new highest priority blocking rule to NSG {curNSG.Name}: Outbound");
                curNSG.Update().DefineRule(myNewRuleNameOut)
                    .DenyOutbound()
                    .FromAnyAddress()
                    .FromAnyPort()
                    .ToAnyAddress()
                    .ToAnyPort()
                    .WithAnyProtocol()
                    .WithDescription("Temporary stop everything rule from Azure Fuzzer to simulate a cut network")
                    .WithPriority(100)
                    .Attach()
                    .Apply();
                return true;
            }
            catch (Exception err)
            {
                log.LogError($"Error blocking NSG {curSubName} -> {curTarget} -> {curNSG.Name}: {err}");
                return false;
            }
        }

    }
}