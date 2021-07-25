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
        // GET: examiner
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Subjects()
        {
            DataSet ds = db.getsubjects(Convert.ToInt32(Session["id"]));
            return View(ds);
        }
        [HttpPost]
        public ActionResult Subjects(string sid, string sch)
        {
            if(sid != null)
            {
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
            DataSet ds = db.getquestionbanks(Convert.ToInt32(Session["sid"]));
            return View(ds);
        }
        public ActionResult Units()
        {
            ViewBag.subjectname = db.getsubjectname(Convert.ToInt32(Session["sch"]));
            DataSet ds = db.getunits(Convert.ToInt32(Session["sch"]));
            return View(ds);
        }
        [HttpPost]
        public ActionResult Units(string unitno, string unitname)
        {
            if(unitno != "" && unitname != "" && Session["sch"] != null)
            {
                int x = db.addunit(unitno, unitname, Convert.ToInt32(Session["sch"]));
                if(x == 1)
                {
                    ViewBag.message = "done";
                }
                else
                {
                    ViewBag.message = "Unit already added";
                }
            }
            else
            {
                ViewBag.message = "Empty Form";
            }
            DataSet ds = db.getunits(Convert.ToInt32(Session["sch"]));
            return View(ds);
        }
        [HttpPost]
        public ActionResult UpdateUnit(string uid, string unitno, string unitname)
        {
            if(uid != "" && unitno != "" && unitname != "")
            {
                int x = db.updateunit(Convert.ToInt32(uid), Convert.ToInt32(unitno), unitname);
                if(x == 1)
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
        [HttpPost]
        public ActionResult DeleteUnit(string uid)
        {
            if(uid != null)
            {
                int x = db.deleteunit(Convert.ToInt32(uid));
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
            DataSet ds = new DataSet();
            if (Session["id"] != null)
            {
                Session["uid"] = uid;
                ViewBag.unitname = db.getunitname(Convert.ToInt32(uid));
                ds = db.getsubunits(Convert.ToInt32(uid));
            }
            return View(ds);
        }
        [HttpPost]
        public ActionResult SubUnits(string suno, string suname)
        {

            ViewBag.unitname = db.getunitname(Convert.ToInt32(Session["uid"]));
            if(suno != "" && suname != "")
            {
                int x = db.addsubunit(suno, suname, Convert.ToInt32(Session["uid"]));
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
                ViewBag.message = "Empty Form";
            }
            DataSet ds = db.getsubunits(Convert.ToInt32(Session["uid"]));
            return View(ds);
        }
        [HttpPost]
        public ActionResult UpdateSubUnit(string suid,string suno, string suname)
        {
            if (suid != "" &&suno != "" && suname != "")
            {
                int x = db.updatesubunit(Convert.ToInt32(suid), Convert.ToInt32(suno),suname);
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
        public ActionResult DeleteSubUnit(string suid)
        {
            if (suid != "")
            {
                int x = db.deletesubunit(Convert.ToInt32(suid));
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
            ViewBag.subunitname = db.getsubunitname(Convert.ToInt32(Session["suid"]));
            string imgpath = "";
            int imgflag = 0;
            if (img != null && img.ContentLength > 0)
            {
                string imgname = Path.GetFileName(img.FileName);
                string ext = Path.GetExtension(imgname);
                int filesize = 1024 * 1024 * 2;
                if (ext == ".jpg" || ext == ".png" || ext ==".PNG" || ext == ".jpeg" || ext == ".Jpg" && filesize <= img.ContentLength)
                {
                    DateTime saveNow = DateTime.Now;
                    imgpath = Path.Combine(Server.MapPath("~/Images"), saveNow.ToString("ddMMyyyyHHmmss") + ext);
                    img.SaveAs(imgpath);
                    imgpath = "/Images/" + saveNow.ToString("ddMMyyyyHHmmss") + ext;
                    //int x = db.updateimage(Convert.ToInt32(Session["id"]), imgpath);
                }
                else
                {
                    imgflag = 1;
                    ViewBag.message = "Invalid image type... Please upload .jpg or .png only";
                }
            }
            if (question != "" && opa != "" && opb != "" && opc != "" && opd != "" && level != "" && ans != "" && imgflag == 0)
            {
                int x = db.addquestion(question, opa, opb, opc, opd, level, Convert.ToInt32(Session["suid"]), ans, imgpath);
                if(x == 1)
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
                ViewBag.message = "Please enter details correctly";
            }
            return View();
        }
        public ActionResult ViewQuestions(string suid)
        {
            if(suid != null)
            {
                Session["suid"] = suid;
                ViewBag.subunitname = db.getsubunitname(Convert.ToInt32(Session["suid"]));
                DataSet ds = db.viewquestions(Convert.ToInt32(Session["suid"]));
                return View(ds);
            }
            return View("Dashboard");
        }
        [HttpPost]
        public ActionResult UpdateQuestion(string qid, string question, string optionA, string optionB, string optionC, string optionD, string level, string ans)
        {
            if(qid != "" && question != "" && optionA != "" && optionB != "" && optionC != "" && optionD != "" && level != "" && ans != "")
            {
                int x = db.updatequestion(Convert.ToInt32(qid), question, optionA, optionB, optionC, optionD, level, ans);
                if(x == 1)
                {
                    return Json("done");
                }
                else
                {
                    return Json("Something went wrong");
                }
            }
            return Json("Empty Form");
        }
        public ActionResult DeleteQuestion(string qid)
        {
            if(qid != null)
            {
                int x = db.deletequestion(Convert.ToInt32(qid));
                if (x == 1)
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
        public ActionResult ManageProfile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ManageProfile(string fname, string lname, string gender)
        {
            if(fname != "" && lname != "" && gender != "")
            {
                if(fname.Length <= 20 && lname.Length <= 20 && gender.Length == 1)
                {
                    int x = db.UpdateProfile(Convert.ToInt32(Session["id"]), fname, lname, gender);
                    if( x == 1)
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
        public ActionResult ChangePassword(string cpass, string pass1, string pass2)
        {
            if(cpass != "" && pass1 != "" && pass2 != "")
            {
                int check = db.CheckPassword(Convert.ToInt32(Session["id"]), cpass);
                if(check == 1)
                {
                    if(pass1 == pass2)
                    {
                        int x = db.changepassword(Convert.ToInt32(Session["id"]), pass1);
                        if(x == 1)
                        {
                            return Json("done");
                        }
                        else
                        {
                            return Json("Something went wrong");
                        }
                    }
                    else{
                        return Json("Password and Re-Entered Password does not match");
                    }
                }
                else
                {
                    return Json("Current password does not match");
                }
            }
            return Json("Empty Details");
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
        public ActionResult Papers()
        {
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
            string title = form["title"];
            string duration = form["duration"];
            string total = form["total"];
            string chkid = form["chkid"];
            string noq = form["noq"];
            string date = form["date"];
            string level = form["level"];
            if(title != "" && duration != "" && total != "" && noq != "" && date != "" && level != "")
            {
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
                        /*ArrayList arrqid = new ArrayList();
                        for (int z = 0; z < dt.Rows.Count; z++)
                        {
                            arrqid.Add(dt.Rows[z][0].ToString());
                        }
                        Session["qdata"] = arrqid;*/
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
            //{ title,duration,total,noq,date};
            ArrayList pdata = (ArrayList)Session["tempdata"];
            //try
            //{
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
            //}
            //catch(Exception e)
            //{
            //    //error
            //}
            return RedirectToAction("Papers");
        }
        [HttpPost]
        public ActionResult DeletePaper(string pid)
        {
            if(pid != null)
            {
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
            if (title != "" && desc != "")
            {
                if(title.Length <= 20 && desc.Length <= 500)
                {
                    int x = db.givefeedback(Convert.ToInt32(Session["id"]), title, desc);
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
            Session.Remove("id");
            return RedirectToAction("Login","Home");
        }
    }
}