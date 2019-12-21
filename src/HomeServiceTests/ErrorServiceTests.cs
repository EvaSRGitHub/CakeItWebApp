using CakeWebApp.Services.Common.CommonServices;
using System.Collections.Generic;
using Xunit;

namespace CakeItWebApp.Services.Common.Tests
{
    public class ErrorServiceTests
    {
        [Fact]
        public void PassErrorParam_ShouldSaveErrorValueToErrorParamObj()
        {
            //Arrange
            var service = new ErrorService();

            string error = "Some error occurred";

            //Act
            service.PassErrorParam(error);

            //Assert
            Assert.Equal(error, service.ErrorParm);

        }

        [Fact]
        public void GetErrorModel_ShouldReturnErrorCollection()
        {
            //Arrange
            var service = new ErrorService();

            ICollection<string> errors = new List<string>
            {
                "Name fieled is required",
                "Email fieled is required"
            };

            //Act
            var result = service.GetErrorModel(errors);

            //Assert
            Assert.NotNull(result);
        }
    }
}
