#region Copyright Quipment
//-----------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Jsinh.in">
//     Copyright (c) 2013 Jsinh.in. All rights reserved.
// </copyright>
// <summary>
//      Sample program to demonstrate SqlDependency for SQL Server using C#.
// </summary>
//-----------------------------------------------------------------------------------------------------------------------------------------------------
#endregion

namespace SqlDependencyNotificationSample
{
    #region Namespace

    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    #endregion

    /// <summary>
    /// Sample program.
    /// </summary>
    public class Program : IDisposable
    {
        #region Variable declaration

        /// <summary>
        /// Option selected.
        /// </summary>
        private static string optionSelected;

        /// <summary>
        /// Sample connection string.
        /// </summary>
        private readonly string sampleConnectionString;

        /// <summary>
        /// Notification query.
        /// </summary>
        private readonly string notificationQuery;

        /// <summary>
        /// Notification stored procedure.
        /// </summary>
        private readonly string notificationStoredProcedure;

        /// <summary>
        /// Instance of SQL dependency.
        /// </summary>
        private SqlDependency sampleSqlDependency;

        /// <summary>
        /// SQL command.
        /// </summary>
        private SqlCommand sampleSqlCommand;

        /// <summary>
        /// SQL connection
        /// </summary>
        private SqlConnection sampleSqlConnection;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="Program"/> class from being created. 
        /// </summary>
        private Program()
        {
            this.sampleConnectionString = ConfigurationManager.ConnectionStrings["SampleDbConnection"].ConnectionString;
            this.notificationQuery = "SELECT [SampleId],[SampleName],[SampleCategory],[SampleDateTime],[IsSampleProcessed] FROM [dbo].[SampleTable01];";
            this.notificationStoredProcedure = "uspGetSampleInformation";
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Input arguments.</param>
        public static void Main(string[] args)
        {
            var program = new Program();
            Console.WriteLine("Select from the below options:");
            Console.WriteLine("1. SqlDependency using Stored procedure and default Queue");
            Console.WriteLine("2. SqlDependency using Stored procedure and specific Queue");
            Console.WriteLine("3. SqlDependency using text query and default Queue");
            Console.WriteLine("4. SqlDependency using text query and specific Queue");
            optionSelected = Console.ReadLine();
            switch (optionSelected)
            {
                case "1":
                case "3":
                    program.RegisterDependencyUsingDefaultQueue();
                    break;
                case "2":
                case "4":
                    program.RegisterDependencyUsingSpecificQueue();
                    break;
            }

            Console.ReadLine();
            program.Dispose();
        }

        /// <summary>
        /// Dispose all used resources.
        /// </summary>
        public void Dispose()
        {
            if (null != this.sampleSqlDependency)
            {
                this.sampleSqlDependency.OnChange -= null;
            }

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }

            this.sampleSqlDependency = null;
            this.sampleSqlCommand = null;
            this.sampleSqlConnection = null;

            if (optionSelected.Equals("1") || optionSelected.Equals("3"))
            {
                SqlDependency.Stop(this.sampleConnectionString);
            }
            else if (optionSelected.Equals("2") || optionSelected.Equals("4"))
            {
                SqlDependency.Start(this.sampleConnectionString, "QueueSampleInformationDataChange");
            }
        }

        /// <summary>
        /// Register dependency using default queue name.
        /// </summary>
        private void RegisterDependencyUsingDefaultQueue()
        {
            SqlDependency.Stop(this.sampleConnectionString);
            SqlDependency.Stop(this.sampleConnectionString, "QueueSampleInformationDataChange");
            SqlDependency.Start(this.sampleConnectionString);
            if (optionSelected.Equals("1"))
            {
                this.ConfigureDependencyUsingStoreProcedureAndDefaultQueue();
            }
            else if (optionSelected.Equals("3"))
            {
                this.ConfigureDependencyUsingTextQueryAndDefaultQueue();
            }
        }

        /// <summary>
        /// Register dependency using  specific queue name.
        /// </summary>
        private void RegisterDependencyUsingSpecificQueue()
        {
            SqlDependency.Stop(this.sampleConnectionString);
            SqlDependency.Stop(this.sampleConnectionString, "QueueSampleInformationDataChange");
            SqlDependency.Start(this.sampleConnectionString, "QueueSampleInformationDataChange");
            if (optionSelected.Equals("2"))
            {
                this.ConfigureDependencyUsingStoreProcedureAndSpecificQueue();
            }
            else if (optionSelected.Equals("4"))
            {
                this.ConfigureDependencyUsingTextQueryAndSpecificQueue();
            }
        }

        /// <summary>
        /// Configure dependency using stored procedure and specified queue.
        /// </summary>
        private async void ConfigureDependencyUsingStoreProcedureAndDefaultQueue()
        {
            if (null != this.sampleSqlDependency)
            {
                this.sampleSqlDependency.OnChange -= null;
            }

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }

            this.sampleSqlDependency = null;
            this.sampleSqlCommand = null;
            this.sampleSqlConnection = null;

            //// Create connection.
            this.sampleSqlConnection = new SqlConnection(this.sampleConnectionString);

            //// Create command.
            this.sampleSqlCommand = new SqlCommand { Connection = this.sampleSqlConnection };
            this.sampleSqlCommand.CommandType = CommandType.StoredProcedure;
            this.sampleSqlCommand.CommandText = this.notificationStoredProcedure;
            this.sampleSqlCommand.Notification = null;

            //// Create Sql Dependency.
            this.sampleSqlDependency = new SqlDependency(this.sampleSqlCommand);
            this.sampleSqlDependency.OnChange += this.SqlDependencyOnChange;
            await this.sampleSqlCommand.Connection.OpenAsync();
            await this.sampleSqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }
        }

        /// <summary>
        /// Configure dependency using stored procedure and specified queue.
        /// </summary>
        private async void ConfigureDependencyUsingTextQueryAndDefaultQueue()
        {
            if (null != this.sampleSqlDependency)
            {
                this.sampleSqlDependency.OnChange -= null;
            }

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }

            this.sampleSqlDependency = null;
            this.sampleSqlCommand = null;
            this.sampleSqlConnection = null;

            //// Create connection.
            this.sampleSqlConnection = new SqlConnection(this.sampleConnectionString);

            //// Create command.
            this.sampleSqlCommand = new SqlCommand { Connection = this.sampleSqlConnection };
            this.sampleSqlCommand.CommandType = CommandType.Text;
            this.sampleSqlCommand.CommandText = this.notificationQuery;
            this.sampleSqlCommand.Notification = null;

            //// Create Sql Dependency.
            this.sampleSqlDependency = new SqlDependency(this.sampleSqlCommand);
            this.sampleSqlDependency.OnChange += this.SqlDependencyOnChange;
            await this.sampleSqlCommand.Connection.OpenAsync();
            await this.sampleSqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }
        }

        /// <summary>
        /// Configure dependency using stored procedure and specified queue.
        /// </summary>
        private async void ConfigureDependencyUsingStoreProcedureAndSpecificQueue()
        {
            if (null != this.sampleSqlDependency)
            {
                this.sampleSqlDependency.OnChange -= null;
            }

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }

            this.sampleSqlDependency = null;
            this.sampleSqlCommand = null;
            this.sampleSqlConnection = null;

            //// Create connection.
            this.sampleSqlConnection = new SqlConnection(this.sampleConnectionString);

            //// Create command.
            this.sampleSqlCommand = new SqlCommand { Connection = this.sampleSqlConnection };
            this.sampleSqlCommand.CommandType = CommandType.StoredProcedure;
            this.sampleSqlCommand.CommandText = this.notificationStoredProcedure;
            this.sampleSqlCommand.Notification = null;

            //// Create Sql Dependency.
            this.sampleSqlDependency = new SqlDependency(this.sampleSqlCommand, "service=ServiceSampleInformationDataChange;Local database=SampleDb", 432000);
            this.sampleSqlDependency.OnChange += this.SqlDependencyOnChange;
            await this.sampleSqlCommand.Connection.OpenAsync();
            await this.sampleSqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }
        }

        /// <summary>
        /// Configure dependency using stored procedure and specified queue.
        /// </summary>
        private async void ConfigureDependencyUsingTextQueryAndSpecificQueue()
        {
            if (null != this.sampleSqlDependency)
            {
                this.sampleSqlDependency.OnChange -= null;
            }

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }

            this.sampleSqlDependency = null;
            this.sampleSqlCommand = null;
            this.sampleSqlConnection = null;

            //// Create connection.
            this.sampleSqlConnection = new SqlConnection(this.sampleConnectionString);

            //// Create command.
            this.sampleSqlCommand = new SqlCommand { Connection = this.sampleSqlConnection };
            this.sampleSqlCommand.CommandType = CommandType.Text;
            this.sampleSqlCommand.CommandText = this.notificationQuery;
            this.sampleSqlCommand.Notification = null;

            //// Create Sql Dependency.
            this.sampleSqlDependency = new SqlDependency(this.sampleSqlCommand, "service=ServiceSampleInformationDataChange;Local database=SampleDb", 432000);
            this.sampleSqlDependency.OnChange += this.SqlDependencyOnChange;
            await this.sampleSqlCommand.Connection.OpenAsync();
            await this.sampleSqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            if (null != this.sampleSqlCommand)
            {
                this.sampleSqlCommand.Dispose();
            }

            if (null != this.sampleSqlConnection)
            {
                this.sampleSqlConnection.Dispose();
            }
        }

        /// <summary>
        /// Event handler for SQL dependency notification on change event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private void SqlDependencyOnChange(object sender, SqlNotificationEventArgs eventArgs)
        {
            if (eventArgs.Info == SqlNotificationInfo.Invalid)
            {
                Console.WriteLine("The above notification query is not valid.");
            }
            else
            {
                Console.WriteLine("Notification Info: " + eventArgs.Info);
                Console.WriteLine("Notification source: " + eventArgs.Source);
                Console.WriteLine("Notification type: " + eventArgs.Type);
            }

            switch (optionSelected)
            {
                case "1":
                    this.ConfigureDependencyUsingStoreProcedureAndDefaultQueue();
                    break;
                case "2":
                    this.ConfigureDependencyUsingStoreProcedureAndSpecificQueue();
                    break;
                case "3":
                    this.ConfigureDependencyUsingTextQueryAndDefaultQueue();
                    break;
                case "4":
                    this.ConfigureDependencyUsingTextQueryAndSpecificQueue();
                    break;
            }
        }
        
        #endregion
    }
}