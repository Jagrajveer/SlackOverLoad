using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SlackOverload.Models;

namespace SlackOverload.Data {
    public class ApplicationDbContext : IdentityDbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }
        public DbSet<SlackOverload.Models.Question> Question { get; set; }
        public DbSet<SlackOverload.Models.Answer> Answer { get; set; }
        public DbSet<SlackOverload.Models.QuestionComment> QuestionComment { get; set; }
        public DbSet<SlackOverload.Models.AnswerComment> AnswerComment { get; set; }
        public DbSet<SlackOverload.Models.QuestionTag> QuestionTag { get; set; }
        public DbSet<SlackOverload.Models.Vote> Vote { get; set; }
    }
}
