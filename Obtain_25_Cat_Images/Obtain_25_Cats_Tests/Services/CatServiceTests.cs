using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Obtain_25_Cat_Images.Data;
using Obtain_25_Cat_Images.DTOs;
using Obtain_25_Cat_Images.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Obtain_25_Cats_Tests.Services {
    public class CatServiceTests {

        private readonly IMapper _mapper;

        public CatServiceTests() {
            var config = new MapperConfiguration(mc => {
                mc.AddMaps("Obtain_25_Cat_Images");
            });
            _mapper= config.CreateMapper();
        }

        private static AppDbContext GetInMemoryDbContext() {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private static HttpClient GetMockHttpClient(List<CatApiResponseDTO> mockData) {
            var json = JsonSerializer.Serialize(mockData);
            var msgHandlerMock = new Mock<HttpMessageHandler>();

            msgHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });

            return new HttpClient(msgHandlerMock.Object) {
                BaseAddress = new Uri("https://api.thecatapi.com/")
            };
        }

        [Fact]
        public async Task Fetch_And_Save_Cats_Has_Cats_And_Tags() {
            var db = GetInMemoryDbContext();
            var mockApiCats = new List<CatApiResponseDTO> {
                new() {
                    Id = "0XYvRd7oD",
                    Url = "https://cdn2.thecatapi.com/images/0XYvRd7oD.jpg",
                    Width = 800,
                    Height = 200,
                    Breads = [
                        new() {
                            Temperament = "Active, Energetic, Independent"
                        }
                    ]
                },
                new() {
                    Id = "5rYSrf3",
                    Url = "https://cdn2.thecatapi.com/images/5rYSrf3.jpg",
                    Width = 500,
                    Height = 150,
                    Breads = [
                        new() {
                            Temperament = "Intelligent, Gentle"
                        }
                    ]
                }
            };
            var httpClient = GetMockHttpClient(mockApiCats);
            var service = new CatService(db, _mapper, httpClient);

            var result = await service.FetchAndSaveCatsAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);

            var cat1 = result.Value[0];            
            cat1.Id.Should().Be("0XYvRd7oD");
            cat1.Image.Should().Be("https://cdn2.thecatapi.com/images/0XYvRd7oD.jpg");
            cat1.Width.Should().Be(800);
            cat1.Height.Should().Be(200);
            cat1.Tags.Select(t => t.Name).Should().Contain(["Active", "Energetic", "Independent"]);


            var cat2 = result.Value[1];
            cat2.Id.Should().Be("5rYSrf3");
            cat2.Image.Should().Be("https://cdn2.thecatapi.com/images/5rYSrf3.jpg");
            cat2.Width.Should().Be(500);
            cat2.Height.Should().Be(150);
            cat2.Tags.Select(t => t.Name).Should().Contain(["Intelligent", "Gentle"]);
        }

    }
}
