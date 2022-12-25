using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWorkData.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CourseWorkData.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            var totalUserByDoctor = from o in db.DiagnosiS
                                join p in db.Doctors on o.DoctorId equals p.Id
                                group 
                                new
                                {
                                    Uid = o.UserId
                                }                                
                                by new
                                {
                                     p.Id,
                                     p.LastName,
                                     p.FirstName,
                                     p.PatrName
                                }  
                                into grouping
                                select new 
                                { 
                                    Id = grouping.Key.Id, 
                                    Lname = grouping.Key.LastName, 
                                    Fname = grouping.Key.FirstName, 
                                    Pname = grouping.Key.PatrName, 
                                    Total = grouping.Select(a => a.Uid).Count() 
                                };
            var totalUserByDoctorList = totalUserByDoctor.ToList();

            var totalUser = (from diag in db.DiagnosiS
                             join us in db.Users on diag.UserId equals us.Id
                             join dis in db.Diseases on diag.DiseaseId equals dis.Id
                             where dis.Name != "Здоров"
                             group diag.UserId by diag.UserId).Count();

            var totalDisease =  from diag in db.DiagnosiS
                                join dis in db.Diseases on diag.DiseaseId equals dis.Id
                                group new
                                {
                                   Uid = diag.UserId
                                }
                                by new
                                {
                                    diag.DiseaseId,
                                    dis.Name
                                }
                                into grouping
                                select new
                                {
                                    Id = grouping.Key.DiseaseId,
                                    name = grouping.Key.Name,
                                    Total = grouping.Select(a => a.Uid).Count()
                                };


            var totalDiseaseList = totalDisease.ToList();

            List<AppointmentTable> totalAppointment = (
                     from app in db.Appointments
                     join doc in db.Doctors on app.DoctorId equals doc.Id
                     select new AppointmentTable(doc.Id, doc.LastName, doc.FirstName, doc.PatrName, app.Сabinet, app.District, app.Day,
                     app.AppStart.Hour.ToString() + ":" + app.AppStart.Minute.ToString(), app.AppEnd.Hour.ToString() + ":" + app.AppEnd.Minute.ToString())).ToList();

            using (FileStream fs = new FileStream("wwwroot/files/отчет.txt", FileMode.Create))
            {

                byte[] buffer = Encoding.Default.GetBytes("Количество больных в поликлинике:" + totalUser.ToString() + "\n"+ "Количество больных по каждому врачу:\n");
                fs.Write(buffer);
                foreach (var i in totalUserByDoctorList)
                {
                    buffer = Encoding.Default.GetBytes(i.Lname + " " + i.Fname + " " + i.Pname + ": " + i.Total.ToString() + "\n");
                    fs.Write(buffer);
                }
                buffer = Encoding.Default.GetBytes("Количество заболеваний по каждому виду болезней:\n");
                fs.Write(buffer);
                foreach (var i in totalDiseaseList)
                {
                    buffer = Encoding.Default.GetBytes(i.name.ToString() + ": " +  i.Total.ToString() + "\n");
                    fs.Write(buffer);
                }
                buffer = Encoding.Default.GetBytes("Расписание работы врачей:\n");
                fs.Write(buffer);
                foreach (var i in totalAppointment)
                {
                    buffer = Encoding.Default.GetBytes(i.Lname + " " + i.Fname +  " " + i.Pname + "участок: " + i.District + "кабинет: " + i.Cabinet + "время: " + i.Day + " " + i.AppStart + " " + i.AppEnd + "\n");
                    fs.Write(buffer);
                }
                fs.Write(buffer);
            }

            return View();
        }
        [HttpPost]
        public IActionResult CreateFindUser(string Lname, string Fname, string Pname)
        {
            return RedirectToAction("ShowFindUser", "Home", new { Lname = Lname, Fname = Fname, Pname = Pname });
        }
        public IActionResult CreateFindUser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateFindDisease(string name)
        {
            return RedirectToAction("ShowFindDisease", "Home", new { name = name});
        }
        public IActionResult CreateFindDisease()
        {
            return View();
        }
        [HttpPost]
        public  IActionResult CreateFindDoctor(string Lname, string Fname, string Pname)
        {
            return RedirectToAction("ShowFindDoctor", "Home", new { Lname =  Lname, Fname =  Fname, Pname =  Pname });
        }
        public IActionResult CreateFindDoctorByUser(int id)
        {
            var groups = (from user in db.Doctors
                          where (user.Id == id)
                          select user).ToList();
            return RedirectToAction("ShowFindDoctor", "Home", new { Lname = groups[0].LastName, Fname = groups[0].FirstName, Pname = groups[0].PatrName });
        }
        public IActionResult CreateFindDoctor()
        {
            return View();
        }

        public  IActionResult ShowFindUser(string Lname, string Fname, string Pname)
        {
            List<UserTable> groups = (from diag in db.DiagnosiS
                          join user in db.Users on diag.UserId equals user.Id
                          join dis in db.Diseases on diag.DiseaseId equals dis.Id
                          where (user.FirstName == Fname && user.LastName == Lname && user.PatrName == Pname)
                          select new UserTable(user.Adress, dis.Name, user.LastName, user.FirstName, user.PatrName, diag.Date, diag.DoctorId)).ToList();                                   
            ViewBag.Name = groups;
            return View();
        }
        public IActionResult ShowFindUserGetFile(string Address, string Lname, string Fname, string Pname, string Dname, string DateD)
        {
            using (FileStream fs = new FileStream("wwwroot/files/справка.txt", FileMode.Create))
            {
                byte[] buffer = Encoding.Default.GetBytes("Справка: " + Address + " " + Lname + " " +
                    Fname + " " + Pname + " " + Dname + " " + DateD + " поликлиника №0");
                fs.Write(buffer);
            }
            return RedirectToAction("ShowFindUser", new {Lname, Fname, Pname});
        }


        public  IActionResult ShowFindDisease(string name)
        {
            var groups = (from user in db.Diseases
                          where (user.Name == name)
                          select user).ToList();
            ViewBag.Name = groups;
            return View();
        }
        public  IActionResult ShowFindDoctor(string Lname, string Fname, string Pname)
        {
            var groups = (from user in db.Doctors                    
                          where (user.FirstName == Fname && user.LastName == Lname && user.PatrName == Pname)
                          select user).ToList();
            ViewBag.Name = groups;
            return View();
        }
        public async Task<IActionResult> ShowUser(int docId = 0)
        {
            if(docId == 0)
              return View(await db.Users.ToListAsync());
            else
            {
                var innerJoinQuery =(
                    from diag in db.DiagnosiS
                    join user in db.Users on diag.UserId equals user.Id
                    join dis in db.Diseases on diag.DiseaseId equals dis.Id
                    where diag.DoctorId == docId && dis.Name != "Здоров"
                    select user);
                return View(innerJoinQuery.ToList());
            }
        }
        public async Task<IActionResult> ShowDoctor()
        {
            return View(await db.Doctors.ToListAsync());
        }
        public IActionResult ShowSymptom(int Did = 0)
        {
            if(Did == 0) 
            { 
                var groups = (from user in db.Symptoms
                              select user.Name
                       ).Distinct().ToList();
                ViewBag.Name = groups;
                return View();
            }
            else
            {
                var groups = (from user in db.Symptoms
                              where user.DiseaseId == Did
                              select user.Name
                       ).Distinct().ToList();
                ViewBag.Name = groups;
                return View();
            }
        }
        public IActionResult ShowDiseaseForSymptom(string name)
        {
            var d = (from user in db.Symptoms
                       where (name == user.Name && user.DiseaseId != null)
                       select user.DiseaseId).ToList();
            return RedirectToAction("ShowDisease", "Home", new {dis = d });
        }
        public IActionResult ShowMedicine(int Did = 0)
        {
            if (Did == 0)
            {
               var groups = (from user in db.Medicines
                             select user.Name
                      ).Distinct().ToList();
               ViewBag.Name = groups;
               return View();
            }
            else
            {
                var groups = (from user in db.Medicines
                              where user.DiseaseId == Did
                              select user.Name
                       ).Distinct().ToList();
                ViewBag.Name = groups;
                return View();
            }
        }
        public  IActionResult ShowDiseaseForMedicine(string name)
        {
            var d = (from user in db.Medicines
                     where (name == user.Name && user.DiseaseId != null)
                     select user.DiseaseId).ToList();
            return RedirectToAction("ShowDisease", "Home", new { dis = d });
        }
        public IActionResult ShowDisease(List<int> dis, bool all = false)
        {
            var d = (from user in db.Diseases
                     where(dis.Contains(user.Id))
                     select user).ToList();
            if (all == false)        
                return View(d);
            return View(db.Diseases.ToList());
        }
        public IActionResult ShowDiagnosis()
        {
            var dId = (from user in db.DiagnosiS            
                     select user.DiseaseId).ToList();
            List<DiagnosisTable> innerJoinQuery =(
                     from diag in db.DiagnosiS
                     join dis in db.Diseases on diag.DiseaseId equals dis.Id
                     join us in db.Users on diag.UserId equals us.Id
                     join doc in db.Doctors on diag.DoctorId equals doc.Id
                     select new DiagnosisTable(dis.Name, us.LastName, us.FirstName, 
                         us.PatrName, doc.LastName, doc.FirstName, doc.PatrName, diag.Date)).ToList();
            return View(innerJoinQuery.ToList());
        }
        public async Task<IActionResult> ShowMedicineSymptomForDisease()
        {
            return View(await db.Symptoms.ToListAsync());
        }
        public IActionResult ShowAppointment(int id = 0)
        {
            if (id == 0)
            {
                List<AppointmentTable> innerJoinQuery = (
                         from app in db.Appointments
                         join doc in db.Doctors on app.DoctorId equals doc.Id
                         select new AppointmentTable(doc.Id, doc.LastName, doc.FirstName, doc.PatrName, app.Сabinet, app.District, app.Day,
                         app.AppStart.Hour.ToString() + ":" + app.AppStart.Minute.ToString(), app.AppEnd.Hour.ToString() + ":" + app.AppEnd.Minute.ToString())).ToList();
                return View(innerJoinQuery.ToList());
            }
            else
            {
                List<AppointmentTable> innerJoinQuery = (
                from app in db.Appointments
                join doc in db.Doctors on app.DoctorId equals doc.Id
                where app.DoctorId == id
                select new AppointmentTable(doc.Id, doc.LastName, doc.FirstName, doc.PatrName, app.Сabinet, app.District, app.Day,
                app.AppStart.Hour.ToString() + app.AppStart.Minute.ToString(), app.AppEnd.Hour.ToString() + app.AppEnd.Minute.ToString())).ToList();
                return View(innerJoinQuery.ToList());
            }
        }
        [HttpPost]
        [Authorize(Roles = "Doctor, HeadPhysn")] 
        public async Task<IActionResult> CreateDiagnosis(string disId, string usrId, string docId, int value = 0, string disNewId = "")
        {
            int disId1 = Convert.ToInt32(disId);
            int usrId1 = Convert.ToInt32(usrId);
            int docId1 = Convert.ToInt32(docId);
            int disNewId1 = (disNewId != "" ? Convert.ToInt32(docId) : 0);
            if (value == 0)
            {
                var doc = (from user in db.Doctors
                           where (user.Id == docId1)
                           select user).ToList();
                var usr = (from user in db.Users
                           where (user.Id == usrId1)
                           select user).ToList();
                var dis = (from user in db.Diseases
                           where (user.Id == disId1)
                           select user).ToList();
                var date = System.DateTime.UtcNow;
                db.DiagnosiS.Add(new Diagnosis { Disease = dis[0], Doctor = doc[0], User = usr[0], Date = date });
                await db.SaveChangesAsync();
                return RedirectToAction("CreateDiagnosis");
            }
            else
            {              
                return RedirectToAction("UpdateDiagnosis", new { disId1, usrId1, docId1, disNewId1});
            }
        }
        [Authorize(Roles = "HeadPhysn")]
        public async Task<IActionResult> UpdateDiagnosis(int disId1, int usrId1, int docId1, int disNewId1)
        {
            var d = (from diag in db.DiagnosiS
                     where (diag.UserId == usrId1 && diag.DiseaseId == disId1 && diag.DoctorId == docId1)
                     select diag).ToList();
            var date = System.DateTime.UtcNow;
            foreach (var i in d)
                i.DiseaseId = disNewId1;
            await db.SaveChangesAsync();
            return RedirectToAction("CreateDiagnosis");
        }

        [Authorize(Roles = "Doctor, HeadPhysn")]
        public IActionResult CreateDiagnosis()
        {
            var dis = (from user in db.Diseases
                       select user).Distinct().ToList();
            var usr = (from user in db.Users
                       select user).Distinct().ToList();
            var doc = (from user in db.Doctors
                       select user).Distinct().ToList();
            ViewBag.dis = dis;
            ViewBag.usr = usr;
            ViewBag.doc = doc;
            return View();
        }
        [Authorize(Roles = "HeadPhysn")]
        [HttpPost]
        public async Task<IActionResult> CreateAppointment(Appointment app, string id1, string day, string timeEnd, string timeStart)
        {
            if (ModelState["District"].Errors.Count == 0 && ModelState["Сabinet"].Errors.Count == 0)
            {
                int id2 = Convert.ToInt32(id1);
                var d = (from user in db.Doctors
                         where (user.Id == id2)
                         select user).ToList();
                app.Doctor = d[0];
                Enum.TryParse("Active", out CourseWorkData.Models.DayOfWeek myStatus);
                CourseWorkData.Models.DayOfWeek t;
                Enum.TryParse(day, out t);
                var dateStart = new DateTime(2022, 5, 1, Int32.Parse(timeStart.Substring(0, 2)), Int32.Parse(timeStart.Substring(3, 2)), 0);
                var dateEnd = new DateTime(2022, 5, 1, Int32.Parse(timeEnd.Substring(0, 2)), Int32.Parse(timeEnd.Substring(3, 2)), 0);
                app.Day = t;
                app.AppStart = dateStart.ToUniversalTime();
                app.AppEnd = dateEnd.ToUniversalTime();
                db.Appointments.Add(app);
                await db.SaveChangesAsync();
                return RedirectToAction("CreateAppointment");
            }
            var allErrors = "неверный номер кабинета или участка";
            ViewBag.error = allErrors;
            return RedirectToAction("CreateAppointment", new { allErrors });
        }
        [Authorize(Roles = "HeadPhysn")]
        public IActionResult CreateAppointment(string allErrors = "")
        {
            List<AppointmentTable> appTable = (from user in db.Doctors
                       select new AppointmentTable(user.Id, user.LastName, user.FirstName, user.PatrName)).ToList();
            ViewBag.appTable = appTable;
            if (allErrors != "") 
                ViewBag.error = allErrors;
            return View();
        }
        [Authorize(Roles = "HeadPhysn")]
        [HttpPost]
        public async Task<IActionResult> CreateSymptom(Symptom user)
        {
            if (ModelState.IsValid)
            {
                db.Symptoms.Add(user);
                await db.SaveChangesAsync();
                return View();
            }
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            ViewBag.error = allErrors;
            return View();
        }
        [Authorize(Roles = "HeadPhysn")]
        public IActionResult CreateSymptom()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "HeadPhysn")]
        public async Task<IActionResult> CreateMedicine(Medicine user)
        {
            if (ModelState.IsValid)
            {
                db.Medicines.Add(user);
                await db.SaveChangesAsync();
                return View();
            }
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            ViewBag.error = allErrors;
            return View();
        }
        [Authorize(Roles = "HeadPhysn")]
        public IActionResult CreateMedicine()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "HeadPhysn")]
        public async Task<IActionResult> CreateDisease(Disease user, string[] sym, string[] med)
        {
            if (ModelState["Name"].Errors.Count == 0 && sym.Length != 0 && med.Length != 0)
            {
                var query1 =
                (from ord in db.Symptoms
                 where sym.Contains(ord.Name)
                 select ord.Name).Distinct();
                var query2 =
                 (from ord in db.Medicines
                  where med.Contains(ord.Name)
                  select ord.Name).Distinct();
                foreach (string ord in query1)
                    db.Symptoms.Add(new Symptom { Name = ord, Disease = user });
                foreach (string ord in query2)
                    db.Medicines.Add(new Medicine { Name = ord, Disease = user });
                db.Diseases.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("CreateDisease");
            }
            var allErrors = "неправильная длина названия или отсутствуют симптомы или лекарства";
            return RedirectToAction("CreateDisease", new { allErrors });
        }
        [Authorize(Roles = "HeadPhysn")]
        public IActionResult CreateDisease(string allErrors = "")
        {
            var med = (from user in db.Medicines
                       select user.Name).Distinct().ToList();
            var sym = (from user in db.Symptoms
                       select user.Name).Distinct().ToList();
            ViewBag.Sym = sym;
            ViewBag.Med = med;
            if(allErrors != "") 
                ViewBag.error = allErrors;
            return View();
           
        }       
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return View();
            }
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            ViewBag.error = allErrors;
            return View();
        }
        [Authorize(Roles = "HeadPhysn")]
        public IActionResult CreateDoctor()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "HeadPhysn")]
        public async Task<IActionResult> CreateDoctor(Doctor user, int value = 0)
        {
            if (ModelState.IsValid)
            {
                if (value == 0)
                {
                    db.Doctors.Add(user);
                    await db.SaveChangesAsync();
                    return View();
                }
                else
                {
                    var d = (from doc in db.Doctors
                             where doc.FirstName == user.FirstName && doc.LastName == user.LastName && doc.PatrName == user.PatrName
                             select doc).ToList();
                    foreach (var i in d)
                        db.Doctors.Remove(i);
                    await db.SaveChangesAsync();
                    return View();
                }
            }
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            ViewBag.error = allErrors;
            return View();

        }


    }
}