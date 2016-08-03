using HJPT.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HJPT.Model
{
    public interface IUserRepository
    {
        User Login(LoginForm form);
        void SignUp(SignUpForm form);
    }

    public class UserRepository : IUserRepository
    {
        private HJPTContext _context;
        public UserRepository(HJPTContext context)
        {
            _context = context;
        }

        public User Login(LoginForm form)
        {
            User user;
            try
            {
                user = _context.Users.Single(r => r.Username == form.Username || r.Email == form.Username);
            }
            catch (InvalidOperationException e)
            {
                throw new AuthenticationFailedException("无此用户", e);
            }
            if (form.Password != user.Password) throw new AuthenticationFailedException("密码错误");

            return user;
        }

        public void SignUp(SignUpForm form)
        {
            var newUser = new User();
            if (form == null || form.Username == null || form.Password == null || form.Email == null)
                throw new AuthenticationFailedException("缺少参数");
            newUser.Username = form.Username;
            newUser.Password = form.Password;
            newUser.Email = form.Email;
            newUser.StuID = form.StuID;

            //也许没用？
            if (_context.Users.Any(a => a.Username == newUser.Username))
                throw new AuthenticationFailedException("用户名已存在");
            if (_context.Users.Any(a => a.Email == newUser.Email))
                throw new AuthenticationFailedException("邮箱已存在");
            if (_context.Users.Any(a => (a.StuID == null && a.StuID == newUser.StuID)))
                throw new AuthenticationFailedException("学号已被注册");

            try
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }
            catch (Exception e) when (e is InvalidOperationException || e is DbUpdateException)
            {
                if (_context.Users.Any(a => a.Username == newUser.Username))
                    throw new AuthenticationFailedException("用户名已存在");
                if (_context.Users.Any(a => a.Email == newUser.Email))
                    throw new AuthenticationFailedException("邮箱已存在");
                if (_context.Users.Any(a => (a.StuID == null && a.StuID == newUser.StuID)))
                    throw new AuthenticationFailedException("学号已被注册");
            }
        }
    }
}
