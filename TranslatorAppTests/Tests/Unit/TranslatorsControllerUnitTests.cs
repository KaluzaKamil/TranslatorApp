using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorApp.Controllers;
using TranslatorApp.Models;
using TranslatorApp.Repositories;
using TranslatorApp.ViewModels;

namespace TranslatorAppTests.Tests.Unit
{
    public class TranslatorsControllerUnitTests
    {
        private List<TranslatorViewModel> GetTestTranslators()
        {
            var translators = new List<TranslatorViewModel>();

            translators.Add(new TranslatorViewModel()
            {
                Id = 1,
                Name = "test1",
                ApiUri = "testApiUri1",
                ApiUriParameters = "testApiUriParameters"
            });

            translators.Add(new TranslatorViewModel()
            {
                Id = 2,
                Name = "test2",
                ApiUri = "testApiUri2",
                ApiUriParameters = "testApiUriParameters"
            });

            return translators;
        }

        [Fact]
        public async Task Index_ReturnsViewAsResult_WithTranslatorsList()
        {
            //Arrange
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslatorsAsync())
                .ReturnsAsync(GetTestTranslators());
            var controller = new TranslatorsController(mockRepo.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TranslatorViewModel>>
                (viewResult.ViewData.Model);
            Assert.Equal(2, model.Count()); 
        }

        [Fact]
        public async Task Details_ReturnsViewAsResult_WithSingleTranslator()
        {
            //Arrange
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslatorAsync(1))
                .ReturnsAsync(new TranslatorViewModel 
                {
                    Id = 1, 
                    Name = "mockTranslator", 
                    ApiUri = "mockApiUri",
                    ApiUriParameters = "mockApiUriParameters"
                });
            var controller = new TranslatorsController(mockRepo.Object);

            //Act
            var result = await controller.Details(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<TranslatorViewModel>
                (viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WithMissingId()
        {
            //Arrange
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslatorAsync(It.IsAny<int>()))
                .ReturnsAsync((TranslatorViewModel)null);
            var controller = new TranslatorsController(mockRepo.Object);

            //Act
            var result = await controller.Details(1);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WithNotFoundTranslator()
        {
            //Arrange
            var mockRepo = new Mock<ITranslatorRepository>();
            var controller = new TranslatorsController(mockRepo.Object);

            //Act
            var result = await controller.Details(null);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsRedirectAndAddsTranslator_WhenModelStateIsValid()
        {
            //Arrange
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.AddTranslatorAsync(It.IsAny<TranslatorViewModel>()))
                .ReturnsAsync(1)
                .Verifiable();

            var controller = new TranslatorsController(mockRepo.Object);
            var newTranslator = new TranslatorViewModel()
            {
                Name = "newTranslator",
                ApiUri = "newTranslatorApiUri",
                ApiUriParameters = "newApiUriParameters"
            };

            //Act
            var result = await controller.Create(newTranslator);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockRepo.Verify();
        }

        [Fact]
        public async Task Edit_ReturnsRedirectToAction_WithEditedTranslator()
        {
            //Arrange
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.EditTranslatorAsync(It.IsAny<TranslatorViewModel>()))
                .ReturnsAsync(1)
                .Verifiable();

            var controller = new TranslatorsController(mockRepo.Object);
            var editedTranslator = new TranslatorViewModel()
            {
                Id = 1,
                Name = "editedTranslator",
                ApiUri = "editedTranslatorApiUri",
                ApiUriParameters = "editedApiUriParameters"
            };

            //Act
            var result = await controller.Edit(1, editedTranslator);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockRepo.Verify();
        }

        [Fact]
        public async Task Delete_ReturnsVewResult_WithChosenToDeleteTranslator()
        {
            //Arrange
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslatorAsync(1))
                .ReturnsAsync(new TranslatorViewModel()
                {
                    Id = 1,
                    Name = "newTranslator",
                    ApiUri = "newTranslatorApiUri",
                    ApiUriParameters = "newApiUriParameters"
                });

            var controller = new TranslatorsController(mockRepo.Object);

            //Act
            var result = await controller.Delete(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TranslatorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToAction_WithTranslatorDeleted()
        {
            //Arrange
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslatorAsync(1))
                .ReturnsAsync(new TranslatorViewModel()
                {
                    Id = 1,
                    Name = "newTranslator",
                    ApiUri = "newTranslatorApiUri",
                    ApiUriParameters = "newApiUriParameters"
                });
            mockRepo.Setup(repo => repo.DeleteTranslatorAsync(1))
                .ReturnsAsync(1)
                .Verifiable();

            var controller = new TranslatorsController(mockRepo.Object);

            //Act
            var result = await controller.DeleteConfirmed(1);

            //Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            mockRepo.Verify();
        }
    }
}
