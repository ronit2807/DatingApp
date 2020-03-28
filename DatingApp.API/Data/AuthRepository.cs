using System.IO.Compression;
using System;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Models;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash,passwordSalt;
            CreatePasswordHash(password,out passwordHash,out passwordSalt);

            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<User> Login(string username, string password)
        {
            var user=await _context.Users.FirstOrDefaultAsync(x=>x.Username==username);

            if(user==null)
            {
                return null;
            }

            if(!VerifyPassword(password,user.PasswordHash, user.PasswordSalt)) return null;

            return user;
        }

        
        public async Task<bool> UserExists(string username)
        {
            if( await _context.Users.AnyAsync(x=>x.Username == username))
                return true;

            return false;
        }

        private void CreatePasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt=hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password,byte[] PasswordHash,byte[] PasswordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512(PasswordSalt))
            {
                var computedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i=0;i<computedHash.Length;i++)
                {
                    if(computedHash[i] != PasswordHash[i]) return false;
                }
            }
            return true;
        }

    }
}