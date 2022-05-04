#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using SlackOverload.Data;
using SlackOverload.Models;

namespace SlackOverload.Controllers {
    [Authorize]
    public class QuestionsController : Controller {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public QuestionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }


            var question = await _context.Question
                .Include(q => q.User)
                .Include(q => q.Answers)
                .ThenInclude(a => a.AnswerComments)
                .Include(q => q.QuestionComments)
                .FirstOrDefaultAsync(m => m.Id == id);

            ICollection<Answer> Answers = new List<Answer>();
            Answers.AddRange(question.Answers.Where(a => a.IsCorrect));
            Answers.AddRange(question.Answers.Where(a => !a.IsCorrect));
            question.Answers = Answers;
            if (question == null) {
                return NotFound();
            }

            return View(question);
        }

        // GET: Questions/Create
        public IActionResult Create() {

            ViewData["UserId"] = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Body,UserId")] Question question) {
            ApplicationUser user = await _userManager.FindByIdAsync(question.UserId);
            if (user == null) {
                return NotFound();
            }

            Question newQuestion = new Question(question.Title, question.Body, user);

            try {

                _context.Question.Add(newQuestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var question = await _context.Question.FindAsync(id);
            if (question == null) {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", question.UserId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,UserId")] Question question) {
            if (id != question.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!QuestionExists(question.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", question.UserId);
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var question = await _context.Question
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null) {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var question = await _context.Question.FindAsync(id);
            _context.Question.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id) {
            return _context.Question.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Vote(int value, int answerId) {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            Vote vote = null;
            if (_context.Vote.Any()) {
                vote = _context.Vote.Where(v => v.UserId == user.Id & v.AnswerId == answerId).FirstOrDefault();
            }

            Answer answer = await _context.Answer.FirstOrDefaultAsync(a => a.Id == answerId);

            if (user.Id == answer.UserId) {
                TempData["Message"] = "You cannot vote your own answer";
                return RedirectToAction("Error");
            }

            if (vote == null) {
                vote = new Vote(value, user, answer);
                answer.Vote++;
                _context.Answer.Update(answer);
                _context.Vote.Add(vote);
            } else {
                vote.VoteValue = value;
                
                _context.Vote.Update(vote);
            }

            ApplicationUser AnswerUser = await _userManager.FindByIdAsync(answer.UserId);
            
            if (value > 0) {
                AnswerUser.Reputation += 5;
            } else if (value < 0) {
                AnswerUser.Reputation -= 5;
            }

            await _userManager.UpdateAsync(AnswerUser);

            try {
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return View();
        }

        public async Task<IActionResult> CorrectAnswer(int questionId, int answerId) {
            ViewBag.Question = await _context.Question.FindAsync(questionId);
            Answer answer = await _context.Answer.FindAsync(answerId);
            return View(answer);
        }

        [HttpPost]
        public async Task<IActionResult> CorrectAnswer([Bind("Id")] Answer ans) {
            Answer answer = await _context.Answer.FindAsync(ans.Id);

            List<Answer> answers = await _context.Answer.Where(a => a.QuestionId == answer.QuestionId).ToListAsync();

            if(answer == null) {
                return NotFound();
            } else {
                foreach (var a in answers) {
                    if (a.IsCorrect) {
                        a.IsCorrect = false;
                        _context.Answer.Update(a);
                    }
                }
                answer.IsCorrect = true;
            }

            try {
                _context.Answer.Update(answer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }


            return View();
        }
        public IActionResult Error() {
            return View();
        }
    }
}
