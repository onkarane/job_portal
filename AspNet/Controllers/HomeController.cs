using CIS655Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CIS665Project.Models;
using Microsoft.AspNetCore.Http;

namespace CIS655Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly Team116dbContext _context;

        public HomeController(Team116dbContext context)
        {
            _context = context;
        }


        //home page
        [Authorize(Roles = "User")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SearchResults()
        {
            var jobs = _context.Jobs.Where(c => c.JobCatId == 0);


            //if only category is searched for
            if (Search.StaticCategory != 0 && String.IsNullOrEmpty(Search.StaticWildCard))
            {
                jobs = _context.Jobs.Where(c => c.JobCatId == Search.StaticCategory);

            }
            //if only title is searched for
            else if (Search.StaticCategory == 0 && !String.IsNullOrEmpty(Search.StaticWildCard))
            {
                jobs = _context.Jobs.Where(c => c.JobTitle.Contains(Search.StaticWildCard));

            }
            else if (Search.StaticCategory != 0 && !String.IsNullOrEmpty(Search.StaticWildCard))
            {
                jobs = _context.Jobs
                   .Where(c => c.JobCatId == Search.StaticCategory && c.JobTitle.Contains(Search.StaticWildCard));

            }

            return View(jobs.ToList());
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult SearchResults(Search search)
        {
            var jobs = _context.Jobs.Where(c => c.JobCatId == 0);


            //if only category is searched for
            if (search.Category != 0 && String.IsNullOrEmpty(search.WildCard))
            {
                jobs = _context.Jobs.Where(c => c.JobCatId == search.Category);

            }
            //if only title is searched for
            else if (search.Category == 0 && !String.IsNullOrEmpty(search.WildCard))
            {
                jobs = _context.Jobs.Where(c => c.JobTitle.Contains(search.WildCard));

            }
            else if (search.Category != 0 && !String.IsNullOrEmpty(search.WildCard))
            {
                jobs = _context.Jobs
                   .Where(c => c.JobCatId == search.Category && c.JobTitle.Contains(search.WildCard));

            }

            //save searches
            //HttpContext.Session.SetString("Category", search?.Category.ToString());
            //HttpContext.Session.SetString("Wilcard", search?.WildCard);
            Search.StaticCategory = search.Category;
            Search.StaticWildCard = search.WildCard;

            return View(jobs.ToList());
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Apply(int? id, JobApplication application, Search search)
        {
            //get the user id
            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            application.UserId = userPK;

            //get the job id
            if (id != null)
            {
                application.JobId = id;
            }
            else
            {
                return RedirectToAction(nameof(Applications));
            }

            //check if the job is already applied to
            var applied = await _context.JobApplications.FirstOrDefaultAsync(j => j.JobId == id && j.UserId == userPK);

            if (applied != null)
            {
                TempData["AppError"] = "Job Already Applied!";
                return RedirectToAction(nameof(SearchResults));
            }

            _context.Add(application);
            await _context.SaveChangesAsync();
            TempData["App"] = "Job Application Successful!";

            return RedirectToAction(nameof(Applications));

        }

        //my profile
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyProfile()
        {
            //get the user id
            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            //get the user details
            var users = await _context.Users.FindAsync(userPK);

            return View(users);

        }

        //edit my profile
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            //get the user id
            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            user.UserId = userPK;

            //check if the email is registered
            var email = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.UserId != userPK);

            if (email != null)
            {
                TempData["updateFail"] = $"Email Already Registered!";
                return RedirectToAction(nameof(MyProfile));
            }

            //try catch update
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["updateFail"] = $"Profile update failed!";
                return View(user);
            }

            TempData["edit"] = "Account updated successfully!";

            return RedirectToAction(nameof(MyProfile));

        }

        //my applications
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Applications(JobApplication application, Job job)
        {

            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            var jobs = _context.JobApplications
                .Include(a => a.Job)
                .Where(b => b.UserId == userPK);

            return View(await jobs.ToListAsync());

        }

        //withdraw applications
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Withdraw(int? id, JobApplication application)
        {
            int userPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            if (id == null)
            {
                return RedirectToAction(nameof(Applications));
            }

            var jobApp = await _context.JobApplications.FirstOrDefaultAsync(j => j.JobId == id && j.UserId == userPK);

            try
            {
                _context.JobApplications.Remove(jobApp);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["deletefail"] = $"Application withdrawal failed!";
                return RedirectToAction(nameof(Applications));
            }

            TempData["delete"] = $"Application withdrawn successfully!";

            return RedirectToAction(nameof(Applications));

        }


        //BUSINESS PART START
        //home page
        [Authorize(Roles = "Business")]
        public IActionResult BIndex()
        {
            return View();
        }

        //post jobs
        [Authorize(Roles = "Business")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostJobs(Job job)
        {
            int busPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            job.BusId = busPK;

            if (job.JobCatId == 0)
            {
                TempData["jobPostfail"] = $"Please select job Category!";
                return RedirectToAction(nameof(BIndex));
               
            }

            try
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["jobPostfail"] = $"Job Post failed!";
                return RedirectToAction(nameof(BIndex), job);
            }

            TempData["jobPost"] = "Job Post SuccessFul!";

            return RedirectToAction(nameof(JobPostings));

        }

        [Authorize(Roles = "Business")]
        public async Task<IActionResult> JobPostings(JobApplication application, Job job)
        {

            int busPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            var jobs = _context.Jobs
                .Where(b => b.BusId == busPK);

            return View(await jobs.ToListAsync());

        }

        [Authorize(Roles = "Business")]
        public async Task<IActionResult> JobDelete(int? id, Job job)
        {
            int busPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            if (id == null)
            {
                return RedirectToAction(nameof(JobPostings));
            }

            var jobPost = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == id && j.BusId == busPK);

            try
            {
                _context.Jobs.Remove(jobPost);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["deletefail"] = $"Job Delete failed!";
                return RedirectToAction(nameof(JobPostings));
            }

            TempData["delete"] = $"Job Deleted successfully!";

            return RedirectToAction(nameof(JobPostings));

        }

        [Authorize(Roles = "Business")]
        public async Task<IActionResult> JobProfile(int id)
        {
            //get the user id
            int busPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);

            if (id == null)
            {
                return RedirectToAction(nameof(JobPostings));
            }

            var jobPost = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == id && j.BusId == busPK);

            return View(jobPost);

        }

        [Authorize(Roles = "Business")]
        public async Task<IActionResult> JobEdit(Job job)
        {
            //get the user id
            int busPK = Int32.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value);
            job.BusId = busPK;

            //try catch update
            try
            {
                _context.Update(job);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["updateFail"] = $"Job update failed!";
                return RedirectToAction(nameof(JobPostings));
            }

            TempData["edit"] = "Account updated successfully!";

            return RedirectToAction(nameof(JobPostings));

        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
