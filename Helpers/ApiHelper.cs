using System;
using System.Data.SqlClient;
using NomadMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace NomadMVC.Api {
    public class ApiHelper {
        private string connectionString;
        public ApiHelper(string connection) {
            this.connectionString = connection;
        }

        public List<EliasModel> GetCustomItems(int page, string filter) {
            List<EliasModel> result = new List<EliasModel>();

            using (SqlConnection con = new SqlConnection(this.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Elias.GetCustomItems"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Page", SqlDbType.Int).Value = page;
                    cmd.Parameters.Add("@Filter", SqlDbType.NVarChar).Value = filter;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if (reader.HasRows) {
                                result.Add(new EliasModel());
                                result.Last().Id = reader.GetInt32(0);
                                result.Last().Name = reader.GetString(1);
                                result.Last().TypeId = reader.GetInt32(2);
                                result.Last().Type = reader.GetString(3);
                                if (!reader.IsDBNull(4)) {
                                    result.Last().ImageArray = (byte[])reader.GetValue(4);
                                }
                                if (!reader.IsDBNull(5))
                                    result.Last().ParentId = reader.GetInt32(5);
                                if (!reader.IsDBNull(6))
                                    result.Last().ParentName = reader.GetString(6);
                                result.Last().IsDeleted = reader.GetBoolean(7);
                            }
                        }
                    }
                    con.Close();
                }
            }
            return result;
        }
        public EliasModel GetCustomItem(int customItemId) {
            EliasModel result = new EliasModel();
            
            using (SqlConnection con = new SqlConnection(this.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Elias.GetCustomItem"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CustomItemId", SqlDbType.Int).Value = customItemId;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if (reader.HasRows) {
                                result.Id = reader.GetInt32(0);
                                result.Name = reader.GetString(1);
                                result.TypeId = reader.GetInt32(2);
                                result.Type = reader.GetString(3);
                                if (!reader.IsDBNull(4)) {
                                    result.ImageArray = (byte[])reader.GetValue(4);
                                }
                                if (!reader.IsDBNull(5))
                                    result.ParentId = reader.GetInt32(5);
                                result.IsDeleted = reader.GetBoolean(6);
                            }
                        }
                    }
                    con.Close();
                }
            }
            return result;
        }
        public int GetCustomItemCount(string filter) {
            int result = 0;

            using (SqlConnection con = new SqlConnection(this.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Elias.GetCustomItemCount"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Filter", SqlDbType.NVarChar).Value = filter;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if (reader.HasRows) {
                                result = reader.GetInt32(0);
                            }
                        }
                    }
                    con.Close();
                }
            }

            return result;
        }
        public List<EliasTypeModel> GetCustomItemTypes() {
            List<EliasTypeModel> result = new List<EliasTypeModel>();

            using (SqlConnection con = new SqlConnection(this.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Elias.GetCustomTypes"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if (reader.HasRows) {
                                result.Add(new EliasTypeModel());
                                result.Last().Id = reader.GetInt32(0);
                                result.Last().Name = reader.GetString(1);
                            }
                        }
                    }
                    con.Close();
                }
            }
            return result;
        }
        public bool AuthenticateUser(string userName, string password) {
            bool result = false;

            using (SqlConnection con = new SqlConnection(this.connectionString)) {
                using (SqlCommand cmd = new SqlCommand("Elias.AuthenticateUser"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if (reader.HasRows) {
                                result = reader.GetBoolean(0);
                            }
                        }
                    }
                    con.Close();
                }
            }

            return result;
        }
        public bool UserExits(string userName) {
            bool result = false;

            using (SqlConnection con = new SqlConnection(this.connectionString)) {
                using (SqlCommand cmd = new SqlCommand("Elias.UserExists"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if (reader.HasRows) {
                                result = reader.GetBoolean(0);
                            }
                        }
                    }
                    con.Close();
                }
            }

            return result;
        }

        public int InsertCustomItem(EliasModel customItem, IFormFile Image) {
            int result = 0;

            using (SqlConnection con = new SqlConnection(this.connectionString)) {
                using (SqlCommand cmd = new SqlCommand("Elias.InsertCustomItem"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = customItem.Name;
                    cmd.Parameters.Add("@TypeId", SqlDbType.Int).Value = customItem.TypeId;
                    if (customItem.Image != null) {
                        MemoryStream memoryStream = new MemoryStream();
                        customItem.Image.CopyTo(memoryStream);
                        cmd.Parameters.Add("@Image", SqlDbType.Image).Value = memoryStream.ToArray();
                    }
                    cmd.Parameters.Add("@ParentId", SqlDbType.Int).Value = customItem.ParentId;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if (reader.HasRows) {
                                result = Convert.ToInt32(reader.GetValue(0));
                            }
                        }
                    }
                    con.Close();
                }
            }

            return result;
        }

        public void InsertUser(string userName, string password) {
            using (SqlConnection con = new SqlConnection(this.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Elias.InsertUser"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public void UpdateCustomItem(EliasModel customItem) {
            using (SqlConnection con = new SqlConnection(this.connectionString)) {
                using (SqlCommand cmd = new SqlCommand("Elias.UpdateCustomItem"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CustomItemId", SqlDbType.Int).Value = customItem.Id;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = customItem.Name;
                    cmd.Parameters.Add("@TypeId", SqlDbType.Int).Value = customItem.TypeId;
                    if (customItem.Image != null) {
                        MemoryStream memoryStream = new MemoryStream();
                        customItem.Image.CopyTo(memoryStream);
                        cmd.Parameters.Add("@Image", SqlDbType.Image).Value = memoryStream.ToArray();
                    }
                    cmd.Parameters.Add("@ParentId", SqlDbType.Int).Value = customItem.ParentId; //.HasValue ? customItem.ParentId.Value : DBNull.Value;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }

        public void DeleteCustomItem(int customItemId) {
            using (SqlConnection con = new SqlConnection(this.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Elias.DeleteCustomItem"))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CustomItemId", SqlDbType.Int).Value = customItemId;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
    }
}