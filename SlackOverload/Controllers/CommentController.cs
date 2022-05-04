using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SlackOverload.Data;
using SlackOverload.Models;

namespace SlackOverload.Controllers {
    [Authorize]
    public class CommentController : Controller {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index() {
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> QuestionComment(int questionId) {
            ViewBag.QuestionId = questionId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuestionComment([Bind("Body,QuestionId")] QuestionComment questionComment) {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            Question? question = await _context.Question.FindAsync(questionComment.QuestionId);

            if (user == null) {
                return NotFound();
            }

            QuestionComment newComment = new QuestionComment(questionComment.Body, user, question);

            try {
                await _context.QuestionComment.AddAsync(newComment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            } catch (Exception ex) {
                Console.Write(ex);
            }

            return View();
        }

        public IActionResult AnswerComment(int answerId) {
            ViewBag.AnswerId = answerId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnswerComment([Bind("Body,AnswerId")] AnswerComment answerComment) {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            Answer? answer = await _context.Answer.FindAsync(answerComment.AnswerId);

            if (user == null) {
                return NotFound();
            }

            AnswerComment newComment = new AnswerComment(answerComment.Body, user, answer);

            try {
                await _context.AnswerComment.AddAsync(newComment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            } catch (Exception ex) {
                Console.Write(ex);
            }

            return View();
        }

    }
}
