using AutoMapper;
using Azure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Obtain_25_Cat_Images.Data;
using Obtain_25_Cat_Images.DTOs;
using Obtain_25_Cat_Images.Models.Entities;
using Obtain_25_Cat_Images.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Obtain_25_Cats_Tests.Services {
    public class CatServiceTests {

        #region Mock Data
        private readonly IMapper _mapper;

        public CatServiceTests() {
            var config = new MapperConfiguration(mc => {
                mc.AddMaps("Obtain_25_Cat_Images");
            });
            _mapper = config.CreateMapper();
        }

        private static AppDbContext GetInMemoryDbContext() {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private static HttpClient GetMockHttpClient<T>(T mockData) {
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

        #endregion

        #region Tests for FetchAndSaveCatsAsync()

        [Fact]
        public async Task FetchAndSaveCatsAsync_Valid_Save_Cats_And_Tags() {
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

        [Fact]
        public async Task FetchAndSaveCatsAsync_Should_Ignore_Duplicates_Cats() {
            var db = GetInMemoryDbContext();
            db.Cats.Add(new CatEntity { Id = 1, CatId = "CatId1", Image = "image234.jpg", Width = 300, Height = 120 });
            db.SaveChanges();

            var mockApiCats = new List<CatApiResponseDTO> {
                new() { Id = "CatId1", Url = "https://image543.jpg", Width = 400, Height = 220 }
            };
            var httpClient = GetMockHttpClient(mockApiCats);
            var service = new CatService(db, _mapper, httpClient);

            var result = await service.FetchAndSaveCatsAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task FetchAndSaveCatsAsync_Should_Fail_If_Response_Is_Null() {

            var db = GetInMemoryDbContext();
           
            var httpClient = GetMockHttpClient<List<CatApiResponseDTO>>(null!);
            var service = new CatService(db, _mapper, httpClient);

            var result = await service.FetchAndSaveCatsAsync();

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task FetchAndSaveCatsAsync_Should_Have_Tags_With_Empty_Name() {
            var db = GetInMemoryDbContext();
            var fakeApiCats = new List<CatApiResponseDTO>
            {
                new()
                {
                    Id = "cat_no_tags",
                    Url = "img.jpg",
                    Width = 300,
                    Height = 300,
                    Breads = [new() { Temperament = "" }]
                }
            };

            var httpClient = GetMockHttpClient(fakeApiCats);
            var service = new CatService(db, _mapper, httpClient);

            var result = await service.FetchAndSaveCatsAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Single().Tags.Should().BeEmpty();
        }
        #endregion

        #region GetCatByIdAsync()

        [Fact]
        public async Task GetCatByIdAsync_Cat_Exists_Should_Return_DTO() {

            var db = GetInMemoryDbContext();
            var cat = new CatEntity {
                Id = 1,
                CatId = "Cat01",
                Width = 400,
                Height = 300,
                Image = "https://piccat12332.jpg",
                CatTags = [
                        new() {
                            Tag = new TagEntity { Name = "Playful" }
                        },
                        new() {
                            Tag = new TagEntity { Name = "Independent" }
                        }
                    ]
            };
            db.Cats.Add(cat);
            await db.SaveChangesAsync();

            var service = new CatService(db, _mapper, new HttpClient());

            var result = await service.GetCatByIdAsync(1);

            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be("Cat01");
            result.Value.Image.Should().Be("https://piccat12332.jpg");
            result.Value.Tags.Select(t => t.Name).Should().Contain("Playful");
            result.Value.Tags.Select(t => t.Name).Should().BeEquivalentTo("Playful", "Independent");
        }

        [Fact]
        public async Task GetCatByIdAsync_Cat_Not_Exist_Should_Fail() {
            
            var db = GetInMemoryDbContext();
            var service = new CatService(db, _mapper, new HttpClient());

            var result = await service.GetCatByIdAsync(1); 

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message.Equals("Cat not found"));
        }
        #endregion

        #region GetCatsAsync()
        [Fact]
        public async Task GetCatsAsync_Should_Return_Cats_Paginated() {
            
            var db = GetInMemoryDbContext();
            for (int i = 1; i <= 20; i++) {

                db.Cats.Add(new() {
                    Id = i,
                    CatId = $"Cat{i}",
                    Image = $"https://cat{i}.jpg",
                    Width = 300,
                    Height = 200,
                    CatTags = [
                        new() { Tag = new() { Name = $"Playful{i}" } },
                        new() { Tag = new() { Name = $"Independent{i}" }
                        }
                    ]
                });
            }
            await db.SaveChangesAsync();

            var service = new CatService(db, _mapper, new HttpClient());

            var result1 = await service.GetCatsAsync(null, page: 2, pageSize: 5);

            result1.IsSuccess.Should().BeTrue();
            result1.Value.Should().HaveCount(5);
            result1.Value.First().Id.Should().Be("Cat6");
            result1.Value.First().Image.Should().Be("https://cat6.jpg");

            var count1 = 6;
            foreach (var cat in result1.Value) {
                cat.Id.Should().Be($"Cat{count1}");
                cat.Image.Should().Be($"https://cat{count1}.jpg");
                cat.Tags.Select(t => t.Name).Should().BeEquivalentTo(
                    $"Playful{count1}", $"Independent{count1}"
                );
                count1++;
            }

            var result2 = await service.GetCatsAsync(null, page: 5, pageSize: 4);

            result2.IsSuccess.Should().BeTrue();
            result2.Value.Should().HaveCount(4);
            result2.Value.First().Id.Should().Be("Cat17");
            result2.Value.First().Image.Should().Be("https://cat17.jpg");

            var count2 = 17;
            foreach (var cat in result2.Value) {
                cat.Id.Should().Be($"Cat{count2}");
                cat.Image.Should().Be($"https://cat{count2}.jpg");
                cat.Tags.Select(t => t.Name).Should().BeEquivalentTo(
                    $"Playful{count2}", $"Independent{count2}"
                );
                count2++;
            }
        }

        [Fact]
        public async Task GetCatsAsync_Should_Fail_If_No_Cats_Found() {
            
            var db = GetInMemoryDbContext();
            var service = new CatService(db, _mapper, new HttpClient());
            
            var result = await service.GetCatsAsync();

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Cats not found");
        }

        [Fact]
        public async Task GetCatsAsync_Should_Filter_By_Tag() {
            
            var db = GetInMemoryDbContext();

            var playfulCat = new CatEntity {
                CatId = "PlayfulId",
                Image = "http://Playful.jpg",
                CatTags = [new() { Tag = new() { Name = "Playful" } }]
            };

            var independentCat = new CatEntity {
                CatId = "IndependentId",
                Image = "http://Independent.jpg",
                CatTags = [new() { Tag = new() { Name = "Independent" } }]
            };

            var bothCat = new CatEntity {
                CatId = "BothId",
                Image = "http://Both.jpg",
                CatTags = [
                    new() { Tag = new() { Name = "Independent" } },
                    new() { Tag = new() { Name = "Playful" } },
                    new() { Tag = new() { Name = "Active" } },
                ]
            };

            var EmptyTagCat = new CatEntity {
                CatId = "EmptyId",
                Image = "http://Empty.jpg",
                CatTags = [new() { Tag = new() { Name = "" } }]
            };

            db.Cats.AddRange(playfulCat, independentCat, bothCat, EmptyTagCat);
            await db.SaveChangesAsync();

            var service = new CatService(db, _mapper, new HttpClient());

            var result1 = await service.GetCatsAsync(tag: "Active");

            result1.IsSuccess.Should().BeTrue();
            result1.Value.Should().ContainSingle();
            result1.Value.Single().Id.Should().Be("BothId");

            var result2 = await service.GetCatsAsync(tag: "Playful");

            result2.IsSuccess.Should().BeTrue();
            result2.Value.Should().HaveCount(2);
            result2.Value.Select(c => c.Id).Should().BeEquivalentTo("PlayfulId", "BothId");

            var result3 = await service.GetCatsAsync(tag: "Independent");

            result3.IsSuccess.Should().BeTrue();
            result3.Value.Should().HaveCount(2);
            result3.Value.Select(c => c.Id).Should().BeEquivalentTo("IndependentId", "BothId");
        }


        #endregion
    }
}
