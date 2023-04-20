using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FYPWebAPI.Data;
using FYPWebAPI.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FYPWebAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class NetworkDevicesController : ControllerBase
    {
        private readonly ApplicationDbContext2 _context;
         

        public NetworkDevicesController(ApplicationDbContext2 context)
        {
            _context = context;
             
        }

        // GET: api/NetworkDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NetworkDevice>>> GetNetworkDevices()
        {
            return await _context.NetworkDevices.ToListAsync();
        }

        // GET: api/NetworkDevices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NetworkDevice>> GetNetworkDevice(int id)
        {
            var networkDevice = await _context.NetworkDevices.FindAsync(id);

            if (networkDevice == null)
            {
                return NotFound();
            }

            return networkDevice;
        }

        // PUT: api/NetworkDevices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNetworkDevice(int id, NetworkDevice networkDevice)
        {
            if (id != networkDevice.id)
            {
                return BadRequest();
            }

            _context.Entry(networkDevice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NetworkDeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/NetworkDevices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NetworkDevice>> PostDevice([FromBody] Object post)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(post.ToString());
            if (body["adminPassword"] == null || (body["adminPassword"].ToString() != Globals.AdminPassword))
            {
                return BadRequest();
            }
            int deviceId = 0;
            if (body["id"] != null)
            {
                deviceId = Int32.Parse(body["id"].ToString());
            }
            String deviceMac = body["deviceMAC"].ToString();
            NetworkDevice device = await _context.NetworkDevices.FindAsync(deviceId);
            if (device == null)
            {
                List<NetworkDevice> devices = await _context.NetworkDevices.Where(d => d.DeviceMAC == deviceMac).ToListAsync();
                if (devices.Count > 0)
                {
                    device = devices[0];
                }
                else
                {
                    device = new NetworkDevice();
                }
            }
            device.DeviceMAC = deviceMac;
            device.DeviceName = body["deviceName"];
            device.DeviceUUIDs = body["deviceUUIDs"];
            device.IsBluetooth = body["isBluetooth"];
            device.IsPublic = body["isPublic"];
            _context.NetworkDevices.Update(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNetworkDevice", new { id = device.id }, device);
        }

        // DELETE: api/NetworkDevices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNetworkDevice(int id)
        {
            var networkDevice = await _context.NetworkDevices.FindAsync(id);
            if (networkDevice == null)
            {
                return NotFound();
            }

            _context.NetworkDevices.Remove(networkDevice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NetworkDeviceExists(int id)
        {
            return _context.NetworkDevices.Any(e => e.id == id);
        }
    }
}
