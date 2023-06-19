using hospital_back.Controllers;
using hospital_back.Data;
using hospital_back.Dto;
using hospital_back.Enums;
using hospital_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace hospital_back.Tests.Controllers
{
    public class LoginControllerTests
    {
        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext())
            {
                var hashedPassword = HashPassword("jonas");

                context.Users.Add(new User
                {
                    Id = 1,
                    Name = "Test User",
                    Email = "email.Teste17@gmail.com",
                    Password = hashedPassword,
                    Role = RoleType.Doctor
                });
                context.SaveChanges();
            }

            using (var context = new AppDbContext())
            {
                var controller = new LoginController(context);
                var loginDto = new Login
                {
                    Email = "email.Teste17@gmail.com",
                    Password = "jonas",
                };

                // Act
                var result = await controller.Login(loginDto);

                // Assert
                Assert.IsType<OkObjectResult>(result);
                var okResult = (OkObjectResult)result;
                Assert.IsType<DoctorResult>(okResult.Value);
                var doctorResult = (DoctorResult)okResult.Value!;
                Assert.Equal("Test User", doctorResult.Name);
                Assert.Equal(RoleType.Doctor, doctorResult.Role);
            }
        }

        [Fact]
        public async Task Login_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext())
            {
                context.Users.Add(new User
                {
                    Id = 1,
                    Name = "Test User",
                    Email = "email.Teste17@gmail.com",
                    Password = HashPassword("jonas"),
                    Role = RoleType.Doctor
                });
                context.SaveChanges();
            }

            using (var context = new AppDbContext())
            {
                var controller = new LoginController(context);
                var loginDto = new Login
                {
                    Email = "invalid.email@example.com",
                    Password = "jonas",
                };

                // Act
                var result = await controller.Login(loginDto);

                // Assert
                Assert.IsType<NotFoundObjectResult>(result);
                var notFoundResult = (NotFoundObjectResult)result;
                Assert.Equal("Usuário não encontrado", notFoundResult.Value);
            }
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext())
            {
                context.Users.Add(new User
                {
                    Id = 1,
                    Name = "Test User",
                    Email = "email.Teste17@gmail.com",
                    Password = HashPassword("jonas"),
                    Role = RoleType.Doctor
                });
                context.SaveChanges();
            }

            using (var context = new AppDbContext())
            {
                var controller = new LoginController(context);
                var loginDto = new Login
                {
                    Email = "email.Teste17@gmail.com",
                    Password = "wrongpassword",
                };

                // Act
                var result = await controller.Login(loginDto);

                // Assert
                Assert.IsType<BadRequestObjectResult>(result);
                var badRequestResult = (BadRequestObjectResult)result;
                Assert.Equal("Senha incorreta", badRequestResult.Value);
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedPasswordBytes = sha256.ComputeHash(passwordBytes);
                return BitConverter.ToString(hashedPasswordBytes).Replace("-", "").ToLower();
            }
        }
    }
}
