using StingerCheck.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace StingerCheck.Controllers
{
    public class StingerController : ApiController
    {
        public StingerContext db = new StingerContext();

        [Authorize]
        public async Task<IHttpActionResult> PostVote(Stinger stinger)
        { 
            if (stinger.Movie == null)
            {
                return BadRequest();
            }
            stinger.Movie = await db.Movies.FindAsync(stinger.Movie.Id);

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user == null)
            {
                user = new User { Email = User.Identity.Name };
            }
            else if (await db.Stingers.Where(s => s.Movie.Id == stinger.Movie.Id && s.User.Id == stinger.User.Id).AnyAsync())
            {
                throw new ApplicationException("Already voted."); // TODO: Allow Revoting?
            }
            stinger.User = user;

            db.Stingers.Add(stinger);
            
            await db.SaveChangesAsync();

            // TODO: Optimization: move this into a queue-fed job
            var adjQuery = from s in db.Stingers
                           where s.Movie.Id == stinger.Movie.Id
                           group s by s.Movie.Id into g
                           select new
                           {
                               // TODO: How much doom is this query?

                               t = g.Count(),
                               hm = g.Count(s => s.HasMidStinger),
                               hf = g.Count(s => s.HasFinalStinger),

                               dmCE = g.Count(s => s.MidClosure > s.MidEgg),
                               dmCG = g.Count(s => s.MidClosure > s.MidGag),
                               dmCT = g.Count(s => s.MidClosure > s.MidTeaser),
                               dmEC = g.Count(s => s.MidEgg > s.MidClosure),
                               dmEG = g.Count(s => s.MidEgg > s.MidGag),
                               dmET = g.Count(s => s.MidEgg > s.MidTeaser),
                               dmGC = g.Count(s => s.MidGag > s.MidClosure),
                               dmGE = g.Count(s => s.MidGag > s.MidEgg),
                               dmGT = g.Count(s => s.MidGag > s.MidTeaser),
                               dmTC = g.Count(s => s.MidTeaser > s.MidClosure),
                               dmTE = g.Count(s => s.MidTeaser > s.MidEgg),
                               dmTG = g.Count(s => s.MidTeaser > s.MidGag),

                               dfCE = g.Count(s => s.FinalClosure > s.FinalEgg),
                               dfCG = g.Count(s => s.FinalClosure > s.FinalGag),
                               dfCT = g.Count(s => s.FinalClosure > s.FinalTeaser),
                               dfEC = g.Count(s => s.FinalEgg > s.FinalClosure),
                               dfEG = g.Count(s => s.FinalEgg > s.FinalGag),
                               dfET = g.Count(s => s.FinalEgg > s.FinalTeaser),
                               dfGC = g.Count(s => s.FinalGag > s.FinalClosure),
                               dfGE = g.Count(s => s.FinalGag > s.FinalEgg),
                               dfGT = g.Count(s => s.FinalGag > s.FinalTeaser),
                               dfTC = g.Count(s => s.FinalTeaser > s.FinalClosure),
                               dfTE = g.Count(s => s.FinalTeaser > s.FinalEgg),
                               dfTG = g.Count(s => s.FinalTeaser > s.FinalGag),

                               // NOTE: Let's never add things to the adjacency matrix
                           };
            var adjMatrix = await adjQuery.FirstAsync();

            // Simple majority votes
            stinger.Movie.HasMidStinger = (adjMatrix.hm / (float)adjMatrix.t) > 0.5f;
            stinger.Movie.HasFinalStinger = (adjMatrix.hf / (float)adjMatrix.t) > 0.5f;

            // This would be easier with C#6 indexing construction in the above query, oh well
            // NOTE: No seriously, lets never add things to these matrices
            var dm = new int[4, 4]
            {
                { 0,              adjMatrix.dmCE, adjMatrix.dmCG, adjMatrix.dmCT },
                { adjMatrix.dmEC, 0,              adjMatrix.dmEG, adjMatrix.dmET },
                { adjMatrix.dmGC, adjMatrix.dmGE, 0,              adjMatrix.dmGT },
                { adjMatrix.dmTC, adjMatrix.dmTE, adjMatrix.dmTG, 0              },
            };
            var df = new int[4, 4]
            {
                { 0,              adjMatrix.dfCE, adjMatrix.dfCG, adjMatrix.dfCT },
                { adjMatrix.dfEC, 0,              adjMatrix.dfEG, adjMatrix.dfET },
                { adjMatrix.dfGC, adjMatrix.dfGE, 0,              adjMatrix.dfGT },
                { adjMatrix.dfTC, adjMatrix.dfTE, adjMatrix.dfTG, 0              },
            };
            var pm = new int[4, 4];
            var pf = new int[4, 4];

            // Floyd-Warshall the widest path to the Schulze candidates
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i != j)
                    {
                        if (dm[i, j] > dm[j, i])
                        {
                            pm[i, j] = dm[i, j];
                        }
                        if (df[i, j] > df[j, i])
                        {
                            pf[i, j] = df[i, j];
                        }
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i != j)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            if (i != k && j != k)
                            {
                                pm[j, k] = Math.Max(pm[j, k], Math.Min(pm[j, i], pm[i, k]));
                                pf[j, k] = Math.Max(pf[j, k], Math.Min(pf[j, i], pf[i, k]));
                            }
                        }
                    }
                }
            }

            stinger.Movie.MidClosure = (short)((pm[0, 1] > pm[1, 0] ? 1 : 0) + (pm[0, 2] > pm[2, 0] ? 1 : 0) + (pm[0, 3] > pm[3, 0] ? 1 : 0));
            stinger.Movie.MidEgg     = (short)((pm[1, 0] > pm[0, 1] ? 1 : 0) + (pm[1, 2] > pm[2, 1] ? 1 : 0) + (pm[1, 3] > pm[3, 1] ? 1 : 0));
            stinger.Movie.MidGag     = (short)((pm[2, 0] > pm[0, 2] ? 1 : 0) + (pm[2, 1] > pm[1, 2] ? 1 : 0) + (pm[2, 3] > pm[3, 2] ? 1 : 0));
            stinger.Movie.MidTeaser  = (short)((pm[3, 0] > pm[0, 3] ? 1 : 0) + (pm[3, 1] > pm[1, 3] ? 1 : 0) + (pm[3, 2] > pm[2, 3] ? 1 : 0));

            stinger.Movie.FinalClosure = (short)((pf[0, 1] > pf[1, 0] ? 1 : 0) + (pf[0, 2] > pf[2, 0] ? 1 : 0) + (pf[0, 3] > pf[3, 0] ? 1 : 0));
            stinger.Movie.FinalEgg     = (short)((pf[1, 0] > pf[0, 1] ? 1 : 0) + (pf[1, 2] > pf[2, 1] ? 1 : 0) + (pf[1, 3] > pf[3, 1] ? 1 : 0));
            stinger.Movie.FinalGag     = (short)((pf[2, 0] > pf[0, 2] ? 1 : 0) + (pf[2, 1] > pf[1, 2] ? 1 : 0) + (pf[2, 3] > pf[3, 2] ? 1 : 0));
            stinger.Movie.FinalTeaser  = (short)((pf[3, 0] > pf[0, 3] ? 1 : 0) + (pf[3, 1] > pf[1, 3] ? 1 : 0) + (pf[3, 2] > pf[2, 3] ? 1 : 0));

            await db.SaveChangesAsync();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
