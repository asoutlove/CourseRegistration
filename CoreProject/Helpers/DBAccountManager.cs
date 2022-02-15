using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using CoreProject.Models;
using CoreProject.Helpers;
using System.Text.RegularExpressions;

namespace Ubay_CourseRegistration
{
    public class DBAccountManager: DBBase
    {
        public static DataTable GetUserAccount(string account)
        {
            string connectionstring =
                GetConnectionString();

            string queryString =
                $@" SELECT *
                    FROM Account_summary 
                    LEFT  OUTER JOIN Manager
                    ON Manager.Account  = Account_summary.Account 
					LEFT  OUTER JOIN Student
                    ON   Account_summary.Account = Student.Idn  
                    WHERE Account_summary.Account = 
                    @account AND Student.d_empno IS NULL AND Manager.d_empno IS NULL ;";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@account", account);
                


                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    reader.Close();
                    return dt;
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }

            }


        }

        /// <summary>
        /// 取得帳號資料
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public AccountModel GetAccount(string Account)
        {
            string connectionString = GetConnectionString();
            string queryString =
                $@" SELECT * FROM Account_summary
                    WHERE Account = @Account;
                ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Account", Account);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    AccountModel model = null;

                    while (reader.Read())
                    {
                        model = new AccountModel();
                        model.Acc_sum_ID = (Guid)reader["Acc_sum_ID"];
                        model.Account = (string)reader["Account"];
                        model.password = (string)reader["password"];
                        model.Type = (bool)reader["Type"];
                    }

                    reader.Close();

                    return model;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public AccountModel Chackpwd(string Account,string pwd)
        {
            string connectionString = GetConnectionString();
            string queryString =
                $@" SELECT * FROM Account_summary
                    WHERE Account = @Account AND password= @password;
                ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@Account", Account);
                command.Parameters.AddWithValue("@password", pwd);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    AccountModel model = null;

                    while (reader.Read())
                    {
                        model = new AccountModel();
                        model.password = (string)reader["password"];
                    }

                    reader.Close();

                    return model;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 身分證驗證
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Check(string id)
        {
            // 使用「正規表達式」檢驗格式 [A~Z] {1}個數字 [0~9] {9}個數字
            var regex = new Regex("^[A-Z]{1}[0-9]{9}$");
            if (!regex.IsMatch(id))
            {
                //Regular Expression 驗證失敗，回傳 ID 錯誤
                return false;
            }

            //除了檢查碼外每個數字的存放空間 
            int[] seed = new int[10];

            //建立字母陣列(A~Z)
            //A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
            //P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35            
            string[] charMapping = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
            string target = id.Substring(0, 1).ToUpper(); //取第一個英文數字
            for (int index = 0; index < charMapping.Length; index++)
            {
                if (charMapping[index] == target)
                {
                    index += 10;
                    //10進制的高位元放入存放空間   (權重*1)
                    seed[0] = index / 10;

                    //10進制的低位元*9後放入存放空間 (權重*9)
                    seed[1] = (index % 10) * 9;

                    break;
                }
            }
            for (int index = 2; index < 10; index++) //(權重*8~1)
            {   //將剩餘數字乘上權數後放入存放空間                
                seed[index] = Convert.ToInt32(id.Substring(index - 1, 1)) * (10 - index);
            }
            //檢查是否符合檢查規則，10減存放空間所有數字和除以10的餘數的個位數字是否等於檢查碼            
            //(10 - ((seed[0] + .... + seed[9]) % 10)) % 10 == 身分證字號的最後一碼   
            if ((10 - (seed.Sum() % 10)) % 10 != Convert.ToInt32(id.Substring(9, 1)))
            {
                return false;
            }

            return true;
        }

    }
}