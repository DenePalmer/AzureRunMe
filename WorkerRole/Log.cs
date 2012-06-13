﻿ #region Copyright (c) 2010 - 2012 Active Web Solutions Ltd
//
// (C) Copyright 2010 - 2012 Active Web Solutions Ltd
//      All rights reserved.
//
// This software is provided "as is" without warranty of any kind,
// express or implied, including but not limited to warranties as to
// quality and fitness for a particular purpose. Active Web Solutions Ltd
// does not support the Software, nor does it warrant that the Software
// will meet your requirements or that the operation of the Software will
// be uninterrupted or error free or that any defects will be
// corrected. Nothing in this statement is intended to limit or exclude
// any liability for personal injury or death caused by the negligence of
// Active Web Solutions Ltd, its employees, contractors or agents.
//
#endregion

using System;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WorkerRole
{
    public class Log
    {
        CloudTableClient cloudTableClient;

        public Log(string connectionString)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            cloudTableClient = account.CreateCloudTableClient();
        }

        public void WriteEntry(string eventName, string notes, string label)
        {
            try
            {
                string tableName = RoleEnvironment.GetConfigurationSettingValue("LogTableName");

                cloudTableClient.CreateTableIfNotExist(tableName);
                
                TableServiceContext dataContext = cloudTableClient.GetDataServiceContext();
                dataContext.AddObject(tableName, new LogEntry(eventName, notes, label));
                dataContext.SaveChangesWithRetries();
            }
            catch (Exception e)
            {
                Tracer.WriteLine(e.ToString(), "Critical");
            }
        }
    }
}
