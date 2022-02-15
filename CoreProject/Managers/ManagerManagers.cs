using CoreProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using CoreProject.Helpers;
using CoreProject.ViewModels;


namespace Ubay_CourseRegistration.Managers
{
    public class ManagerManagers : DBBase
    {

        public static void InsertAdminTablel(AccountModel acmodel, Account_summaryModel asmodel, string createtime, Guid Creator)
        {
            string connectionstring = GetConnectionString();

            string queryString =
                $@"

                INSERT INTO Account_summary
                    (Acc_sum_ID,Account, password, Type)
                VALUES
                    (@GUID,@Account, @Password, @Type);
                INSERT INTO Manager
                    (Manager_ID,Manager_FirstName,Manager_LastName,Department,Account,b_date,b_empno)
                VALUES
                    (@GUID,@Firstname,@Lastname,@Department,@Account,@createtime,@Creator);
                ";



            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@GUID", acmodel.Acc_sum_ID);
                command.Parameters.AddWithValue("@Firstname", asmodel.firstname);
                command.Parameters.AddWithValue("@Lastname", asmodel.lastname);
                command.Parameters.AddWithValue("@Department", asmodel.department);
                command.Parameters.AddWithValue("@Account", acmodel.Account);
                command.Parameters.AddWithValue("@Password", acmodel.password);
                command.Parameters.AddWithValue("@Pwdcheck", asmodel.Pwdcheck);
                command.Parameters.AddWithValue("@Type", acmodel.Type);
                command.Parameters.AddWithValue("@createtime", createtime);
                command.Parameters.AddWithValue("@Creator", Creator);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    
                }

                catch (Exception ex)
                {
                    HttpContext.Current.Response.Write(ex.Message);
                }
            }
        } //新增管理人(勿更改)
        /// <summary>
        /// 取得單筆帳號
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public AccountModel GetAccount(string Account)

        {
            string connectionstring =
                GetConnectionString();

            string queryString =
                $@" SELECT * FROM Account_summary
                    WHERE Account = @Account;";

            using (SqlConnection connection = new SqlConnection(connectionstring))
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
                        model.Account = (string)reader["Account"];

                    }
                    reader.Close();
                    return model;
                }

                catch (Exception ex)
                {
                    HttpContext.Current.Response.Write(ex.Message);
                    return null;
                }

            }

        }

         /// <summary>
         /// 取得學生資料並分頁顯示
         /// </summary>
         /// <param name="name"></param>
         /// <param name="Idn"></param>
         /// <param name="totalSize"></param>
         /// <param name="currentPage"></param>
         /// <param name="pageSize"></param>
         /// <returns></returns>
        public List<StudentAccountViewModel> GetStudentViewModels(
      string name, string Idn, out int totalSize, int currentPage = 1, int pageSize = 10)
        {
            //----- Process filter conditions -----
            List<string> conditions = new List<string>();

            if (!string.IsNullOrEmpty(name))
                conditions.Add(" Student.S_FirstName+Student.S_LastName LIKE '%' + @name + '%'");

            if (!string.IsNullOrEmpty(Idn))
                conditions.Add(" Idn = @Idn");

            string filterConditions =
                (conditions.Count > 0)
                    ? (" WHERE " + string.Join(" AND ", conditions))
                    : string.Empty;
            //----- Process filter conditions -----


            string query =
                $@"         
					SELECT TOP {10} * FROM
                    (
                        SELECT 
                            ROW_NUMBER() OVER(ORDER BY Student.Idn) AS RowNumber,
                            Student.Student_ID,
                            Student.S_FirstName,
                            Student.S_LastName,
                            Student.gender,
                            Student.Idn,
							Student.CellPhone,
                            Student.Address,
                            Student.d_empno
                        FROM Student
                        JOIN Account_summary
                        ON Student.Student_ID = Account_summary.Acc_sum_ID
                        {filterConditions}
                    ) AS TempT
                    WHERE RowNumber > {pageSize * (currentPage - 1)} AND d_empno IS NULL
                    ORDER BY Student_ID
                    ";

            string countQuery =
                $@" SELECT 
                        COUNT(Student.Idn) 
                    FROM Student
                    JOIN Account_summary
                    ON  Student.Student_ID = Account_summary.Acc_sum_ID
                    {filterConditions}
                ";

            List<SqlParameter> dbParameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(name))
                dbParameters.Add(new SqlParameter("@name", name));

            if (!string.IsNullOrEmpty(Idn))
                dbParameters.Add(new SqlParameter("@Idn", Idn));


            var dt = this.GetDataTable(query, dbParameters);

            List<StudentAccountViewModel> list = new List<StudentAccountViewModel>();

            foreach (DataRow dr in dt.Rows)
            {
                StudentAccountViewModel model = new StudentAccountViewModel();
                model.Student_ID = (Guid)dr["Student_ID"];
                model.S_FirstName = (string)dr["S_FirstName"];
                model.S_LastName = (string)dr["S_LastName"];
                model.gender = (bool)dr["gender"];
                model.Idn = (string)dr["Idn"];
                model.CellPhone = (string)dr["CellPhone"];
                model.Address = (string)dr["Address"];


                list.Add(model);
            }


            // 算總數並回傳
            int? totalSize2 = this.GetScale(countQuery, dbParameters) as int?;
            totalSize = (totalSize2.HasValue) ? totalSize2.Value : 0;

            return list;
        }

        /// <summary>
        /// 刪除學生(資料庫新增刪除者、刪除時間) 需(學生ID、登入者ID參數)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="destoryer"></param>
        public void DeleteStudentViewModel(Guid id,Guid destoryer)
        {
            string dbCommandText =
                $@" UPDATE Student
                    SET 
                        d_empno = @d_empno, 
                        d_date = @d_date 
                    WHERE
                        Student_ID = @Student_ID;
                ";

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@Student_ID", id),
                new SqlParameter("@d_empno", destoryer),
                new SqlParameter("@d_date", DateTime.Now)
            };

            this.ExecuteNonQuery(dbCommandText, parameters);
        }

        public void DeleteManagerViewModel(Guid id, Guid destoryer)
        {
            string dbCommandText =
                $@" UPDATE Manager
                    SET 
                        d_empno = @d_empno, 
                        d_date = @d_date 
                    WHERE
                        Manager_ID = @Manager_ID;
                ";

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@Manager_ID", id),
                new SqlParameter("@d_empno", destoryer),
                new SqlParameter("@d_date", DateTime.Now)
            };

            this.ExecuteNonQuery(dbCommandText, parameters);
        }
        /// <summary>
        /// 性別顯示
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        public string GetgenderName(bool gender)
        {
            switch (gender)
            {
                case false:
                    return "男";
                case true:
                    return "女";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 讀取單筆學生資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StudentAccountViewModel GetStudentViewModel(Guid id)
        {
            string connectionString = GetConnectionString();
            string queryString =
                $@" SELECT 
                        Student.Student_ID,
                        Student.S_FirstName,
                        Student.S_LastName,
                        Student.Birthday,
                        Student.idn,
                        Student.Email,
                        Student.Address,
                        Student.CellPhone,
                        Student.Education,
                        Student.School_ID,
                        Student.Experience,
                        Student.ExYear,
                        Student.gender,
                        Student.PassNumber,
                        Student.PassPic,
                        Student.d_empno,
                        Account_summary.password
                    FROM Student
                    JOIN Account_summary
                        ON Student.Student_ID = Account_summary.Acc_sum_ID
                    WHERE Student.Student_ID = @id AND d_empno IS NULL
                ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    StudentAccountViewModel model = null;

                    while (reader.Read())
                    {
                        model = new StudentAccountViewModel();
                        model.Student_ID = (Guid)reader["Student_ID"];
                        model.S_FirstName = (string)reader["S_FirstName"];
                        model.S_LastName = (string)reader["S_LastName"];
                        model.Birthday = (DateTime)reader["Birthday"];
                        model.Idn = (string)reader["Idn"];
                        model.Email = (string)reader["Email"];
                        model.Address = (string)reader["Address"];
                        model.CellPhone = (string)reader["CellPhone"];
                        model.Education = (byte)reader["Education"];
                        model.School_ID = (int)reader["School_ID"];
                        model.Experience = (bool)reader["Experience"];
                        model.ExYear = (byte)reader["ExYear"];
                        model.gender = (bool)reader["gender"];
                        model.PassNumber = (string)reader["PassNumber"];
                        model.PassPic = (string)reader["PassPic"];

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

        private bool HasAccount(string account)
        {
            return false;
        }

        /// <summary>
        /// 新增學生資料
        /// </summary>
        /// <param name="model"></param>
        public void CreatStudent(StudentAccountViewModel model)
        {

            Guid student_id = Guid.NewGuid();


            string queryString =
                $@" INSERT INTO Account_summary
                    (Acc_sum_ID,Account,password,Type)
                VALUES
                    (@Acc_sum_ID,@Account,@password,@Type);


                INSERT INTO Student
                    (Student_ID,S_FirstName,S_LastName,Birthday,Idn,Email,Address,CellPhone,Education,School_ID,
                        Experience,ExYear,gender,PassNumber,PassPic,b_empno,b_date)
                    
                VALUES
                    (@Student_ID,@S_FirstName,@S_LastName,@Birthday,@idn,@Email,@Address,@CellPhone,@Education,@School_ID,
                        @Experience,@ExYear,@gender,@PassNumber,@PassPic,@b_empno,@b_date);

";

            List<SqlParameter> parameters = new List<SqlParameter>()
            {

            new SqlParameter("@Student_ID", student_id),
            new SqlParameter("@S_FirstName", model.S_FirstName),
            new SqlParameter("@S_LastName", model.S_LastName),
            new SqlParameter("@Birthday", model.Birthday),
            new SqlParameter("@idn", model.Idn),
            new SqlParameter("@Email", model.Email),
            new SqlParameter("@Address", model.Address),
            new SqlParameter("@CellPhone", model.CellPhone),
            new SqlParameter("@Education", model.Education),
            new SqlParameter("@School_ID", model.School_ID),
            new SqlParameter("@Experience", model.Experience),
            new SqlParameter("@ExYear", model.ExYear),
            new SqlParameter("@gender", model.gender),
            new SqlParameter("@PassNumber",model.PassNumber),
            new SqlParameter("@PassPic",model.PassPic),
            new SqlParameter("@b_empno", model.b_empno),
            new SqlParameter("@b_date", DateTime.Now),

            new SqlParameter("@Acc_sum_ID", student_id),
            new SqlParameter("@Account", model.Idn),
            new SqlParameter("@password", model.password),
            new SqlParameter("@Type", false)
            };

            this.ExecuteNonQuery(queryString, parameters);

        }
        /// <summary>
        /// 修改學生資料
        /// </summary>
        /// <param name="model"></param>
        public void UpdataStudent(StudentAccountViewModel model)
        {


            string queryString =
                $@" UPDATE Account_summary
                    SET 
                        Account = @Account, 
                        password = @password 
                    WHERE
                        Acc_sum_ID = @Acc_sum_ID;

                    UPDATE Student
                    SET 
                        S_FirstName = @S_FirstName, 
                        S_LastName = @S_LastName, 
                        Birthday = @Birthday, 
                        Idn = @idn, 
                        Email = @Email, 
                        Address = @Address, 
                        CellPhone = @CellPhone, 
                        Education = @Education, 
                        School_ID = @School_ID, 
                        Experience = @Experience, 
                        ExYear = @ExYear, 
                        gender = @gender, 
                        PassNumber = @PassNumber, 
                        PassPic = @PassPic, 
                        e_empno = @e_empno, 
                        e_date = @e_date 
                    WHERE
                        Student_ID = @Student_ID;
                    ";

            List<SqlParameter> parameters = new List<SqlParameter>()
            {
            new SqlParameter("@Acc_sum_ID", model.Student_ID),
            new SqlParameter("@Account", model.Idn),
            new SqlParameter("@password", model.password),
            new SqlParameter("@Type", false),


            new SqlParameter("@Student_ID", model.Student_ID),
            new SqlParameter("@S_FirstName", model.S_FirstName),
            new SqlParameter("@S_LastName", model.S_LastName),
            new SqlParameter("@Birthday", model.Birthday),
            new SqlParameter("@idn", model.Idn),
            new SqlParameter("@Email", model.Email),
            new SqlParameter("@Address", model.Address),
            new SqlParameter("@CellPhone", model.CellPhone),
            new SqlParameter("@Education", model.Education),
            new SqlParameter("@School_ID", model.School_ID),
            new SqlParameter("@Experience", model.Experience),
            new SqlParameter("@ExYear", model.ExYear),
            new SqlParameter("@gender", model.gender),
            new SqlParameter("@PassNumber",model.PassNumber),
            new SqlParameter("@PassPic",model.PassPic),
            new SqlParameter("@e_empno", model.e_empno),
            new SqlParameter("@e_date", DateTime.Now)

            };
            this.ExecuteNonQuery(queryString, parameters);
        } 
    
        public AccountViewModel GetAccountViewModel(Guid id)
        {
            string connectionString = GetConnectionString();
            string queryString =
                $@" SELECT
                        Manager.Manager_ID,
                        Manager.Manager_FirstName,
                        Manager.Manager_LastName,
                        Manager.Department,
                        Manager.Account,
                        Account_summary.password
                    FROM Manager 
                    INNER JOIN Account_summary 
                    ON Account_summary.Acc_sum_ID=Manager.Manager_ID
                    WHERE Account_summary.Acc_sum_ID = @id;
          
                ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@id", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    AccountViewModel model = null;

                    while (reader.Read())
                    {
                        model = new AccountViewModel();
                        model.Manager_ID = (Guid)reader["Manager_ID"];
                        model.firstname = (string)reader["Manager_FirstName"];
                        model.lastname = (string)reader["Manager_LastName"];
                        model.department = (string)reader["Department"];
                        model.Account = (string)reader["Account"];
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
        /// 修改管理人
        /// </summary>
        /// <param name="acmodel"></param>
        /// <param name="asmodel"></param>
        /// <param name="updatetime"></param>
        /// <param name="editor"></param>
        public static void UpdateAdminTablel(AccountModel acmodel, Account_summaryModel asmodel, string updatetime, Guid editor)
        {
            string connectionstring = GetConnectionString();

            string queryString =
                $@"UPDATE  Account_summary 
                        SET
                            Account = @Account,
                            password = @password
                        WHERE
                            Acc_sum_ID = @Acc_sum_ID;
                        UPDATE Manager
                        SET 
                            Manager_FirstName = @Firstname, 
                            Manager_LastName = @Lastname  , 
                            Department = @Department, 
                            Account = @Account, 
                            e_empno = @editor, 
                            e_date = @updatetime
                        WHERE
                            Manager_ID = @Acc_sum_ID;
                ";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                command.Parameters.AddWithValue("@Acc_sum_ID", acmodel.Acc_sum_ID);
                command.Parameters.AddWithValue("@Firstname", asmodel.firstname);
                command.Parameters.AddWithValue("@Lastname", asmodel.lastname);
                command.Parameters.AddWithValue("@Department", asmodel.department);
                command.Parameters.AddWithValue("@Account", acmodel.Account);
                command.Parameters.AddWithValue("@password", acmodel.password);
                command.Parameters.AddWithValue("@editor", editor);
                command.Parameters.AddWithValue("@updatetime", updatetime);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                    HttpContext.Current.Response.Write(ex.Message);
                }
            } 
        }

        public List<AccountViewModel> GetManagerViewModels(
      string name, string Account, out int totalSize, int currentPage = 1, int pageSize = 10)
        {
            //----- Process filter conditions -----
            List<string> conditions = new List<string>();

            if (!string.IsNullOrEmpty(name))
                conditions.Add(" Manager.Manager_FirstName+Manager.Manager_LastName LIKE '%' + @name + '%'");

            if (!string.IsNullOrEmpty(Account))
                conditions.Add(" Account = @Account");

            string filterConditions =
                (conditions.Count > 0)
                    ? (" WHERE " + string.Join(" AND ", conditions))
                    : string.Empty;
            //----- Process filter conditions -----


            string query =
                $@"         
					SELECT TOP {10} * FROM
                    (
                        SELECT 
                            ROW_NUMBER() OVER(ORDER BY Manager.Account) AS RowNumber,
                            Manager.Manager_ID,
                            Manager.Manager_FirstName,
                            Manager.Manager_LastName,
                            Manager.Department AS 單位,
                            Manager.Account AS 帳號,
                            Manager.d_empno
                        FROM Manager
                        JOIN Account_summary
                        ON Manager.Manager_ID = Account_summary.Acc_sum_ID
                        {filterConditions}
                    ) AS TempT
                    WHERE RowNumber > {pageSize * (currentPage - 1)} AND TempT.d_empno IS NULL
                    ORDER BY 帳號
                    ";

            string countQuery =
                $@" SELECT 
                        COUNT(Manager.Account) 
                    FROM Manager
                    JOIN Account_summary
                    ON  Manager.Manager_ID = Account_summary.Acc_sum_ID
                    {filterConditions}
                ";

            List<SqlParameter> dbParameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(name))
                dbParameters.Add(new SqlParameter("@name", name));

            if (!string.IsNullOrEmpty(Account))
                dbParameters.Add(new SqlParameter("@Account", Account));


            var dt = this.GetDataTable(query, dbParameters);

            List<AccountViewModel> list = new List<AccountViewModel>();

            foreach (DataRow dr in dt.Rows)
            {
                AccountViewModel model = new AccountViewModel();
                model.Manager_ID = (Guid)dr["Manager_ID"];
                model.firstname = (string)dr["Manager_FirstName"];
                model.lastname = (string)dr["Manager_LastName"];
                model.department = (string)dr["單位"];
                model.Account = (string)dr["帳號"];

                list.Add(model);
            }


            // 算總數並回傳
            int? totalSize2 = this.GetScale(countQuery, dbParameters) as int?;
            totalSize = (totalSize2.HasValue) ? totalSize2.Value : 0;

            return list;
        }

    }
}


