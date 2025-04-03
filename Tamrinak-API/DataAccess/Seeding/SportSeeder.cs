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
                new Sport { SportId = 3, Name = "Volleyball", Description = "Team sport played with a ball over a net" },
                new Sport { SportId = 4, Name = "Cricket", Description = "Bat-and-ball game popular in many countries" },
                new Sport { SportId = 5, Name = "Rugby", Description = "Physical contact team sport with oval-shaped ball" },
                new Sport { SportId = 6, Name = "Handball", Description = "Team sport where players pass and shoot a ball" },
                new Sport { SportId = 7, Name = "Ice Hockey", Description = "Team sport played on ice with a puck" },
                new Sport { SportId = 8, Name = "Baseball", Description = "Bat-and-ball game popular in North America" },
                
                new Sport { SportId = 9, Name = "Tennis", Description = "Racket sport played individually or in pairs" },
                new Sport { SportId = 10, Name = "Badminton", Description = "Racket sport played with a lightweight shuttlecock" },
                new Sport { SportId = 11, Name = "Table Tennis", Description = "Indoor racket sport played on a table" },
                new Sport { SportId = 12, Name = "Swimming", Description = "Individual sport performed in a pool or open water" },
                new Sport { SportId = 13, Name = "Athletics", Description = "Track and field sports including running, jumping, and throwing" },
                new Sport { SportId = 14, Name = "Boxing", Description = "Combat sport involving punching and defensive techniques" },
                new Sport { SportId = 15, Name = "Wrestling", Description = "Combat sport involving grappling techniques" },

                new Sport { SportId = 16, Name = "Karate", Description = "Japanese martial art focusing on striking techniques" },
                new Sport { SportId = 17, Name = "Judo", Description = "Japanese martial art focusing on throws and grappling" },
                new Sport { SportId = 18, Name = "Taekwondo", Description = "Korean martial art known for high kicks and strikes" },
                new Sport { SportId = 19, Name = "Brazilian Jiu-Jitsu", Description = "Martial art focusing on ground fighting and submission" },
        
                new Sport { SportId = 20, Name = "Squash", Description = "Racket sport played in an indoor court" },
                new Sport { SportId = 21, Name = "Padel", Description = "Racket sport combining elements of tennis and squash" },
              
                new Sport { SportId = 22, Name = "Yoga", Description = "Mind-body practice focusing on strength, flexibility, and meditation" },
                new Sport { SportId = 23, Name = "Pilates", Description = "Low-impact exercise method focusing on core strength" },
                new Sport { SportId = 24, Name = "Weightlifting", Description = "Strength sport involving lifting heavy weights" },
  
                new Sport { SportId = 25, Name = "Rock Climbing", Description = "Sport involving climbing natural rock formations or artificial walls" },
                new Sport { SportId = 26, Name = "Bouldering", Description = "Form of rock climbing performed without ropes" },

                new Sport { SportId = 27, Name = "Indoor Skydiving", Description = "Simulated skydiving experience in a vertical wind tunnel" },

                new Sport { SportId = 28, Name = "Water Polo", Description = "Team sport played in a swimming pool" },
                new Sport { SportId = 29, Name = "Synchronized Swimming", Description = "Artistic swimming performed in synchronized routines" }
            );
        }
    }

}
