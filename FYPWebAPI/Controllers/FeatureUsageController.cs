using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty featureUsages, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FYPWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FeatureUsageController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;

        public FeatureUsageController(ApplicationDbContext2 context)
        {
            _context = context;
        }

        // GET: api/FeatureUsage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeatureUsage>>> GetFeatureUsages()
        {
            return await _context.FeatureUsage.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<FeatureUsage>> PostFeatureUsage([FromBody] Object post)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(post.ToString());
            var macAddress = body["macAddress"];
            var macHashString = macAddress.ToString(); 
            foreach (var feature in body["features"])
            {
                var featureName = feature["featureName"].ToString();
                FeatureUsage featureUsage = await getFeatureUsageOrNew(macHashString, featureName);
                int usageCount = feature["usageCountInSeconds"];
                if (featureUsage.usageCountInSeconds == null || featureUsage.usageCountInSeconds == 0)
                {
                    featureUsage.usageCountInSeconds = usageCount;
                }
                else
                {
                    if (featureUsage.usageCountInSeconds - 60 > usageCount)
                    {
                        featureUsage.usageCountInSeconds += usageCount;
                    }
                    else
                    {
                        featureUsage.usageCountInSeconds = usageCount;
                    }
                }

                if (featureUsage.id == null || featureUsage.id == 0)
                {
                    featureUsage.id = DateTime.Now.Millisecond;
                    _context.FeatureUsage.Add(featureUsage);
                }
                else
                {
                    _context.FeatureUsage.Update(featureUsage);
                }
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<FeatureUsage> getFeatureUsageOrNew(string macHash, string feature)
        {
            var featureId = 0;
            var featureObjectList = _context.FeatureUsage.Where(fU => fU.macHash == macHash && fU.featureName == feature).ToList();
            var featureObject = featureObjectList.Count > 0 ? featureObjectList[0] : null;


            featureObject ??= new FeatureUsage();
            featureObject.macHash = macHash;
            featureObject.featureName = feature;

            return featureObject;
        }
    }
}
