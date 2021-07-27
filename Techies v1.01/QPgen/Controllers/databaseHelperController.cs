using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace QPgen.Controllers
{
    // Database helper methods is called whenever any database operation is performed 
    public class databaseHelperController
    {
        static string strcon = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
        // Connection string
        SqlConnection con = new SqlConnection(strcon);
        // Establishing a database connection
        public int getID(string username)
        {
            SqlCommand cmd = new SqlCommand("GetID", con);
            cmd.CommandType = CommandType.StoredProcedure;
            // Calling Stored procedure getID
            cmd.Parameters.AddWithValue("@username", username);
            con.Open();
            int id = (int)cmd.ExecuteScalar();
            con.Close();
            return id;
        }
        public int checkemail(string email)
        {
            SqlCommand cmd = new SqlCommand("CheckEmail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            // Calling stored procedure checkemail
            cmd.Parameters.AddWithValue("@email", email);
            con.Open();
            int id = (int)cmd.ExecuteScalar();
            con.Close();
            return id;
        }
        public int checklogin(string username, string password)
        {
            // Method to check login username and password 
            string source = password;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                password = GetHash(sha256Hash, source);
            }
            SqlCommand cmd = new SqlCommand("CheckLogin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            // calling stored procedure checklogin with parameters as username and password
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            con.Open();
            int status = (int)cmd.ExecuteScalar();
            con.Close();
            return status;
        }
        public string[] getinfo()
        {
            // get info method is used for displaying dashboard related total counts list  
            string[] data = new string[4];
            SqlCommand cmd = new SqlCommand("GetInfo", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // calling stored procedure getInfo 
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            sdr.Read();
            data[0] = sdr[0].ToString();
            data[1] = sdr[1].ToString();
            data[2] = sdr[2].ToString();
            data[3] = sdr[3].ToString();

            // Procedure returning 4 values 
            sdr.Close();
            con.Close();
            return data;
        }
        public string[] getDashboardDetails(int id)
        {
            // database helper method get Dashboard details is used for getting the detials of the logged in user like images and first name and last name 
            string []data = new string[3] ;
            SqlCommand cmd = new SqlCommand("GetDashboardDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            // Stored procedure getdashboard detials is called
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            sdr.Read();
            data[0] = sdr["Fname"].ToString();
            data[1] = sdr["Lname"].ToString();
            data[2] = sdr["Image"].ToString();
            sdr.Close();
            con.Close();
            return data;
        }
        public string[] getProfile(int id)
        {
            // Get profile method is used to get the list of  entire profile of the logged in user like first name last name , email , inmage and gender
            string[] data = new string[6];
            SqlCommand cmd = new SqlCommand("GetProfile", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            sdr.Read();
            data[0] = sdr["Fname"].ToString();
            data[1] = sdr["Lname"].ToString();
            data[2] = sdr["Email"].ToString();
            data[3] = sdr["Image"].ToString();
            data[4] = sdr["Gender"].ToString();
            sdr.Close();
            con.Close();
            return data;
        }
        public int CheckPassword(int id, string cpass)
        {// check password database helper method
            string source = cpass;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                cpass = GetHash(sha256Hash, source);
            }
            SqlCommand cmd = new SqlCommand("CheckPassword", con);
            cmd.CommandType = CommandType.StoredProcedure;
            // calling checkpassword stored procedure
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@cpass", cpass);
            con.Open();
            int status = (int)cmd.ExecuteScalar();
            con.Close();
            return status;
        }
        public int changepassword(int id, string pass)
        {
            // changing the password and generation SHA256 for the new password and storing in the database
            string source = pass;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                pass = GetHash(sha256Hash, source);
            }
            SqlCommand cmd = new SqlCommand("ChangePassword", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@pass", pass);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int UpdateProfile(int id, string fname, string lname, string gender)
        {
            // Updating the profile and using the stored procedure Update Profile 
            SqlCommand cmd = new SqlCommand("UpdateProfile", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fname", fname);
            cmd.Parameters.AddWithValue("@lname", lname);
            cmd.Parameters.AddWithValue("@gender", gender);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet viewexaminers()
        {
            // method returns a list of examiners 
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select Id,Fname,Lname,Email,Image,Status from tbl_users where Id != 1 and IsDeleted = '0'", con);
            sda.Fill(ds);

            //sqldataadapter returns all the examiners with their first name last name email status information
            return ds;
        }
        public int ChangeExaminerStatus(int id)
        {// updating the examiner status
            SqlCommand cmd = new SqlCommand("select Status from tbl_users where id = @id", con);
            
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            string x = cmd.ExecuteScalar().ToString();
            con.Close();

            int status = 0;
            if (x == "0")
            {
                status = 1;
            }
            cmd = new SqlCommand("ChangeExaminerStatus", con);
            cmd.CommandType = CommandType.StoredProcedure;
            // calling the stored procedure change examiner status with parameters as id and status of examiners
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@status", status);
            con.Open();
            int z = cmd.ExecuteNonQuery();
            con.Close();
            return z;
        }
        public int updateexaminer(int id, string email, string fname, string lname)
        {
            
            // update examiner method to update details of examiner like id , email , fname and last name
            SqlCommand cmd = new SqlCommand("UpdateExaminer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@fname", fname);
            cmd.Parameters.AddWithValue("@lname", lname);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int deleteexaminer(int id)
        {
            // method that is used to delete the examiner by their id
            SqlCommand cmd = new SqlCommand("DeleteExaminer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet viewdeletedexaminers()
        {
            // method that is used to display all the list of deleted examiners
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select Id,Fname,Lname,Email,Image,Status from tbl_users where IsDeleted = '1'", con);
            sda.Fill(ds);
            return ds;
        }
        public int restoreexaminer(int id)
        {
            // method that is used to restore the deleted examiner 
            SqlCommand cmd = new SqlCommand("RestoreExaminer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int addexaminer(string fname, string lname, string email, string password, string image, string gender)
        {
            // method that is used to store a new examiner into the database 
            string source = password;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                password = GetHash(sha256Hash, source);
            }
            SqlCommand cmd = new SqlCommand("AddExaminer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fname", fname);
            cmd.Parameters.AddWithValue("@lname", lname);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@image", image);
            cmd.Parameters.AddWithValue("@gender", gender);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int addsubject(string subcode, string subname, string sem, string year)
        {
            // method that is used to add a new subject 
            SqlCommand cmd = new SqlCommand("AddSubject", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@subjectcode", subcode);
            cmd.Parameters.AddWithValue("@subjectname", subname);
            cmd.Parameters.AddWithValue("@sem", sem);
            cmd.Parameters.AddWithValue("@year", year);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet viewsubjects()
        {
            // method that is used to view all the subjects 
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_subjects where IsDeleted = '0'",con);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            return ds;
        }
        public int updatesubject(int id, string subcode, string subname, string sem, string year)
        {
            // updating the subject detials 
            SqlCommand cmd = new SqlCommand("UpdateSubject", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@subcode", subcode);
            cmd.Parameters.AddWithValue("@subname", subname);
            cmd.Parameters.AddWithValue("@sem", sem);
            cmd.Parameters.AddWithValue("@year", year);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int deletesubject(int id)
        {
            // deleting the subject 
            SqlCommand cmd = new SqlCommand("DeleteSubject", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet viewdeletedsubjects()
        {
            // viewing all the subjects that are deleted 
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_subjects where IsDeleted = '1'",con);
            sda.Fill(ds);
            return ds;
        }
        public int restoresubject(int id)
        {
            // restoring the deleted subject
            SqlCommand cmd = new SqlCommand("RestoreSubject", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int givefeedback(int eid,string title, string desc)
        {

        // Method that is used when examiner gives feedback to admin and givefeedback method is used to store it 
            SqlCommand cmd = new SqlCommand("GiveFeedback", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@eid", eid);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@desc", desc);
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet viewfeedback()
        {
            // Method that lists all the feedback given by examiner
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select f.Id, f.Eid, f.Title, f.Feedback, f.IsPinned, f.IsLiked, u.Fname, u.Lname, u.Image from tbl_Feedback f inner join tbl_users u on f.Eid = u.Id order by f.Id desc", con);
            sda.Fill(ds);
            return ds;
        }
        public int pinfeedback(int id)
        {
            // pin the feedback that is good
            SqlCommand cmd = new SqlCommand("PinFeedback", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int deletefeedback(int id)
        {
            // deleting feedback
            SqlCommand cmd = new SqlCommand("DeleteFeedback", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet getExaminerAndSubjects()
        {
            // method to list examiner and their respective assigned subjects
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select Id, Fname, Lname from tbl_users",con);
            sda.Fill(ds,"examiner");
            sda = new SqlDataAdapter("select Id, SubjectCode, SubjectName from tbl_subjects", con);
            sda.Fill(ds, "subjects");
            sda = new SqlDataAdapter("select a.id, a.SubjectId, a.ExaminerId, s.SubjectCode, s.SubjectName, u.Fname, u.Lname from tbl_subjectallocation a inner join tbl_subjects s on a.SubjectId = s.Id inner join tbl_users u on a.ExaminerId = u.Id order by a.Id desc ; ",con);
            sda.Fill(ds, "data");
            return ds;
        }
        public string assignsubject(int eid, int sid)
        {
            // method to assign subject to examiner
            SqlCommand cmd = new SqlCommand("AssignSubject", con);
            cmd.Parameters.AddWithValue("@eid", eid);
            cmd.Parameters.AddWithValue("@sid", sid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            string status = cmd.ExecuteScalar().ToString();
            con.Close();
            return status;
        }
        public string updateassignsubject(int id,int eid, int sid)
        {
            // update assigned subject to examiner
            SqlCommand cmd = new SqlCommand("UpdateAssignSubject", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@eid", eid);
            cmd.Parameters.AddWithValue("@sid", sid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            string status = cmd.ExecuteScalar().ToString();
            con.Close();
            return status;
        }
        public int deleteassignsubject(int id)
        {
            // delete the subject that is already assigned to a examiner
            SqlCommand cmd = new SqlCommand("DeleteAssignSubject", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet getsubjects(int id)
        {
            //method to get list of all subjects and their detials
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select a.SubjectId, s.SubjectCode, s.SubjectName, s.Sem, s.Year from tbl_subjectallocation a " +
                "inner join tbl_subjects s on a.SubjectId = s.Id inner join tbl_users u on a.ExaminerId = u.Id where s.IsDeleted = '0' and u.Id = " + id, con);
            sda.Fill(ds);
            return ds;
        }
        public int checksubject(int eid, int sid)
        {
            SqlCommand cmd = new SqlCommand("CheckSubject", con);
            cmd.Parameters.AddWithValue("@eid", eid);
            cmd.Parameters.AddWithValue("@sid", sid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = (int)cmd.ExecuteScalar();
            con.Close();
            return status;
        }
        public DataSet getunits(int sch)
        {
            // method to get all the list of units 
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_units where SubjectId = "+sch+" and IsDeleted = '0'", con);
            sda.Fill(ds);
            return ds;
        }
        public string getsubjectname(int sid)
        {
            // method to get subject name based on its id 
            SqlCommand cmd = new SqlCommand("GetSubjectName", con);
            cmd.Parameters.AddWithValue("@sid", sid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            string status = cmd.ExecuteScalar().ToString();
            con.Close();
            return status;
        }
        public string getsubjectcode(int sid)
        {
            // method to get subject code 
            SqlCommand cmd = new SqlCommand("GetSubjectCode", con);
            cmd.Parameters.AddWithValue("@sid", sid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            string code = cmd.ExecuteScalar().ToString();
            con.Close();
            return code;
        }
        public string getunitname(int uid)
        {
            // method to get unit name 
            SqlCommand cmd = new SqlCommand("GetUnitName", con);
            cmd.Parameters.AddWithValue("@uid", uid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            string status = cmd.ExecuteScalar().ToString();
            con.Close();
            return status;
        }
        public int addunit(string unitno, string unitname, int sch)
        {
            //method to add a new unit to a subject
            SqlCommand cmd = new SqlCommand("AddUnit", con);
            cmd.Parameters.AddWithValue("@unitno", unitno);
            cmd.Parameters.AddWithValue("@unitname", unitname);
            cmd.Parameters.AddWithValue("@sid", sch);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int updateunit(int uid, int unitno, string unitname)
        {
            // method to update unit 
            SqlCommand cmd = new SqlCommand("UpdateUnit", con);
            cmd.Parameters.AddWithValue("@unitno", unitno);
            cmd.Parameters.AddWithValue("@unitname", unitname);
            cmd.Parameters.AddWithValue("@uid", uid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int deleteunit(int uid)
        {
            //method to delete unit 
            SqlCommand cmd = new SqlCommand("DeleteUnit", con);
            cmd.Parameters.AddWithValue("@uid", uid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet getsubunits(int uid)
        {
            // getting the list of subunits under a unit
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_subunits where UnitId = " + uid + " and IsDeleted = '0'", con);
            sda.Fill(ds);
            return ds;
        }
        public int addsubunit(string suno, string suname, int uid)
        {
            // adding new subunit under a unit
            SqlCommand cmd = new SqlCommand("AddSubUnit", con);
            cmd.Parameters.AddWithValue("@suno", suno);
            cmd.Parameters.AddWithValue("@suname", suname);
            cmd.Parameters.AddWithValue("@uid", uid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int updatesubunit(int suid, int suno, string suname)
        {
            // Update subunit details 
            SqlCommand cmd = new SqlCommand("UpdateSubUnit", con);
            cmd.Parameters.AddWithValue("@suno", suno);
            cmd.Parameters.AddWithValue("@suname", suname);
            cmd.Parameters.AddWithValue("@suid", suid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int deletesubunit(int suid)
        {
            // deleting subunit under a unit 
            SqlCommand cmd = new SqlCommand("DeleteSubUnit", con);
            cmd.Parameters.AddWithValue("@suid", suid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public string getsubunitname(int suid)
        {
            // getting the names of subunits of unit 
            SqlCommand cmd = new SqlCommand("GetSubUnitName", con);
            cmd.Parameters.AddWithValue("@suid", suid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            string status = cmd.ExecuteScalar().ToString();
            con.Close();
            return status;
        }
        public DataSet getquestionbanks(int sid)
        {
            // method to get question banks by subjects unit and subunits
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_units where SubjectId = " + sid + " and IsDeleted = '0'", con);
            sda.Fill(ds,"units");
            sda = new SqlDataAdapter("select s.Id, s.SubUnitNo, s.SubUnitName, s.UnitId from tbl_subunits s inner join tbl_units u on s.UnitId = u.Id where u.SubjectId = " + sid + " and s.IsDeleted = '0'", con);
            sda.Fill(ds, "subunits");
            return ds;
        }
        public int addquestion(string question, string opa, string opb, string opc, string opd, string level, int suid, string ans, string image)
        {
            // Database helper method addquestions to add new questions to a subject 
            SqlCommand cmd = new SqlCommand("AddQuestion", con);
            cmd.Parameters.AddWithValue("@question", question);
            cmd.Parameters.AddWithValue("@opa", opa);
            cmd.Parameters.AddWithValue("@opb", opb);
            cmd.Parameters.AddWithValue("@opc", opc);
            cmd.Parameters.AddWithValue("@opd", opd);
            cmd.Parameters.AddWithValue("@level", level);
            cmd.Parameters.AddWithValue("@ans", ans);
            cmd.Parameters.AddWithValue("@image", image);
            cmd.Parameters.AddWithValue("@suid", suid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int updatequestion(int qid,string question, string opa, string opb, string opc, string opd, string level, string ans)
        {
            // method to update questions
            SqlCommand cmd = new SqlCommand("UpdateQuestion", con);
            cmd.Parameters.AddWithValue("@qid", qid);
            cmd.Parameters.AddWithValue("@question", question);
            cmd.Parameters.AddWithValue("@opa", opa);
            cmd.Parameters.AddWithValue("@opb", opb);
            cmd.Parameters.AddWithValue("@opc", opc);
            cmd.Parameters.AddWithValue("@opd", opd);
            cmd.Parameters.AddWithValue("@level", level);
            cmd.Parameters.AddWithValue("@ans", ans);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet viewquestions(int suid)
        {
            // method to display questions by subject id 
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_questions where Suid = " + suid + " and IsDeleted = '0'", con);
            sda.Fill(ds);
            return ds;
        }
        public int deletequestion(int qid)
        {
            // method to delete questions by questions id 
            SqlCommand cmd = new SqlCommand("DeleteQuestion", con);
            cmd.Parameters.AddWithValue("@qid", qid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet viewpapers(int sid)
        {
            // method to display list of papers by subject id 
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_papers where SubjectId = " + sid + " and IsDeleted = '0' order by Id desc", con);
            sda.Fill(ds);
            return ds;
        }
        public int updateimage(int id, string imgpath)
        {
            // update image method 
            SqlCommand cmd = new SqlCommand("UpdateImage", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@image", imgpath);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataTable GetRandomQuestions(int noq, int suid, int level)
        {

            SqlDataAdapter sda = new SqlDataAdapter("SELECT top "+noq+" * FROM tbl_questions where Suid = "+suid+" and Difficulty = "+level+ " ORDER BY NEWID()", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            return dt;
        }
        public int createpaper(int sid, string date, string title, string duration, int marks, int noq)
        {
            // creating paper with detials of exam duration , exam name , marks and no of questions
            SqlCommand cmd = new SqlCommand("CreatePaper", con);
            cmd.Parameters.AddWithValue("@sid", sid);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@duration", duration);
            cmd.Parameters.AddWithValue("@marks", marks);
            cmd.Parameters.AddWithValue("@noq", noq);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public int getpaperid()
        {
            SqlCommand cmd = new SqlCommand("GetPaperId", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = (int)cmd.ExecuteScalar();
            con.Close();
            return status;
        }
        public void addpaperdata(int pid, int qid)
        {
            SqlCommand cmd = new SqlCommand("AddPaperData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pid", pid);
            cmd.Parameters.AddWithValue("@qid", qid);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
        public DataSet getpaper(int pid)
        {//get paper database helper method 
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select * from tbl_papers where Id = "+pid+" and IsDeleted = '0'", con);
            sda.Fill(ds,"tblpaper");
            sda = new SqlDataAdapter("select q.Question, q.OptionA, q.OptionB, q.OptionC, q.OptionD, q.Image from tbl_data d inner join tbl_questions q on d.QuestionId = q.Id where PaperId =" + pid, con);
            sda.Fill(ds, "tbldata");
            return ds;
        }
        public int deletepaper(int pid)
        {
            SqlCommand cmd = new SqlCommand("DeletePaper", con);
            cmd.Parameters.AddWithValue("@pid", pid);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            int status = cmd.ExecuteNonQuery();
            con.Close();
            return status;
        }
        public DataSet viewpapersubjects()
        {
            // function that returns list of papers by subjects 
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter("select distinct p.SubjectId, s.SubjectName, s.SubjectCode from tbl_papers p inner join tbl_subjects s on p.SubjectId = s.Id where p.IsDeleted = 0", con);
            sda.Fill(ds);
            return ds;
        }
        public static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}