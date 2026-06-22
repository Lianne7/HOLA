using HOLA.Data;
using HOLA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HOLA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var model = new LandingPageViewModel
            {
                TotalStudents = 10667,
                TreesPlanted = 6700,
                CO2Saved = 6700
            };

            return View(model);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
        }

        [Authorize]
        public IActionResult Forum()
        {
            return View();
        }

        [Authorize]
        public IActionResult Forest()
        {
            // Trees are handled by frontend Javascript now! We don't need backend ViewBags yet.
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var currentUserId = _userManager.GetUserId(User);

            var posts = await _context.Posts
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var formattedPosts = posts.Select(p => new
            {
                id = p.Id,
                board = p.Board ?? "general",
                author = p.AuthorName ?? "Student",
                title = p.Title,
                excerpt = p.Content,
                tags = string.IsNullOrEmpty(p.Tags)
                    ? new string[] { "discussion" }
                    : p.Tags.Split(','),
                votes = 1,
                comments = 0,
                time = p.CreatedAt.ToString("MMM dd, h:mm tt"),
                isOwn = p.AuthorId == currentUserId
            });

            return Json(formattedPosts);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            var post = new Post
            {
                Title = request.Title,
                Content = request.Content,
                Board = request.Board,
                Tags = request.Tags,
                AuthorId = user.Id,
                AuthorName = user.UserName?.Split('@')[0] ?? "User",
                CreatedAt = DateTime.Now
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditPost(int id, [FromBody] EditPostRequest request)
        {
            var post = await _context.Posts.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            if (post == null)
            {
                return Json(new { success = false, message = "Post not found." });
            }

            if (post.AuthorId != userId)
            {
                return Json(new { success = false, message = "Unauthorized." });
            }

            post.Title = request.Title;
            post.Content = request.Content;
            post.Board = request.Board;
            post.Tags = request.Tags;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            var userId = _userManager.GetUserId(User);

            if (post == null)
            {
                return Json(new { success = false, message = "Post not found." });
            }

            if (post.AuthorId != userId)
            {
                return Json(new { success = false, message = "Unauthorized." });
            }

            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }

    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Board { get; set; }
        public string Tags { get; set; }
    }

    public class EditPostRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Board { get; set; }
        public string Tags { get; set; }
    }
}

