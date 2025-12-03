using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using API_Laundry.Models;
using API_Laundry.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Laundry.Controllers
{
    public class CreateCuciSepatuRequest
    {
        public string NamaSepatu { get; set; } = string.Empty;
        public string JenisSepatu { get; set; } = string.Empty;
        public string JenisLayanan { get; set; } = string.Empty;
        public string TipeLayanan { get; set; } = string.Empty;
        public decimal TotalHarga { get; set; }
        public DateTime? TanggalCuci { get; set; }
        public string? Status { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SepatuController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public SepatuController(ApplicationDbContext db)
        {
            _db = db;
        }

        // PENGGUNA: CREATE ORDER CUCI SEPATU
        [HttpPost("CreateCuci")]
        [Authorize(Roles = "PENGGUNA,Pengguna")]
        public async Task<IActionResult> CreateCuciSepatu([FromBody] CreateCuciSepatuRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.NamaSepatu) ||
                string.IsNullOrWhiteSpace(request.JenisSepatu) ||
                string.IsNullOrWhiteSpace(request.JenisLayanan) ||
                string.IsNullOrWhiteSpace(request.TipeLayanan))
            {
                return BadRequest("Nama sepatu, jenis sepatu, jenis layanan, dan tipe layanan wajib diisi.");
            }

            if (request.TotalHarga <= 0)
            {
                return BadRequest("Total harga harus lebih besar dari 0.");
            }

            var model = new CuciSepatu
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                NamaSepatu = request.NamaSepatu,
                JenisSepatu = request.JenisSepatu,
                JenisLayanan = request.JenisLayanan,
                TipeLayanan = request.TipeLayanan,
                TotalHarga = request.TotalHarga,
                TanggalCuci = request.TanggalCuci ?? DateTime.UtcNow,
                Status = string.IsNullOrWhiteSpace(request.Status) ? "PROSES" : request.Status
            };

            _db.CuciSepatu.Add(model);
            await _db.SaveChangesAsync();
            return Ok(new
            {
                Message = "Order cuci sepatu berhasil dibuat.",
                Data = model
            });
        }

        // PENGGUNA: GET SEMUA ORDER CUCI SEPATU MILIK DIRI SENDIRI
        [HttpGet("GETMyCuciSepatuByLogin")]
        [Authorize(Roles = "PENGGUNA,Pengguna")]
        public async Task<IActionResult> GetMyCuciSepatu()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var list = await _db.CuciSepatu
                .Where(c => c.UserId == userId)
                .Include(c => c.User)
                .ToListAsync();

            return Ok(list);
        }

        // ADMIN: GET SEMUA ORDER CUCI SEPATU
        [HttpGet("GETAllCuciSepatu")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> GetAllCuciSepatu()
        {
            var list = await _db.CuciSepatu
                .Include(c => c.User)
                .ToListAsync();

            return Ok(list);
        }

        // ADMIN: GET ORDER BY USER ID
        [HttpGet("GETCuciSepatuByUserId/{userId}")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> GetCuciSepatuByUserId(string userId)
        {
            var list = await _db.CuciSepatu
                .Where(c => c.UserId == userId)
                .Include(c => c.User)
                .ToListAsync();

            return Ok(list);
        }

        // ADMIN: UPDATE STATUS ORDER
        [HttpPut("UpdateStatusCuciSepatu/{id}")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateStatusRequest request)
        {
            var order = await _db.CuciSepatu.FindAsync(id);
            if (order == null)
            {
                return NotFound("Order cuci sepatu tidak ditemukan.");
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest("Status tidak boleh kosong.");
            }

            order.Status = request.Status;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Message = "Status order cuci sepatu berhasil diperbarui.",
                Data = order
            });
        }

        // ADMIN: HAPUS ORDER CUCI SEPATU
        [HttpDelete("DeleteCuciSepatu/{id}")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> DeleteCuciSepatu(string id)
        {
            var order = await _db.CuciSepatu.FindAsync(id);
            if (order == null)
            {
                return NotFound("Order cuci sepatu tidak ditemukan.");
            }

            _db.CuciSepatu.Remove(order);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Order cuci sepatu berhasil dihapus." });
        }
    }
}

