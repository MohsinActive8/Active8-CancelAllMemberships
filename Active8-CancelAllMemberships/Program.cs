﻿using Active8_CancelAllMemberships.Models;
using Active8_CancelAllMemberships.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Active8_CancelAllMemberships
{
    /// <summary>
    /// append line 
    /// {PersonID: 1, Key: 'f4aefc3c3a0233367502f171a395841f', ObjectType: 'FusionFramework.Objects.Web.SalesDocument', Method: 'CancelContract'}
    /// into the web.config file with KEY = "ApiKeys"
    /// </summary>
    class Program
    {
        private static List<int> arr;
        static void Main(string[] args)
        {
            DateTime start, end;

            string connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                start = DateTime.Now;
                Logger.Log("Establishing connection with DB...");

                //open connection
                conn.Open();

                Logger.Log("Connection successful!");

                /// TODO: get users, delete memberships
                //select * from fin.Membership where ContractSalesDocumentID in (select SalesDocumentID from fin.salesdocument where SalesDocumentTypeID = 1 and AddressID in (832969,874969)) and ProductMembershipID in (select ProductMembershipID from fin.ProductMembership where ProductId not in (2716, 2791, 2892, 2893))
                // getting users from db to cancel member ships

                Logger.Log("Getting users list to cancel memberships...");

                //string query = ConfigurationManager.AppSettings.Get("query");
                ReadFromTextFile();
                string query = $"select * from jmp.AccountWallet where AccountID in({String.Join(',', arr)})";

                if (String.IsNullOrEmpty(query))
                {
                    throw new Exception("\"query\" should not be null or empty");
                }

                List<AccountWalletModel> accWallets = GetAccountWallet(query, conn);

                for (int i = 0; i < accWallets.Count; i++)
                {
                    try
                    {
                        Logger.Log($"Processing {i + 1} of {accWallets.Count}");
                        Logger.Log($"AccountWallet ID :: {accWallets[i].AccountWalletID}, Account ID :: {accWallets[i].AccountID}, CCNumber :: {accWallets[i].CCNumber}");

                        var encryptedToken = Security.Encrypt(accWallets[i].CCNumber, ConfigurationManager.AppSettings.Get("Secret"));

                        var res = UpdateAccountWalletToken(accWallets[i].AccountWalletID, encryptedToken, conn);
                        Logger.Log($"{res} rows affected");

                    }
                    catch (Exception ex)
                    {

                        Logger.LogError(ex);
                    }
                }

                //List<MembershipUsersModel> users = GetUsers(query, conn);

                //Logger.Log($"Record Fetched :: {users.Count}");

                ///// TODO: Delete Membership
                //foreach (var user in users)
                //{
                //    try
                //    {
                //        Logger.Log($"Person ID :: {user.PersonID}, Salesdocument ID :: {user.ContractSalesDocumentID}, Membership ID :: {user.MembershipID}, Membership Status ID :: {user.MembershipStatusID}");

                //        var resp = ApiCall.Custom("FusionFramework.Objects.Web.SalesDocument", "CancelContract", 
                //            new Dictionary<string, object>
                //            {
                //                {"salesDocumentID", user.ContractSalesDocumentID},
                //                {"voidContract", true }
                //            }, "f4aefc3c3a0233367502f171a395841f");

                //        Logger.CustomLog($"Process of MemberShip Termination for Person {user.PersonID} ended.", LogStatus.success);
                //        Logger.CustomLog(JsonConvert.SerializeObject(resp), LogStatus.success);
                //    }
                //    catch (Exception ex)
                //    {
                //        Logger.LogError(ex);
                //    }
                //}

                end = DateTime.Now;
                TimeSpan duration = end - start;
                Logger.Log($"Total time elapsed :: {duration.TotalSeconds} sec");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }


        }

        private static List<AccountWalletModel> GetAccountWallet(string query, SqlConnection conn)
        {
            Logger.LogSql(query);

            List<AccountWalletModel> data = new List<AccountWalletModel>();

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                var reader = command.ExecuteReader();
                Logger.Log("Reading Data....");
                while (reader.Read())
                {
                    Console.WriteLine(reader.ToString());
                    var row = new AccountWalletModel()
                    {
                        AccountWalletID = SafeConvert.SafeInt(reader["AccountWalletID"].ToString().Trim()),
                        AccountID = SafeConvert.SafeInt(reader["AccountID"].ToString().Trim()),
                        Name = reader["Name"].ToString().Trim(),
                        CCNumber = reader["CCNumber"].ToString().Trim()
                    };

                    data.Add(row);
                }
                if (!reader.IsClosed)
                    reader.Close();

                return data;
            }
        }
        
        private static int UpdateAccountWalletToken(int AccountWalletID, string token, SqlConnection conn)
        {
            string query = "update jmp.AccountWallet set CCNumber = '"+token+ "' where AccountWalletID = "+AccountWalletID+"";
            Logger.LogSql(query);

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                var reader = command.ExecuteNonQuery();
                return reader;
            }
        }

        private static List<MembershipUsersModel> GetUsers(string query, SqlConnection conn)
        {
            Logger.LogSql(query);

            List<MembershipUsersModel> data = new List<MembershipUsersModel>();

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                var reader = command.ExecuteReader();
                Logger.Log("Reading Data....");
                while (reader.Read())
                {
                    var row = new MembershipUsersModel()
                    {
                        MembershipID = SafeConvert.SafeInt(reader["MembershipID"].ToString().Trim()),
                        PersonID = SafeConvert.SafeInt(reader["PersonID"].ToString().Trim()),
                        MembershipStatusID = SafeConvert.SafeInt(reader["MembershipStatusID"].ToString().Trim()),
                        SalesDocumentItemID = SafeConvert.SafeInt(reader["SalesDocumentItemID"].ToString().Trim()),
                        ProductMembershipID = SafeConvert.SafeInt(reader["ProductMembershipID"].ToString().Trim()),
                        EventID = SafeConvert.SafeInt(reader["EventID"].ToString().Trim()),
                        CardNumber = SafeConvert.SafeInt(reader["CardNumber"].ToString().Trim()),
                        Magstripe = SafeConvert.SafeInt(reader["Magstripe"].ToString().Trim()),
                        Pin = SafeConvert.SafeInt(reader["Pin"].ToString().Trim()),
                        CreatePersonID = SafeConvert.SafeInt(reader["CreatePersonID"].ToString().Trim()),
                        CreateDate = reader["CreateDate"].ToString() == "" ? (DateTime?)null : Convert.ToDateTime(reader["CreateDate"].ToString().Trim()),
                        UpdatePersonID = SafeConvert.SafeInt(reader["UpdatePersonID"].ToString().Trim()),
                        UpdateDate = reader["UpdateDate"].ToString() == "" ? (DateTime?)null : Convert.ToDateTime(reader["UpdateDate"].ToString().Trim()),
                        ContractSalesDocumentID = SafeConvert.SafeInt(reader["ContractSalesDocumentID"].ToString().Trim()),
                        CancelDate = reader["CancelDate"].ToString() == "" ? (DateTime?)null : Convert.ToDateTime(reader["CancelDate"].ToString().Trim()),
                        CurrentMembershipContractID = SafeConvert.SafeInt(reader["CurrentMembershipContractID"].ToString().Trim())
                    };

                    data.Add(row);
                }
                if (!reader.IsClosed)
                    reader.Close();

                return data;
            }
        }
        static void ReadFromTextFile()
        {
            string filename = ConfigurationManager.AppSettings.Get("FileName");
            string path = Path.Combine(Directory.GetCurrentDirectory(), filename);
            if (File.Exists(path))
            {
                using (StreamReader file = new StreamReader(path))
                {
                    string ln;
                    arr = new List<int>();
                    while ((ln = file.ReadLine()) != null)
                    {
                        Console.WriteLine(ln);
                        arr.Add(Convert.ToInt32(ln));
                    }
                }
            }
            else
            {
                Console.WriteLine($"Cannot find the specified file on path : {path}");
            }
        }
    }
}
