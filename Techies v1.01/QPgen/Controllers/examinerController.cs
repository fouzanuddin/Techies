using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QPgen.Controllers
{
    public class examinerController : Controller
    {
        databaseHelperController db = new databaseHelperController();
        // creating database helper controller object
        // GET: examiner
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Subjects()
        {
            // Calling the get subjects method from the helper class based on id
            DataSet ds = db.getsubjects(Convert.ToInt32(Session["id"]));
            return View(ds);
        }
        [HttpPost]
        public ActionResult Subjects(string sid, string sch)
        {
            if(sid != null)
            {// check subject method is called for the examiner 
                int x = db.checksubject(Convert.ToInt32(Session["id"]), Convert.ToInt32(sid));
                if (x == 1)
                {
                    Session["sid"] = sid;
                    return RedirectToAction("QuestionBanks", "examiner");
                }
            }
            else if(sch != null)
            {
                int x = db.checksubject(Convert.ToInt32(Session["id"]), Convert.ToInt32(sch));
                if (x == 1)
                {
                    Session["sch"] = sch;
                    return RedirectToAction("Units", "examiner");
                }
            }
            return Json("");
        }
        public ActionResult QuestionBanks()
        {
            // get question banks method is called for the subject id and dataset object is used to display the list of subjects with question banks 
            DataSet ds = db.getquestionbanks(Convert.ToInt32(Session["sid"]));
            return View(ds);
        }
        public ActionResult Units()
        {
            ViewBag.subjectname = db.getsubjectname(Convert.ToInt32(Session["sch"]));
            // getsubject name method is used to get the subjects 
            DataSet ds = db.getunits(Convert.ToInt32(Session["sch"]));
            // Dataset object is used to list all the units under the subject
            return View(ds);
        }
        [HttpPost]
        public ActionResult Units(string unitno, string unitname)
        {
            if(unitno != "" && unitname != "" && Session["sch"] != null)
            {
                int x = db.addunit(unitno, unitname, Convert.ToInt32(Session["sch"]));
                // add unit method is used to add a unit under a subject
                if(x == 1)
                {
                    ViewBag.message = "done";
                    //if success
                }
                else
                {
                    //if unit already exists
                    ViewBag.message = "Unit already added";
                }
            }
            else
            {// if null values exist
                ViewBag.message = "Empty Form";
            }
            DataSet ds = db.getunits(Convert.ToInt32(Session["sch"]));
            // DataSet object is used to list all the units under a subject 
            return View(ds);
        }
        [HttpPost]
        public ActionResult UpdateUnit(string uid, string unitno, string unitname)
        {

            if(uid != "" && unitno != "" && unitname != "")
            {
                int x = db.updateunit(Convert.ToInt32(uid), Convert.ToInt32(unitno), unitname);
                // update unit method under the database helper controller is used to update the details of unit like unit id and unit name 
                if(x == 1)
                {
                    // if success
                    return Json("done");
                }
                else
                {//if error
                    return Json("something went wrong");
                }
            }
            return Json("Empty Form");
            // if the form contains null values 
        }
        [HttpPost]
        public ActionResult DeleteUnit(string uid)
        {
            if(uid != null)
            {
                // delete unit logic
                int x = db.deleteunit(Convert.ToInt32(uid));
                // database helper method delete unit is called with parameter as unit id
                if (x == 1)
                {
                    return Json("done");
                }
                else
                {
                    return Json("something went wrong");
                }
            }
            return Json("Empty Form");
        }
        public ActionResult SubUnits(string uid)
        {
            //Database object is created
            DataSet ds = new DataSet();
            if (Session["id"] != null)
            {
                Session["uid"] = uid;
                ViewBag.unitname = db.getunitname(Convert.ToInt32(uid));
                // for retrieving unit name getunitname method is called with the unit id
                ds = db.getsubunits(Convert.ToInt32(uid));
// all the subunits under the unit id are retrieved using the getsubunits helper method
            }
            return View(ds);
        }
        [HttpPost]
        public ActionResult SubUnits(string suno, string suname)
        {
            // To add subunit
            ViewBag.unitname = db.getunitname(Convert.ToInt32(Session["uid"]));
            // for retrieving the unit name getunitname method is called from the database helper class
            if(suno != "" && suname != "")
            {
                // if subunitid and subunit name is not null
                int x = db.addsubunit(suno, suname, Convert.ToInt32(Session["uid"]));
                // database helper method addsubunit is called with the parameters as subunit id , subunit name and unit id
                if(x == 1)
                {
                    // if success
                    ViewBag.message = "done";
                }
                else
                {
                    // if error
                    ViewBag.message = "Something went wrong!";
                }
            }
            else
            {
                // if the subunit id and subunit name fields are left blank 
                ViewBag.message = "Empty Form";
            }
            DataSet ds = db.getsubunits(Convert.ToInt32(Session["uid"]));
            // getsubunits method is used to list all the subunits under the unit id and dataset object is used to list all the subunits 
            return View(ds);
        }
        [HttpPost]
        public ActionResult UpdateSubUnit(string suid,string suno, string suname)
        {
            // Update subunit logic
            if (suid != "" &&suno != "" && suname != "")
            {// check for null values
                int x = db.updatesubunit(Convert.ToInt32(suid), Convert.ToInt32(suno),suname);
 //update subunit database helper method is used with parameters as subject id , subunit id and subunit name 
                if (x == 1)
                {
                    // if success
                    return Json("done");
                }
                else
                {
                    // if error 
                    return Json("something went wrong");
                }
            }
            // if there are null values 
            return Json("Empty Form");
        }
        public ActionResult DeleteSubUnit(string suid)
        {
            // Delete subunit logic with parameter as subunit id
            if (suid != "")
            {
                int x = db.deletesubunit(Convert.ToInt32(suid));
                // deletesubunit method is called with subunit id 
                if (x == 1)
                {
                    return Json("done");
                }
                else
                {
                    return Json("something went wrong");
                }
            }
            return Json("Empty Form");
        }
        public ActionResult AddQuestion(string suid)
        {
            // Add questions logic
            if(Session["id"] != null)
            {
                
                Session["suid"] = suid;
                ViewBag.subunitname = db.getsubunitname(Convert.ToInt32(Session["suid"]));
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddQuestion(string question, string opa, string opb, string opc, string opd, string level, string ans, HttpPostedFileBase img)
        {
            // addquestion method with parameters like questions options and difficulty level , correct answer and image
            ViewBag.subunitname = db.getsubunitname(Convert.ToInt32(Session["suid"]));
            // getting the subunit name from database helper controller 
            string imgpath = "";
            int imgflag = 0;
            if (img != null && img.ContentLength > 0)
            {
                // checking if the img is uploaded
                string imgname = Path.GetFileName(img.FileName);
                string ext = Path.GetExtension(imgname);
                int filesize = 1024 * 1024 * 2;
                // specifying the file size of image
                if (ext == ".jpg" || ext == ".png" || ext ==".PNG" || ext == ".jpeg" || ext == ".Jpg" && filesize <= img.ContentLength)
                {
                    // checking the extension of the immage and checking the uploaded file size of image
                    DateTime saveNow = DateTime.Now;
                    imgpath = Path.Combine(Server.MapPath("~/Images"), saveNow.ToString("ddMMyyyyHHmmss") + ext);
                    img.SaveAs(imgpath);
                    imgpath = "/Images/" + saveNow.ToString("ddMMyyyyHHmmss") + ext;
                    // saving the image under the image folder in the project directory with the extension of the image
                    //int x = db.updateimage(Convert.ToInt32(Session["id"]), imgpath);
                }
                else
                {
                    // if the file is not jpg or png type error is displayed
                    imgflag = 1;
                    ViewBag.message = "Invalid image type... Please upload .jpg or .png only";
                }
            }
            if (question != "" && opa != "" && opb != "" && opc != "" && opd != "" && level != "" && ans != "" && imgflag == 0)
            {
                // null check is done for the add question 
                int x = db.addquestion(question, opa, opb, opc, opd, level, Convert.ToInt32(Session["suid"]), ans, imgpath);
                // addquestion database helper method is called with all the input parameters to save the question
                
                if(x == 1)
                {
                    // if success
                    ViewBag.message = "done";
                }
                else
                {
                    // if fail
                    ViewBag.message = "Something went wrong";
                }
            }
            else
            {
                // if any field is left blank 
                ViewBag.message = "Please enter details correctly";
            }
            return View();
        }
        public ActionResult ViewQuestions(string suid)
        {
            // View questions under subunit
            if(suid != null)
            {
                Session["suid"] = suid;
                ViewBag.subunitname = db.getsubunitname(Convert.ToInt32(Session["suid"]));
                // getting subunit names 
                DataSet ds = db.viewquestions(Convert.ToInt32(Session["suid"]));
                // listing questions under subunits using view questions method
                return View(ds);
            }
            return View("Dashboard");
        }
        [HttpPost]
        public ActionResult UpdateQuestion(string qid, string question, string optionA, string optionB, string optionC, string optionD, string level, string ans)
        {
            // Update Question logic
            if(qid != "" && question != "" && optionA != "" && optionB != "" && optionC != "" && optionD != "" && level != "" && ans != "")
            {
                // checking for null values 
                int x = db.updatequestion(Convert.ToInt32(qid), question, optionA, optionB, optionC, optionD, level, ans);
               // updating the question using database helper method updatequestion
                if(x == 1)
                {
                    //success
                    return Json("done");
                }
                else
                {
                    //error
                    return Json("Something went wrong");
                }
            }
            // if null values are present
            return Json("Empty Form");
        }
        public ActionResult DeleteQuestion(string qid)
        {
            // Delete Question logic
            if(qid != null)
            {
                // if question id is not null
                int x = db.deletequestion(Convert.ToInt32(qid));

                // delete question helper method is called with question id as parameter
                if (x == 1)
                {
                    // success
                    return Json("done");
                }
                else
                {
                    // fail
                    return Json("Something went wrong");
                }
            }
            // if question id is null
            return Json("error");
        }
        public ActionResult ManageProfile()
        {
            // manage profile without parameter
            return View();
        }
        [HttpPost]
        public ActionResult ManageProfile(string fname, string lname, string gender)
        {
            // Manage profile logic starts here
            if(fname != "" && lname != "" && gender != "")
            {
                // Null Values check
                if(fname.Length <= 20 && lname.Length <= 20 && gender.Length == 1)
                {
                    // checking the length of first name and last name
                    int x = db.UpdateProfile(Convert.ToInt32(Session["id"]), fname, lname, gender);

                    // Update profile based on id using database helper method update profile
                    if( x == 1)
                    {
                        // success
                        ViewBag.message = "done";
                    }
                    else
                    {
                        //error
                        ViewBag.message = "Something went wrong";
                    }
                }
                else
                {
                    // If length of characters exceeds the specified size
                    ViewBag.message = "Error in form submission";
                }
            }
            else
            {
                // if the null values are present in the text fields
                ViewBag.message = "Empty Details";
            }
            return View();
        }
        public ActionResult ChangePassword(string cpass, string pass1, string pass2)
        {
            // Change password logic
            if(cpass != "" && pass1 != "" && pass2 != "")
            {
                // Null Values check
                int check = db.CheckPassword(Convert.ToInt32(Session["id"]), cpass);
                // Checkpassword helper method is called with the email id and old password
                if(check == 1)
                {
                    // if success
                    if(pass1 == pass2)
                    {
                        //if  New password and rentered password are same 
                        int x = db.changepassword(Convert.ToInt32(Session["id"]), pass1);
                        // changepassword helper method is called with id parameter and second parameter as new password

                        if(x == 1)
                        {
                            // if the change password is successful 
                            return Json("done");
                        }
                        else
                        {
                            // if error
                            return Json("Something went wrong");
                        }
                    }
                    else{
                        // if new password and rentered password are not matching
                        return Json("Password and Re-Entered Password does not match");
                    }
                }
                else
                {// if the old password entered doesnt match 
                    return Json("Current password does not match");
                }
            }
            // Null Values are present 
            return Json("Empty Details");
        }
        [HttpPost]
        public ActionResult UpdateImage(HttpPostedFileBase profileimage)
        {
            // Update image logic
            string imgpath = "";
            if (profileimage != null && profileimage.ContentLength > 0)
            {
                string imgname = Path.GetFileName(profileimage.FileName);
                string ext = Path.GetExtension(imgname);
                int filesize = 1024 * 1024 * 2;
                if (ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".Jpg" && filesize <= profileimage.ContentLength)
                {
                    DateTime saveNow = DateTime.Now;
                    imgpath = Path.Combine(Server.MapPath("~/Images"), saveNow.ToString("ddMMyyyyHHmmss") + ext);
                    profileimage.SaveAs(imgpath);
                    imgpath = "/Images/" + saveNow.ToString("ddMMyyyyHHmmss") + ext;
                    int x = db.updateimage(Convert.ToInt32(Session["id"]), imgpath);
                    if (x == 1)
                    {
                        ViewBag.message = "done";
                    }
                    else
                    {
                        ViewBag.message = "Something went wrong";
                    }
                }
                else
                {
                    ViewBag.message = "Invalid image type... Please upload .jpg or .png only";
                }
            }
            else
            {
                ViewBag.message = "No Image Selected";
            }
            return RedirectToAction("ManageProfile");
        }
        public ActionResult Papers()
        {// Dataset object used to list all the subjects 
            DataSet ds = db.getsubjects(Convert.ToInt32(Session["id"]));
            return View(ds);
        }
        [HttpPost]
        public ActionResult Papers(string sid, string sgen)
        {
            if (sid != null)
            {
                int x = db.checksubject(Convert.ToInt32(Session["id"]), Convert.ToInt32(sid));
                if (x == 1)
                {
                    Session["sid"] = sid;
                    return RedirectToAction("ViewPapers", "examiner");
                }
            }
            else if (sgen != null)
            {
                int x = db.checksubject(Convert.ToInt32(Session["id"]), Convert.ToInt32(sgen));
                if (x == 1)
                {
                    Session["sgen"] = sgen;
                    return RedirectToAction("GeneratePaper", "examiner");
                }
            }
            return Json("");
        }
        public ActionResult ViewPapers()
        {
            DataSet ds = new DataSet();
            if (Session["id"] != null)
            {
                 ds = db.viewpapers(Convert.ToInt32(Session["sid"]));
            }
            return View(ds);
        }
        [HttpPost]
        public ActionResult ViewPapers(string btnview)
        {
            if(btnview != null)
            {
                Session["pid"] = btnview;
                return RedirectToAction("DisplayPaper");
            }
            DataSet ds = db.viewpapers(Convert.ToInt32(Session["sid"]));
            return View(ds);
        }
        public ActionResult DisplayPaper()
        {
            if(Session["pid"] != null)
            {
                string pid = Session["pid"].ToString();
                ViewBag.subcode = db.getsubjectcode(Convert.ToInt32(Session["sid"]));
                ViewBag.subname = db.getsubjectname(Convert.ToInt32(Session["sid"]));
                if (pid != "" && pid != null)
                {
                    DataSet ds = db.getpaper(Convert.ToInt32(pid));
                    return View(ds);
                }
            }
            return RedirectToAction("ViewPapers");
        }
        public ActionResult GeneratePaper()
        {
            DataSet ds = db.getquestionbanks(Convert.ToInt32(Session["sgen"]));
            return View(ds);
        }

        [HttpPost]
        public ActionResult GeneratePaper(FormCollection form)
        {
            //Generate paper logic
            string title = form["title"];
            string duration = form["duration"];
            string total = form["total"];
            string chkid = form["chkid"];
            string noq = form["noq"];
            string date = form["date"];
            string level = form["level"];
            if(title != "" && duration != "" && total != "" && noq != "" && date != "" && level != "")
            {
                // Null check is done for details like title of exam , duration , total , no of questions , date and level
                if(chkid != null)
                {
                    string[] subunitid = chkid.Split(',');
                    float topics = subunitid.Length;
                    float noqint = Convert.ToInt32(noq);
                    float calculate = noqint / topics;
                    double qpert = Math.Ceiling(calculate); // no of questions per topic
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    
                    for (int i = 0; i < subunitid.Length; i++)
                    {
                        dt = db.GetRandomQuestions(Convert.ToInt32(qpert), Convert.ToInt32(subunitid[i]), Convert.ToInt32(level));
                        if(dt.Rows.Count != qpert)
                        {
                            ViewBag.message = "Question Bank does not have enough questions";
                            DataSet ds2 = db.getquestionbanks(Convert.ToInt32(Session["sgen"]));
                            return View(ds2);
                        }
                       
                        ds.Tables.Add(dt);
                    }

                    // Paper preview mode
                    ViewBag.noq = noq;
                    ViewBag.duration = duration;
                    ViewBag.qptitle = title;
                    ViewBag.total = total;
                    ViewBag.date = date;
                    ViewBag.subjectname = db.getsubjectname(Convert.ToInt32(Session["sgen"]));
                    ViewBag.subcode = db.getsubjectcode(Convert.ToInt32(Session["sgen"]));
                    ArrayList data = new ArrayList();
                    //{ title,duration,total,noq,date};
                    data.Add(title);
                    data.Add(duration);
                    data.Add(total);
                    data.Add(noq);
                    data.Add(date);
                    Session["tempdata"] = data;
                    return View("PaperPreview",ds);
                }
                else
                {
                    ViewBag.message = "No Sub Unit selected";
                }
            }
            else
            {
                ViewBag.message = "Empty form";
            }
            DataSet dst = db.getquestionbanks(Convert.ToInt32(Session["sgen"]));
            return View(dst);
        }
        public ActionResult PaperPreview()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PaperPreview(string btnsave)
        {
          // paper preview after save button
            ArrayList pdata = (ArrayList)Session["tempdata"];
            
            int x1 = db.createpaper(Convert.ToInt32(Session["sgen"]), pdata[4].ToString(), pdata[0].ToString(), pdata[1].ToString(),Convert.ToInt32(pdata[2]), Convert.ToInt32(pdata[3]));
                if(x1 == 1)
                {
                    int paperid = db.getpaperid();
                    ArrayList qdata = (ArrayList)Session["qdata"];
                    for(int i = 0; i < qdata.Count; i++)
                    {
                        db.addpaperdata(paperid,Convert.ToInt32(qdata[i]));
                    }
                }
           
            return RedirectToAction("Papers");
        }
        [HttpPost]
        public ActionResult DeletePaper(string pid)
        {
            if(pid != null)
            {
                //if paper id is not null database helper method delete paper is called with paper id as parameter
                int x = db.deletepaper(Convert.ToInt32(pid));
                if(x == 1)
                {
                    return Json("done");
                }
                else
                {
                    return Json("Something went wrong");
                }
            }
            return Json("error");
        }
        public ActionResult Feedback()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Feedback(string title, string desc)
        {
            // feedback logic 
            if (title != "" && desc != "")
            {
                // checking for null values 
                if(title.Length <= 20 && desc.Length <= 500)
                {
                    // length check
                    int x = db.givefeedback(Convert.ToInt32(Session["id"]), title, desc);
                    // givefeedback method is called with title and description of feedback is passed as parameter
                    if(x == 1)
                    {
                        ViewBag.message = "done";
                    }
                    else
                    {
                        ViewBag.message = "Something went wrong!";
                    }
                }
                else
                {
                    ViewBag.message = "Invalid Input";
                }
            }
            else
            {
                ViewBag.message = "Empty Feedback";
            }
            return View();
        }
        public ActionResult Logout()
        {
            // Session logout logic 
            Session.Remove("id");
            //redirecting the page to login page
            return RedirectToAction("Login","Home");
        }
    }
}