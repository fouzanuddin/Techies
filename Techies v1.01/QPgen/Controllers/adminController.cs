using QPgen.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Razor.Generator;

// Techies Admin Controller Logic 
namespace QPgen.Controllers
{
    public class adminController : Controller
    {
        databaseHelperController db = new databaseHelperController();
        // GET: admin
     // declaring database helper object 
        public ActionResult ViewExaminers()
        {
            DataSet ds = db.viewexaminers();    
            return View(ds);
        }
        
        public ActionResult Dashboard()
        {
            string[] data = db.getinfo();
            //Calling db.getinfo() method from database helper class to display count of examiners , question papers and questions 
            ViewData["data"] = data;
            return View();
        }

        [HttpPost]
        public ActionResult ChangeExaminerStatus(int chid)
        {
            //calling change examiner status method from the helper class
            int x = db.ChangeExaminerStatus(chid);
            if(x == 1)
            {
                return Json("done");
            }
            return Json("error");
        }

        [HttpPost]
        public ActionResult DeleteExaminer(int id)
        {
            //calling delete examiner method from database helper class 
            int x = db.deleteexaminer(id);
            if (x == 1)
            {
                return Json("done");
            }
            return Json("error");
        }
        [HttpPost]
        public ActionResult UpdateExaminer(int id, string email, string fname, string lname)
        {
            
            if (id != 0 && email != "" && fname != "" && lname != "")
            {//checking for null values and calling update examiner method from the database helper class with the parameters as id, email , first name and last name 
                int x = db.updateexaminer(id, email, fname, lname);
                if (x == 1)
                {
                    return Json("done");
                    //if the function call returns 1 then json Done message is displayed
                }
                else
                {
                    return Json("Error, Something went wrong");
                    //if the function call fails then error message is sent 
                }
            }
            else
            {
                return Json("Invalid Details, Error in Updation");
                // if any of the values are null then the following message is displayed
            }
        }
        public ActionResult ViewDeletedExaminers()
        {
            // view deleted examiners method is called from the helper class and Dataset is used to display the list 
            DataSet ds = db.viewdeletedexaminers();
            return View(ds);
        }
        [HttpPost]
        public ActionResult RestoreExaminer(int id)
        {
            //To restore the deleted examiner database helper calls the restore examiner method with the parameter as id of the examiner
            int x = db.restoreexaminer(id);
            if (x == 1)
            {
               //if the function call is successful the it returns 1 and json message done is displayed
                return Json("done");
            }
            //if the function returns anything else other than 1 then json error message is displayed
            return Json("error");
        }
        public ActionResult AddExaminer()
        {
            //when add examiner is used without the parameters it just returns view
            return View();
        }
        [HttpPost]
        public ActionResult AddExaminer(Examiner e, string gender, HttpPostedFileBase profileimage)
        {
            //Add Examiner menthod with parameters as Examiner object , gender and profile image
            int imgflag = 0;
            string imgpath = "";
            if (profileimage != null && profileimage.ContentLength > 0)
            {
                //Storing of the uploaded image when adding the examiner
                
                
                string imgname = Path.GetFileName(profileimage.FileName);

                //getting the image file name in the imgname string variable

                string ext = Path.GetExtension(imgname);

                //getting the image extensions if jpeg, png, jpg
                int filesize = 1024 * 1024 * 2 ;
                // Maximum size of the image is specified in the filesize 
                if(ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".Jpg" && filesize <= profileimage.ContentLength)
                {
                    // checking if the image is of the following formats jpg,jpeg,png,Jpg and checking if the uploaded image is less than or equal to the filesize variable that is 2 mb
                    DateTime saveNow = DateTime.Now;
                    imgpath = Path.Combine(Server.MapPath("~/Images"), saveNow.ToString("ddMMyyyyHHmmss") + ext);
                    profileimage.SaveAs(imgpath);
                    imgpath = "/Images/" + saveNow.ToString("ddMMyyyyHHmmss") + ext;

                    //Storing the uploaded image in the Images directory in the project folder with appended date and extension
                    imgflag = 1;
                }
                else
                {
                    ViewBag.message = "Please upload jpg or png images under 2 MB only";

                    // else message is displayed if the file uploaded is other than png, jpg , jpeg, JPG  or the file size of the image is greater than 2 mb
                }
            }
            else
            {
                imgpath = "/Images/avatar5.png";
// if the image is not uploaded then default avatar5.png is used for the added examiner
                imgflag = 1;
            }
            if (gender == "male")
            {
                gender = "m";
            }
            else
            {
                gender = "f";
            }
            if (ModelState.IsValid && imgflag == 1)
            {
                // Database helper method is called with Examiner objects and imgpath and gender 
                int x = db.addexaminer(e.FirstName, e.LastName, e.Email, e.Pass1, imgpath, gender);
                if (x == 1)
                {
                    ViewBag.message = "done";
                    //if the function is successful then it returns 1 and message is updated as Done 
                }
                else
                {
                    ViewBag.message = "error";
                    // else the message error is updated
                }
                return View();
            }
            ViewBag.message = "error";
            return View();
        }
        public ActionResult AddSubject()
        {
            // Database helper method view subjects is used to display list of all the subjects
            // Dataset object is used to display the list 
            DataSet ds = db.viewsubjects();
            return View(ds);
        }
        [HttpPost]
        public ActionResult AddSubject(string Subjectcode, string Subjectname, string sem, string year)
        {
            // Method addsubject when it is used with parameters database helper method addsubcject is called to store the new subject details.
            if(Subjectcode != "" && Subjectname != "" && sem != "" && year != "")
            {
                int x = db.addsubject(Subjectcode, Subjectname, sem, year);
                if(x == 1)
                {
                    ViewBag.message = "done";
                    // if success 
                }
                else
                {
                    //else message
                    ViewBag.message = "Something went wrong... Please try again later";
                }
            }
            DataSet ds = db.viewsubjects();
            //dataset object is used to display all the subjects after addition of the new subject
            return View(ds);
        }
        [HttpPost]
        public ActionResult UpdateSubject(int id,string subcode, string subname, string sem, string year)
        {
            // Update Subject method with subject parameters 
            if(id.ToString() != "" && subcode != "" && subname != "" && sem != "" && year != "")
            {
                // checking for null values 

                int x = db.updatesubject(id, subcode, subname, sem, year);
                // updatesubject helper method with subject parameters
                if(x == 1)
                {
                    return Json("done");
                    //success message 
                }
                else
                {
                    //error message
                    return Json("Something went wrong!");
                }
            }
            return Json("error!");
        }
        [HttpPost]
        public ActionResult DeleteSubject(int id)
        {
            // Delete subject method with id parameter
            int x = db.deletesubject(id);
            if (x == 1)
            {
                return Json("done");
            }
            else
            {
                return Json("Something went wrong!");
            }   
        }
        public ActionResult ViewDeletedSubjects()
        {
            // View deleted subject method is called and Dataset object is used to display the deleted subjects
            DataSet ds = db.viewdeletedsubjects();
            return View(ds);
        }
        [HttpPost]
        public ActionResult RestoreSubject(int id)
        {
            //restoring the deleted subject 
            int x = db.restoresubject(id);
            if (x == 1)
            {
                return Json("done");
            }
            else
            {
                return Json("Something went wrong!");
            }
        }
        public ActionResult ViewFeedback()
        {
            //View feedback method from the database helper is called
            DataSet ds = db.viewfeedback();
            return View(ds);
        }
        [HttpPost]
        public ActionResult PinFeedback(int id)
        {
            int x = db.pinfeedback(id);
            if(x == 1)
            {
                return Json("done");
            }
            return Json("error");
        }
        [HttpPost]
        public ActionResult DeleteFeedback(int id)
        {
            int x = db.deletefeedback(id);
            if (x == 1)
            {
                return Json("done");
            }
            return Json("error");
        }
        public ActionResult AssignSubject()
        {
            // AssignSubject method with no parameters returns Examiners with their assigneed subjects 
            DataSet ds = db.getExaminerAndSubjects();
            return View(ds);
        }
        [HttpPost]
        public ActionResult AssignSubject(string selectexaminer, string selectsubject)
        {
            //assigning Examiner to a subject 
            if(selectexaminer != "" && selectsubject != "")
            {
                string x = db.assignsubject(Convert.ToInt32(selectexaminer), Convert.ToInt32(selectsubject));

                if (x == "done")
                {
                    ViewBag.message = "done";
                }
                else if(x == "error")
                {
                    ViewBag.message = "already exists";
                }
            }
            else
            {
                ViewBag.message = "error";
            }
            DataSet ds = db.getExaminerAndSubjects();

// Displaying examiners with their assigned subjects 
            return View(ds);
        }
        [HttpPost]
        public ActionResult UpdateAssignSubject(string id,string eid, string sid)
        {
            if (id != "" && eid != "" && sid != "")
            {
                string x = db.updateassignsubject(Convert.ToInt32(id),Convert.ToInt32(eid), Convert.ToInt32(sid));
                if (x == "done")
                {
                    ViewBag.message = "done";
                }
                else if (x == "error")
                {
                    ViewBag.message = "already exists";
                }
            }
            else
            {
                ViewBag.message = "error";
            }
            return Json("done");
        }
        [HttpPost]
        public ActionResult DeleteAssignSubject(int id)
        {
            // Deleting the subject that was assigned to examiner
            int x = db.deleteassignsubject(id);
            if (x == 1)
            {
                return Json("done");
            }
            return Json("error");
        }
        public ActionResult Papers()
        {
            // view papers by subjects
            DataSet ds = db.viewpapersubjects();
            return View(ds);
        }
        [HttpPost]
        public ActionResult Papers(string sid)
        {
            if (sid != null)
            {
                Session["sid"] = sid;
                return RedirectToAction("ViewPapers");
            }
            DataSet ds = db.viewpapersubjects();
            return View();
        }
        public ActionResult ViewPapers()
        {
            DataSet ds = db.viewpapers(Convert.ToInt32(Session["sid"]));
            return View(ds);
        }
        [HttpPost]
        public ActionResult ViewPapers(string pid)
        {
            if(pid != null)
            {
                Session["pid"] = pid;
                return RedirectToAction("DisplayPaper");
            }
            DataSet ds = db.viewpapers(Convert.ToInt32(Session["sid"]));
            return View();
        }
        public ActionResult DisplayPaper()
        {
            // Display Papers method 
            if(Session["pid"] != null)
            {
                string pid = Session["pid"].ToString();
                ViewBag.subcode = db.getsubjectcode(Convert.ToInt32(Session["sid"]));
                ViewBag.subname = db.getsubjectname(Convert.ToInt32(Session["sid"]));
                if (pid != "" && pid != null)
                {
                    // Dataset object is used to display the papers returned by the db.getpaper method 
                    DataSet ds = db.getpaper(Convert.ToInt32(pid));
                    return View(ds);
                }
            }
            return RedirectToAction("ViewPapers");
        }
        public ActionResult ManageProfile()
        {
            //Manage profile method without parameters
            return View();
        }
        [HttpPost]
        public ActionResult ManageProfile(string fname, string lname, string gender)
        {
            // Manage profile method to update the admin first name last name and gender

            if (fname != "" && lname != "" && gender != "")
            {
                if (fname.Length <= 20 && lname.Length <= 20 && gender.Length == 1)
                {
                    int x = db.UpdateProfile(Convert.ToInt32(Session["id"]), fname, lname, gender);
                    // Database helper method Update Profile is used to update the details of Admi
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
                    ViewBag.message = "Error in form submission";
                }
            }
            else
            {
                ViewBag.message = "Empty Details";
            }
            return View();
        }
        [HttpPost]
        public ActionResult UpdateImage(HttpPostedFileBase profileimage)
        {
            // updating image method to update image of Admin
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
        public ActionResult Logout()
        {
            //Ending the session and redirecting the page to Login screen
            Session.Remove("id");
            return RedirectToAction("Login", "Home");
        }
    }
}