using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Obtain_25_Cat_Images.Data;
using Obtain_25_Cat_Images.Models.Entities;
using System.Net;

namespace Obtain_25_Cats_Tests.IntegrationTests {
    public class CatsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>> {
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _services;

        public CatsControllerTests(CustomWebApplicationFactory<Program> factory) {
            CustomWebApplicationFactory<Program>.TestDbName = Guid.NewGuid().ToString();
            _httpClient = factory.CreateClient();
            _services = factory.Services;
        }

        [Fact]
        public async Task Fetch_Cats_Should_Save_Cats_And_Return_200() {
            var response = await _httpClient.PostAsync("/api/cats/fetch", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
            content.Should().Contain("id");
            content.Should().Contain("image");
            content.Should().Contain("tags");
        }

        [Fact]
        public async Task GetCatById_Should_Return_200_If_Exists() {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cat = new CatEntity {
                Id = 1,
                CatId = "Cat1",
                Width = 300,
                Height = 200,
                Image = "https://imgCat1.jpg",
                CatTags = [
                    new CatTagEntity { Tag = new TagEntity { Name = "Playful" } }
                ]
            };
            db.Cats.Add(cat);
            await db.SaveChangesAsync();

            var response = await _httpClient.GetAsync("/api/cats/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Cat1");
        }

        [Fact]
        public async Task GetCatById_Should_Return_404_If_Not_Found() {
            var response = await _httpClient.GetAsync("/api/cats/99999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCats_With_Tag_Should_Return_Success() {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Cats.AddRange(
                new CatEntity {
                    Id = 44445,
                    CatId = "Cat1",
                    Image = "img1.jpg",
                    CatTags = [
                        new() { Tag = new TagEntity { Name = "Active" } },
                        new() { Tag = new TagEntity { Name = "Playful" } }
                    ]
                },
                new CatEntity {
                    Id=44446,
                    CatId = "Cat2",
                    Image = "img2.jpg",
                    CatTags = [
                        new() { Tag = new TagEntity { Name = "Active" } },
                        new() { Tag = new TagEntity { Name = "Independent" } }
                    ]
                }
            );
            await db.SaveChangesAsync();

            var response = await _httpClient.GetAsync("/api/cats?tag=Active&page=1&pageSize=5");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Active");
            content.Should().Contain("Independent");
            content.Should().Contain("Playful");
        }

    }
}
