using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess.Models;

namespace Tamrinak_API.DataAccess.Seeding
{
    public static class SportSeeder
    {
        public static void SeedSports(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sport>().HasData(

                new Sport { SportId = 1, Name = "Football", Description = "Popular team sport played with a spherical ball" },
                new Sport { SportId = 2, Name = "Basketball", Description = "Fast-paced team sport played on a rectangular court" },
                new Sport { SportId = 3, Name = "Swimming", Description = "Individual sport performed in a pool or open water" },
                new Sport { SportId = 4, Name = "Tennis", Description = "Racket sport played individually or in pairs" },
                new Sport { SportId = 5, Name = "Handball", Description = "Team sport where players pass and shoot a ball" },
                new Sport { SportId = 6, Name = "Table Tennis", Description = "Indoor racket sport played on a table" },
                new Sport { SportId = 7, Name = "E-Sports", Description = "Gaming counter" },
                new Sport { SportId = 8, Name = "Bowling", Description = "Bowling is a target sport and recreational activity in which a player rolls a ball toward pins to get the highest score" },
                new Sport { SportId = 9, Name = "Squash", Description = "Racket sport played in an indoor court" },

                new Sport { SportId = 10, Name = "Weightlifting", Description = "Strength sport involving lifting heavy weights" },
                new Sport { SportId = 11, Name = "Boxing", Description = "Combat sport involving punching and defensive techniques" },
                new Sport { SportId = 12, Name = "Wrestling", Description = "Combat sport involving grappling techniques" },
                new Sport { SportId = 13, Name = "Karate", Description = "Japanese martial art focusing on striking techniques" },
                new Sport { SportId = 14, Name = "Taekwondo", Description = "Korean martial art known for high kicks and strikes" },
                new Sport { SportId = 15, Name = "Yoga", Description = "Mind-body practice focusing on strength, flexibility, and meditation" },

                new Sport { SportId = 16, Name = "Bouldering", Description = "Form of rock climbing performed without ropes" },
                new Sport { SportId = 17, Name = "Volleyball", Description = "Team sport played with a ball over a net" },
                new Sport { SportId = 18, Name = "Athletics", Description = "Track and field sports including running, jumping, and throwing" },
                new Sport { SportId = 19, Name = "Brazilian Jiu-Jitsu", Description = "Martial art focusing on ground fighting and submission" }

            );
        }
    }

}
