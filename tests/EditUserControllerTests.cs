using hospital_back.Controllers;
using hospital_back.Data;
using hospital_back.Dto;
using hospital_back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace hospital_back.Tests.Controllers
{
    public class EditUserByIdControllerTests
    {
        [Fact]
        public async Task EditUser_ExistingUser_ReturnsOkResult()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext())
            {
                context.Users.Add(new User
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Password = "password",
                    CellPhone = 1234567890
                });
                context.SaveChanges();
            }

            using (var context = new AppDbContext())
            {
                var controller = new EditUserByIdController(context);
                var model = new EditUserModel
                {
                    Name = "John Updated",
                    Email = "john.updated@example.com",
                    Password = "newpassword",
                    CellPhone = 88992075680
                };

                var result = await controller.EditUser(1, model);

                Assert.IsType<OkObjectResult>(result);
                var okResult = (OkObjectResult)result;
                Assert.Equal("Dados salvos com sucesso!", okResult.Value);

                using (var dbContext = new AppDbContext())
                {
                    var updatedUser = await dbContext.Users.FindAsync(1);
                    Assert.NotNull(updatedUser);
                    Assert.Equal("John Updated", updatedUser.Name);
                    Assert.Equal("john.updated@example.com", updatedUser.Email);
                    Assert.Equal("newpassword", updatedUser.Password);
                    Assert.Equal(9876543210, updatedUser.CellPhone);
                }
            }
        }

        [Fact]
        public async Task EditUser_NonExistingUser_ReturnsNotFound()
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
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Password = "password",
                    CellPhone = 1234567890
                });
                context.SaveChanges();
            }

            using (var context = new AppDbContext())
            {
                var controller = new EditUserByIdController(context);
                var model = new EditUserModel
                {
                    Name = "John Updated",
                    Email = "john.updated@example.com",
                    Password = "newpassword",
                    CellPhone = 1234567890
                };

                var result = await controller.EditUser(2, model);

                Assert.IsType<NotFoundResult>(result);

                using (var dbContext = new AppDbContext())
                {
                    var user = await dbContext.Users.FindAsync(1);
                    Assert.NotNull(user);
                    Assert.Equal("John Doe", user.Name);
                    Assert.Equal("john.doe@example.com", user.Email);
                    Assert.Equal("password", user.Password);
                    Assert.Equal(1234567890, user.CellPhone);
                }
            }
        }
    }
}
