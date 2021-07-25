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

namespace QPgen.Controllers
{
    public class adminController : Controller
    {
        databaseHelperController db = new databaseHelperController();
        // GET: admin
        public ActionResult Dashboard()
        {
            string[] data = db.getinfo();
            ViewData["data"] = data;
            return View();
        }
        public ActionResult ViewExaminers()
        {
            DataSet ds = db.viewexaminers();    
            return View(ds);
        }
        [HttpPost]
        public ActionResult ChangeExaminerStatus(int chid)
        {
            int x = db.ChangeExaminerStatus(chid);
            if(x == 1)
            {
                return Json("done");
            }
            return Json("error");
        }
        [HttpPost]
        public ActionResult UpdateExaminer(int id, string email, string fname, string lname)
        {
            if (id != 0 && email != "" && fname != "" && lname != "")
            {
                int x = db.updateexaminer(id,email,fname,lname);
                if(x == 1)
                {
                    return Json("done");
                }
                else
                {
                    return Json("Error, Something went wrong");
                }
            }
            else
            {
                return Json("Invalid Details, Error in Updation");
            }
        }
        [HttpPost]
        public ActionResult DeleteExaminer(int id)
        {
            int x = db.deleteexaminer(id);
            if (x == 1)
            {
                return Json("done");
            }
            return Json("error");
        }
        public ActionResult ViewDeletedExaminers()
        {
            DataSet ds = db.viewdeletedexaminers();
            return View(ds);
        }
        [HttpPost]
        public ActionResult RestoreExaminer(int id)
        {
            int x = db.restoreexaminer(id);
            if (x == 1)
            {
                return Json("done");
            }
            return Json("error");
        }
        public ActionResult AddExaminer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddExaminer(Examiner e, string gender, HttpPostedFileBase profileimage)
        {
            int imgflag = 0;
            string imgpath = "";
            if (profileimage != null && profileimage.ContentLength > 0)
            {
                
                
                string imgname = Path.GetFileName(profileimage.FileName);
                string ext = Path.GetExtension(imgname);
                int filesize = 1024 * 1024 * 2 ;
                if(ext == ".jpg" || ext == ".png" || ext == ".jpeg" || ext == ".Jpg" && filesize <= profileimage.ContentLength)
                {
                    DateTime saveNow = DateTime.Now;
                    imgpath = Path.Combine(Server.MapPath("~/Images"), saveNow.ToString("ddMMyyyyHHmmss") + ext);
                    profileimage.SaveAs(imgpath);
                    imgpath = "/Images/" + saveNow.ToString("ddMMyyyyHHmmss") + ext;
                    imgflag = 1;
                }
                else
                {
                    ViewBag.message = "Please upload jpg or png images under 2 MB only";
                }
            }
            else
            {
                imgpath = "/Images/avatar5.png";
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
                int x = db.addexaminer(e.FirstName, e.LastName, e.Email, e.Pass1, imgpath, gender);
                if (x == 1)
                {
                    ViewBag.message = "done";
                }
                else
                {
                    ViewBag.message = "error";
                }
                return View();
            }
            ViewBag.message = "error";
            return View();
        }
        public ActionResult AddSubject()
        {
            DataSet ds = db.viewsubjects();
            return View(ds);
        }
        [HttpPost]
        public ActionResult AddSubject(string Subjectcode, string Subjectname, string sem, string year)
        {
            if(Subjectcode != "" && Subjectname != "" && sem != "" && year != "")
            {
                int x = db.addsubject(Subjectcode, Subjectname, sem, year);
                if(x == 1)
                {
                    ViewBag.message = "done";
                }
                else
                {
                    ViewBag.message = "Something went wrong... Please try again later";
                }
            }
            DataSet ds = db.viewsubjects();
            return View(ds);
        }
        [HttpPost]
        public ActionResult UpdateSubject(int id,string subcode, string subname, string sem, string year)
        {
            if(id.ToString() != "" && subcode != "" && subname != "" && sem != "" && year != "")
            {
                int x = db.updatesubject(id, subcode, subname, sem, year);
                if(x == 1)
                {
                    return Json("done");
                }
                else
                {
                    return Json("Something went wrong!");
                }
            }
            return Json("error!");
        }
        [HttpPost]
        public ActionResult DeleteSubject(int id)
        {
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
            DataSet ds = db.viewdeletedsubjects();
            return View(ds);
        }
        [HttpPost]
        public ActionResult RestoreSubject(int id)
        {
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
            DataSet ds = db.getExaminerAndSubjects();
            return View(ds);
        }
        [HttpPost]
        public ActionResult AssignSubject(string selectexaminer, string selectsubject)
        {
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
            int x = db.deleteassignsubject(id);
            if (x == 1)
            {
                return Json("done");
            }
            return Json("error");
        }
        public ActionResult Papers()
        {
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
        public ActionResult ManageProfile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ManageProfile(string fname, string lname, string gender)
        {
            if (fname != "" && lname != "" && gender != "")
            {
                if (fname.Length <= 20 && lname.Length <= 20 && gender.Length == 1)
                {
                    int x = db.UpdateProfile(Convert.ToInt32(Session["id"]), fname, lname, gender);
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
            Session.Remove("id");
            return RedirectToAction("Login", "Home");
        }
    }
}