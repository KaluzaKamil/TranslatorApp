using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorApp.ApiCaller;
using TranslatorApp.Controllers;
using TranslatorApp.Repositories;
using TranslatorApp.ViewModels;

namespace TranslatorAppTests.Tests.Unit
{
    public class TranslationsControllerUnitTests
    {
        private List<TranslationViewModel> GetTestTranslations()
        {
            var translations = new List<TranslationViewModel>();

            translations.Add(new TranslationViewModel
            {
                Id = 1,
                OriginalText = "Hello",
                TranslationText = "HelloTranslated",
                DateTranslated = DateTime.Now,
                TranslatorId = 1,
                Translator = new TranslatorViewModel
                {
                    Id = 1,
                    Name = "testTranslator",
                    ApiUri = "testApiUri"
                }
            });

            translations.Add(new TranslationViewModel
            {
                Id = 2,
                OriginalText = "Hello",
                TranslationText = "HelloTranslated",
                DateTranslated = DateTime.Now,
                TranslatorId = 1,
                Translator = new TranslatorViewModel
                {
                    Id = 1,
                    Name = "testTranslator",
                    ApiUri = "testApiUri"
                }
            });

            return translations;
        }
        private List<TranslatorViewModel> GetTestTranslators()
        {
            var translators = new List<TranslatorViewModel>();

            translators.Add(new TranslatorViewModel()
            {
                Id = 1,
                Name = "test1",
                ApiUri = "testApiUri1"
            });

            translators.Add(new TranslatorViewModel()
            {
                Id = 2,
                Name = "test2",
                ApiUri = "testApiUri2"
            });

            return translators;
        }

        [Fact]
        public async Task Index_ReturnsViewAsResult_WithTranslationsList()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<TranslationsController>>();
            var mockApiCaller = new Mock<ITranslatorApiCaller>();
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslationsAsync())
                .ReturnsAsync(GetTestTranslations());
            var controller = new TranslationsController(mockRepo.Object, mockLogger.Object, mockApiCaller.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TranslationViewModel>>
                (viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ReturnsViewAsResult_WithSingleTranslation()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<TranslationsController>>();
            var mockApiCaller = new Mock<ITranslatorApiCaller>();
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslationAsync(1))
                .ReturnsAsync(new TranslationViewModel()
                {
                    Id = 1,
                    OriginalText = "Hello",
                    TranslationText = "HelloTranslated",
                    DateTranslated = DateTime.Now,
                    TranslatorId = 1,
                    Translator = new TranslatorViewModel
                    {
                        Id = 1,
                        Name = "testTranslator",
                        ApiUri = "testApiUri"
                    }
                });
            var controller = new TranslationsController(mockRepo.Object, mockLogger.Object, mockApiCaller.Object);

            //Act
            var result = await controller.Details(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<TranslationViewModel>
                (viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Details_returnsNotFound_WithMissingId()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<TranslationsController>>();
            var mockApiCaller = new Mock<ITranslatorApiCaller>();
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslationAsync(It.IsAny<int>()))
                .ReturnsAsync((TranslationViewModel)null);
            var controller = new TranslationsController(mockRepo.Object, mockLogger.Object, mockApiCaller.Object);

            //Act
            var result = await controller.Details(1);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Translate_ReturnsViewAsResult_WithTranslatorsInViewBag()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<TranslationsController>>();
            var mockApiCaller = new Mock<ITranslatorApiCaller>();
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslatorsAsync())
                .ReturnsAsync(GetTestTranslators);
            var controller = new TranslationsController(mockRepo.Object, mockLogger.Object, mockApiCaller.Object);

            //Act
            var result = await controller.Translate();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var selectList = Assert.IsAssignableFrom<SelectList>(viewResult.ViewData["Translators"]);
            Assert.Equal(2, selectList.Count());
        }

        [Fact]
        public async Task Translate_ReturnsContentAndAddsTranslation_WhenModelStateIsValid()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<TranslationsController>>();
            var mockApiCaller = new Mock<ITranslatorApiCaller>();
            var mockRepo = new Mock<ITranslatorRepository>();
            var mockValidator = new Mock<IObjectModelValidator>();
            mockRepo.Setup(repo => repo.AddTranslationAsync(It.IsAny<TranslationViewModel>()))
                .ReturnsAsync(1)
                .Verifiable();
            mockRepo.Setup(repo => repo.GetTranslatorAsync(1))
                .ReturnsAsync(new TranslatorViewModel
                {
                    Id = 1,
                    Name = "testTranslator",
                    ApiUri = "testApiUri"
                })
                .Verifiable();
            mockApiCaller.Setup(caller => caller.GetTranslationAsync("Hello", "testApiUri"))
                .ReturnsAsync("translated")
                .Verifiable();
            mockValidator.Setup(validator => validator.Validate(It.IsAny<ActionContext>(),
                                                                It.IsAny<ValidationStateDictionary>(),
                                                                It.IsAny<string>(),
                                                                It.IsAny<Object>()));
            var controller = new TranslationsController(mockRepo.Object, mockLogger.Object, mockApiCaller.Object);
            controller.ObjectValidator = mockValidator.Object;

            //Act
            var result = await controller.Translate(new TranslationViewModel
            {
                Id = 1,
                OriginalText = "Hello",
                TranslatorId = 1
            });

            //Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("translated", contentResult.Content);
            mockRepo.Verify();
            mockApiCaller.Verify();
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithChosenToDeleteTranslation()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<TranslationsController>>();
            var mockApiCaller = new Mock<ITranslatorApiCaller>();
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslationAsync(1))
                .ReturnsAsync(new TranslationViewModel()
                {
                    Id = 1,
                    OriginalText = "Hello",
                    TranslationText = "HelloTranslated",
                    DateTranslated = DateTime.Now,
                    TranslatorId = 1,
                    Translator = new TranslatorViewModel
                    {
                        Id = 1,
                        Name = "testTranslator",
                        ApiUri = "testApiUri"
                    }
                });
            var controller = new TranslationsController(mockRepo.Object, mockLogger.Object, mockApiCaller.Object);

            //Act
            var result = await controller.Delete(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<TranslationViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirectToAction_WithTranslationDeleted()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<TranslationsController>>();
            var mockApiCaller = new Mock<ITranslatorApiCaller>();
            var mockRepo = new Mock<ITranslatorRepository>();
            mockRepo.Setup(repo => repo.GetTranslationAsync(1))
                .ReturnsAsync(new TranslationViewModel()
                {
                    Id = 1,
                    OriginalText = "Hello",
                    TranslationText = "HelloTranslated",
                    DateTranslated = DateTime.Now,
                    TranslatorId = 1,
                    Translator = new TranslatorViewModel
                    {
                        Id = 1,
                        Name = "testTranslator",
                        ApiUri = "testApiUri"
                    }
                });
            mockRepo.Setup(repo => repo.DeleteTranslationAsync(1))
                .ReturnsAsync(1)
                .Verifiable();
            var controller = new TranslationsController(mockRepo.Object, mockLogger.Object, mockApiCaller.Object);

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
