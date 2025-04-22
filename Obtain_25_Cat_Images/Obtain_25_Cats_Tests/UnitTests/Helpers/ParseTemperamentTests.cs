using Obtain_25_Cat_Images.Helpers;

namespace Obtain_25_Cats_Tests.UnitTests.Helpers
{
    public class ParseTemperamentTests
    {

        [Fact]
        public void Parse_Temperament_Comma_Seperated_Correctly()
        {
            var result = Parser.ParseTemperament("Active, Energetic, Independent, Intelligent");
            Assert.Equal(["Active", "Energetic", "Independent", "Intelligent"], result);
        }

        [Fact]
        public void Return_Empty_List_If_Null_Input()
        {
            var result = Parser.ParseTemperament(null!);
            Assert.Empty(result);
        }

        [Fact]
        public void Parse_Temperament_Trims_Whitespace()
        {
            var result = Parser.ParseTemperament("    Active  ,   ,    Independent  ,    Intelligent");
            Assert.Equal(["Active", "Independent", "Intelligent"], result);
        }
    }
}