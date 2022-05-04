#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SlackOverload.Data;
using SlackOverload.Models;

namespace SlackOverload.Controllers {
    public class AnswersController : Controller {
        private readonly ApplicationDbContext _context;

        public AnswersController(ApplicationDbContext context) {
            _context = context;
        }

        // GET: Answers
        public async Task<IActionResult> Index() {
            var applicationDbContext = _context.Answer.Include(a => a.Question).Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Answers/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var answer = await _context.Answer
                .Include(a => a.Question)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (answer == null) {
                return NotFound();
            }

            return View(answer);
        }

        // GET: Answers/Create
        public IActionResult Create(int questionId) {
            ViewBag.QuestionId = questionId;
            ViewBag.QuestionTitle = _context.Question.First(q => q.Id == questionId).Title;
            ViewBag.QuestionBody = _context.Question.First(q => q.Id == questionId).Body;
            ViewBag.UserId = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Body,Date,QuestionId,UserId,Vote,IsCorrect")] Answer answer) {
            ApplicationUser user = (ApplicationUser)_context.Users.First(u => u.Id == answer.UserId);

            if (user == null) {
                return NotFound();
            }

            Question question = _context.Question.First(q => q.Id == answer.QuestionId);
            Console.WriteLine(user.Id == question.UserId);
            if(user.Id == question.UserId) {
                TempData["Message"] = "You cannot answer your own question.";
                return RedirectToAction("Error");
            }
            Answer newAnswer = new Answer(answer.Body, question, user);

            try {
                _context.Answer.Add(newAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return View(answer);
        }

        // GET: Answers/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var answer = await _context.Answer.FindAsync(id);
            if (answer == null) {
                return NotFound();
            }
            ViewData["QuestionId"] = new SelectList(_context.Question, "Id", "Id", answer.QuestionId);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", answer.UserId);
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body,Date,QuestionId,UserId,Vote,IsCorrect")] Answer answer) {
            if (id != answer.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(answer);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!AnswerExists(answer.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionId"] = new SelectList(_context.Question, "Id", "Id", answer.QuestionId);
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", answer.UserId);
            return View(answer);
        }

        // GET: Answers/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var answer = await _context.Answer
                .Include(a => a.Question)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (answer == null) {
                return NotFound();
            }

            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var answer = await _context.Answer.FindAsync(id);
            _context.Answer.Remove(answer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnswerExists(int id) {
            return _context.Answer.Any(e => e.Id == id);
        }

        public IActionResult Error() {
            return View();
        }
    }
}
